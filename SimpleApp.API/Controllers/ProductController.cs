using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimpleApp.Application.Queries.Products;

namespace SimpleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> GetPagedProductsAsync([FromBody] GetPagedProductsQuery query)
    {
        var products = await _mediator.Send(query);
        return Ok(products);
    }
}