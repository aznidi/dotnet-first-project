using FIRST.Shared;
using Microsoft.AspNetCore.Mvc;

namespace FIRST.Controllers;

[ApiController]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult ApiOk<T>(T data, string message = "OK")
        => Ok(ApiResponse<T>.Ok(data, message));

    protected IActionResult ApiCreated<T>(string location, T data, string message = "Created")
        => Created(location, ApiResponse<T>.Ok(data, message));

    protected IActionResult ApiNotFound(string message)
        => NotFound(ApiResponse<object>.Fail(message));

    protected IActionResult ApiBadRequest(string message, object? errors = null)
        => BadRequest(ApiResponse<object>.Fail(message, errors));
}
