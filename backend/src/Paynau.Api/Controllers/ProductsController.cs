
namespace Paynau.Api.Controllers;

using MediatR;
using Microsoft.AspNetCore.Mvc;
using Paynau.Application.Commands;
using Paynau.Application.DTOs;
using Paynau.Application.Queries;
using Paynau.Domain.Exceptions;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Get all products
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        _logger.LogInformation("Getting all products");
        var products = await _mediator.Send(new GetAllProductsQuery());
        return Ok(products);
    }

    /// <summary>
    /// Get product by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        _logger.LogInformation("Getting product {ProductId}", id);
        var product = await _mediator.Send(new GetProductByIdQuery(id));
        
        if (product == null)
        {
            _logger.LogWarning("Product {ProductId} not found", id);
            return NotFound(new { message = $"Product with ID {id} was not found" });
        }

        return Ok(product);
    }

    /// <summary>
    /// Create a new product
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        _logger.LogInformation("Creating product {ProductName}", dto.Name);
        
        var command = new CreateProductCommand(dto.Name, dto.Description, dto.Price, dto.Stock);
        var product = await _mediator.Send(command);
        
        _logger.LogInformation("Product {ProductId} created successfully", product.Id);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    /// <summary>
    /// Update an existing product
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto dto)
    {
        _logger.LogInformation("Updating product {ProductId}", id);
        
        var command = new UpdateProductCommand(id, dto.Name, dto.Description, dto.Price, dto.Stock);
        var product = await _mediator.Send(command);
        
        _logger.LogInformation("Product {ProductId} updated successfully", id);
        return Ok(product);
    }

    /// <summary>
    /// Delete a product
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Delete(int id)
    {
        _logger.LogInformation("Deleting product {ProductId}", id);
        
        await _mediator.Send(new DeleteProductCommand(id));
        
        _logger.LogInformation("Product {ProductId} deleted successfully", id);
        return NoContent();
    }
}

