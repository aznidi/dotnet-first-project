using FIRST.DTOs.Files;
using FIRST.Models.Files;

namespace FIRST.Services.Files
{
    public interface IFileStorageService
    {
        Task<List<StoredFilesDto>> GetFilesAsync(CancellationToken ct = default);

        Task<StoredFile> SaveAsync(IFormFile file, CancellationToken ct = default);
        Task<(StoredFile meta, Stream stream)> OpenReadAsync(Guid id, CancellationToken ct = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
    }
}
