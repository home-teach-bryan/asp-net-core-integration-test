using System.Security.Claims;
using AspNetCoreIntegration.Models.Enum;
using AspNetCoreIntegration.Models.Request;
using AspNetCoreIntegration.Models.Response;
using AspNetCoreIntegration.Services;
using AspNetCoreIntegrationTest.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreIntegrationTest.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    /// <summary>
    /// 訂單控制器
    /// </summary>
    /// <param name="orderService"></param>
    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }
    /// <summary>
    /// 成立訂單
    /// </summary>
    /// <returns>回傳執行狀態</returns>
    [HttpPost]
    [Route("")]
    [Authorize(Roles = "User")]
    public IActionResult AddOrder([FromBody] List<AddOrderRequest> addOrderRequest)
    {
        var userId = base.HttpContext.User.Claims.FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return BadRequest(new ApiResponse<object>(ApiResponseStatus.UserNotFound));
        }
        var isSuccess = _orderService.AddOrder(addOrderRequest, Guid.Parse(userId.Value));
        if (!isSuccess)
        {
            return BadRequest(new ApiResponse<object>(ApiResponseStatus.AddOrderFail));
        }

        return Ok(isSuccess);

    }

    /// <summary>
    /// 取得產品詳情
    /// </summary>
    /// <returns></returns>
    [HttpGet]
    [Route("/OrderDetails")]
    [Authorize(Roles = "User")]
    public IActionResult GetOrderDetails()
    {
        var userId = base.HttpContext.User.Claims.FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier);
        if (userId == null)
        {
            return BadRequest(new ApiResponse<object>(ApiResponseStatus.UserNotFound));
        }
        var orderDetails = _orderService.GetOrderDetails(Guid.Parse(userId.Value));
        return Ok(new ApiResponse<IEnumerable<GetOrderDetailsResponse>>(ApiResponseStatus.Success)
        {
            Data = orderDetails
        });
    }
}