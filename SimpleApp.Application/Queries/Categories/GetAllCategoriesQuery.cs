using MediatR;
using SimpleApp.Application.DTOs.Category;
using SimpleApp.Application.Interfaces;
using SimpleApp.Domain.Entities;

namespace SimpleApp.Application.Queries.Categories;

public record GetAllCategoriesQuery : IRequest<IEnumerable<CategoryDto>>;

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, IEnumerable<CategoryDto>>
{
    private readonly IGenericRepository<Category> _categoryRepository;

    public GetAllCategoriesQueryHandler(IGenericRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        return await _categoryRepository.GetAllAsync(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name
        });
    }
}