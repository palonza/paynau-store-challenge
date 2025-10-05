
namespace Paynau.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Paynau.Application.Commands;
using Paynau.Application.DTOs;
using Paynau.Application.Queries;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all orders
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetAll()
    {
        _logger.LogInformation("Getting all orders");
        var orders = await _mediator.Send(new GetAllOrdersQuery());
        
        if (!orders.Any())
        {
            _logger.LogInformation("No orders found");
            return NoContent();
        }

        return Ok(orders);
    }

    /// <summary>
    /// Create a new order
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderDto>> Create([FromBody] CreateOrderDto dto)
    {
        _logger.LogInformation("Creating order for product {ProductId}", dto.ProductId);
        
        var command = new CreateOrderCommand(dto.ProductId, dto.Quantity);
        var order = await _mediator.Send(command);
        
        _logger.LogInformation("Order {OrderId} created successfully", order.Id);
        return CreatedAtAction(nameof(GetAll), new { id = order.Id }, order);
    }
}

