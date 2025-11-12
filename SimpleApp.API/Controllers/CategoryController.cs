using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimpleApp.Application.Queries.Categories;

namespace SimpleApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoryController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var query = new GetAllCategoriesQuery();
        var categories = await _mediator.Send(query);
        return Ok(categories);
    }
}