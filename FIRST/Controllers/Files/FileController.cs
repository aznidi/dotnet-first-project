using FIRST.Services.Files;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIRST.Controllers.Files;

[ApiController]
[Authorize]
[Route("api/files")]


public class FileController : BaseApiController
{
    private readonly IFileStorageService _fileStorage;

    public FileController(IFileStorageService fileStorage)
    {
        _fileStorage = fileStorage;
    }

    [HttpGet]
    public async Task<IActionResult> GetFilesAsync(CancellationToken ct)
    {
        var files = await _fileStorage.GetFilesAsync(ct);

        foreach (var f in files)
        {
            f.DownloadUrl = Url.Action(nameof(Download), new { id = f.Id }) ?? string.Empty;
        }

        return ApiOk(files, "Files retrieved");
    }

    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadFile([FromForm] IFormFile file, CancellationToken ct)
    {
        try
        {
            var meta = await _fileStorage.SaveAsync(file, ct);
            return ApiCreated(
                nameof(Download),
                new { id = meta.Id },"File uploaded successfully"
            );
        }
        catch (Exception ex) when (ex is InvalidOperationException)
        {
            return BadRequest(ex.Message);
        }
    }

    // GET api/file/{id}
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Download([FromRoute] Guid id, CancellationToken ct)
    {
        try
        {
            var (meta, stream) = await _fileStorage.OpenReadAsync(id, ct);

            // "enableRangeProcessing" utile pour gros fichiers (resume/seek)
            return File(stream, meta.ContentType, meta.OriginalName, enableRangeProcessing: true);
        }
        catch (KeyNotFoundException)
        {
            return ApiNotFound("File not found.");
        }
        catch (FileNotFoundException)
        {
            return ApiNotFound("File missing on disk.");
        }
    }

    // DELETE api/file/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid id, CancellationToken ct)
    {
        try
        {
            await _fileStorage.DeleteAsync(id, ct);
            return ApiOk(true, "File deleted");
        }
        catch (KeyNotFoundException)
        {
            return ApiNotFound("File not found.");
        }
    }
}
