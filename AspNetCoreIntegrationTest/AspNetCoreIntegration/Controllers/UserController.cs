using AspNetCoreIntegration.Models.Enum;
using AspNetCoreIntegration.Models.Request;
using AspNetCoreIntegration.Models.Response;
using AspNetCoreIntegration.Services;
using AspNetCoreIntegrationTest.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIntegrationTest.Controllers;

/// <summary>
/// 使用者控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// 新增使用者
    /// </summary>
    /// <returns></returns>
    [HttpPost]
    [Route("")]
    public IActionResult AddUser([FromBody]AddUserRequest request)
    {
        var isValid = _userService.AddUser(request.Name, request.Password, request.Roles);
        if (!isValid)
        {
            return BadRequest(new ApiResponse<object>(ApiResponseStatus.AddUserFail));
        }
        return Ok(new ApiResponse<object>(ApiResponseStatus.Success));
    }
}