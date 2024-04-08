using Microsoft.AspNetCore.Mvc;
using TestProject.Models;
using TestProject.Services;

namespace TestProject.Controllers;

[ApiController]
[Route("[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    /// <summary>
    /// Create new order
    /// </summary>
    /// <param name="userId">user</param>
    /// <param name="order">order items</param>
    /// <returns>Is operation completes successfully</returns>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /order
    ///     {
    ///         "order": [
    ///             {
    ///                 "productId": 1,
    ///                 "quantity": 1
    ///             }
    ///         ]
    ///     }
    /// </remarks>
    /// <response code="200">Operation completes successfully</response>
    /// <response code="400">Wrong parameters passed</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
    public async Task<ActionResult> CreateOrder([FromHeader(Name = "Authorize")]string userId, [FromBody]ICollection<NewOrderItem> order)
    {
        CreateOrderResult res = await _orderService.CreateOrder(userId, order);

        if (!res.IsOk) return BadRequest(res.ErrorMessage);

        return Ok();
    }
}