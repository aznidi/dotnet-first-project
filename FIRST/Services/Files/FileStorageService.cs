using FIRST.Data;
using FIRST.DTOs.Files;
using FIRST.Models;
using FIRST.Models.Files;
using FIRST.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace FIRST.Services.Files
{
    public class FileStorageService : IFileStorageService
    {
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        { ".txt", ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx", ".xls", ".xlsx" };

        private static readonly Dictionary<string, string> ContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            [".txt"] = "text/plain",
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".pdf"] = "application/pdf",
            [".doc"] = "application/msword",
            [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            [".xls"] = "application/vnd.ms-excel",
            [".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
        };

        private readonly AppDbContext _db;
        private readonly FileStorageOptions _opt;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _http;

        public FileStorageService(AppDbContext db, IOptions<FileStorageOptions> opt, IWebHostEnvironment env, IHttpContextAccessor http)
        {
            _db = db;
            _opt = opt.Value;
            _http = http;
            _env = env;
        }

        private int GetCurrentUserIdOrThrow()
        {
            var user = _http.HttpContext?.User;

            if (user?.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("Unauthenticated user.");

            var userIdStr =
                user.FindFirstValue(ClaimTypes.NameIdentifier) ??
                user.FindFirstValue("sub");

            if (!int.TryParse(userIdStr, out var currentUserId))
                throw new UnauthorizedAccessException("Invalid token: missing user id");

            return currentUserId;
        }

        private static string ToSafeFolderName(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return "user";

            var cleaned = input.Trim().Replace(' ', '_');

            foreach (var c in Path.GetInvalidFileNameChars())
                cleaned = cleaned.Replace(c, '_');

            if (cleaned.Length > 50) cleaned = cleaned[..50];

            return cleaned;
        }

        public async Task<StoredFile> SaveAsync(IFormFile file, CancellationToken ct = default)
        {
            if (file is null || file.Length == 0)
                throw new InvalidOperationException("No file selected.");

            if (file.Length > _opt.MaxSizeBytes)
                throw new InvalidOperationException($"File too large. Max = {_opt.MaxSizeBytes} bytes.");

            var ext = Path.GetExtension(file.FileName);
            if (string.IsNullOrWhiteSpace(ext) || !AllowedExtensions.Contains(ext))
                throw new InvalidOperationException("File extension not allowed." + 
                    $" Allowed: {string.Join(", ", AllowedExtensions)}" + 
                    $" Your file extension: '{ext}'");

            var currentUserId = GetCurrentUserIdOrThrow();
            User ?user = await _db.Users.FirstOrDefaultAsync(u => u.Id == currentUserId, ct);
            if (user == null)
                throw new UnauthorizedAccessException("Invalid user.");

            var safeName = ToSafeFolderName(user.FullName);

            var userFolder = $"{user.Id}_{safeName}"; 

            var now = DateTime.UtcNow;


            var relativeDir = Path.Combine(_opt.UploadsFolder, userFolder, now.Year.ToString());
            var absoluteDir = Path.Combine(_env.ContentRootPath, _opt.RootPath, relativeDir);
            Directory.CreateDirectory(absoluteDir);

            var id = Guid.NewGuid();
            var storedName = $"{id}{ext.ToLowerInvariant()}";
            var absolutePath = Path.Combine(absoluteDir, storedName);

            // IMPORTANT: create new, et stream (pas de buffer en mémoire)
            await using (var fs = new FileStream(absolutePath, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, useAsync: true))
            {
                await file.CopyToAsync(fs, ct);
            }

            

            var meta = new StoredFile
            {
                Id = id,
                OriginalName = Path.GetFileName(file.FileName), // nettoie les paths
                StoredName = storedName,
                Size = file.Length,
                ContentType = ContentTypes.TryGetValue(ext, out var ct2) ? ct2 : (file.ContentType ?? "application/octet-stream"),
                RelativePath = Path.Combine(_opt.RootPath, relativeDir, storedName).Replace('\\', '/'),
                CreatedAt = DateTime.UtcNow,
                UploadedByUserId = currentUserId
            };

            _db.StoredFiles.Add(meta);
            await _db.SaveChangesAsync(ct);

            return meta;
        }

        public async Task<(StoredFile meta, Stream stream)> OpenReadAsync(Guid id, CancellationToken ct = default)
        {
            var meta = await _db.StoredFiles.FirstOrDefaultAsync(x => x.Id == id, ct)
                    ?? throw new KeyNotFoundException("File not found.");

            var absolutePath = Path.Combine(_env.ContentRootPath, meta.RelativePath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(absolutePath))
                throw new FileNotFoundException("File missing on disk.", absolutePath);

            var stream = new FileStream(absolutePath, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, useAsync: true);
            return (meta, stream);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var meta = await _db.StoredFiles.FirstOrDefaultAsync(x => x.Id == id, ct)
                    ?? throw new KeyNotFoundException("File not found.");

            var absolutePath = Path.Combine(_env.ContentRootPath, meta.RelativePath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(absolutePath))
            {
                File.Delete(absolutePath);
            }

            _db.StoredFiles.Remove(meta);
            await _db.SaveChangesAsync(ct);

            return true;
        }

        public async Task<List<StoredFilesDto>> GetFilesAsync(CancellationToken ct = default)
        {
            var currentUserId = GetCurrentUserIdOrThrow();

            return await _db.StoredFiles
                .AsNoTracking()
                .Where(f => f.UploadedByUserId == currentUserId)
                .OrderByDescending(f => f.CreatedAt)
                .Select(f => new StoredFilesDto
                {
                    Id = f.Id,
                    OriginalName = f.OriginalName,
                    ContentType = f.ContentType,
                    Size = f.Size,
                    CreatedAt = f.CreatedAt,
                    DownloadUrl = ""
                })
                .ToListAsync(ct);
        }
    }
}