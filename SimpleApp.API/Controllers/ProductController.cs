using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using SimpleApp.API.DTOs;
using SimpleApp.Application.Commands.Products;
using SimpleApp.Application.Queries.Products;

namespace SimpleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetPagedProductsAsync([FromQuery] GetPagedProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> CreateProductAsync([FromForm] CreateProductRequest request)
    {
        using var imageStream = request.ImageData?.OpenReadStream();

        var command = new CreateProductCommand
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Quantity = request.Quantity,
            Stock = request.Stock,
            CategoryId = request.CategoryId
        };

        if (imageStream != null)
        {
            command.ImageStream = imageStream;
            command.ImageFileName = request.ImageData!.FileName;
        }

        var result = await _mediator.Send(command);
        if (result)
        {
            return Ok();
        }
        return BadRequest();
    }
}