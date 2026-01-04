using FIRST.DTOs.Classes.Request;
using FIRST.Services.Classes.Commands;
using FIRST.Services.Classes.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIRST.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ClassesController : BaseApiController
{
    private readonly ClassQueryService _queryService;
    private readonly ClassCommandService _commandService;

    public ClassesController(ClassQueryService classQueryService, ClassCommandService classCommandService)
    {
        _queryService = classQueryService;
        _commandService = classCommandService;
    }


    [HttpGet]
    public async Task<IActionResult> GetClassesAsync()
    {
        var classes =  await _queryService.GetClassesAsync();
        return ApiOk(classes, "Classes retrieved successfully.");
    }

    [HttpPost]
    public async Task<IActionResult> CreateClassAsync([FromBody] CreateClassDto request)
    {
        return ApiOk(
            await _commandService.CreateClassAsync(request), 
            "Class created successfully."
        );
    }

    [HttpGet("{id:min(1)}")]
    public async Task<IActionResult> GetClassByIdAsync(int id)
    {
        var @class = await _queryService.GetClassByIdAsync(id);
        if (@class == null)
        {
            return ApiNotFound("Class not found.");
        }
        return ApiOk(@class, "Class retrieved successfully.");
    }

    [HttpPut("{id:min(1)}")]
    public async Task<IActionResult> UpdateClassAsync(int id, [FromBody] UpdateClassDto request)
    {
        var updatedClass = await _commandService.UpdateClassAsync(id, request);
        if (updatedClass == null)
        {
            return ApiNotFound("Class not found.");
        }
        return ApiOk(updatedClass, "Class updated successfully.");
    }

    [HttpDelete("{id:min(1)}")]
    public async Task<IActionResult> DeleteClassAsync(int id)
    {
        var deleted = await _commandService.DeleteClassAsync(id);
        if (deleted == null)
        {
            return ApiNotFound("Class not found.");
        }
        return ApiNoContent();
    }
}