using MediatR;
using SimpleApp.Application.Common.Models;
using SimpleApp.Application.DTOs.Product;
using SimpleApp.Application.Interfaces;
using SimpleApp.Domain.Entities;

namespace SimpleApp.Application.Queries.Products;

public class GetPagedProductsQuery() : IRequest<PaginatedResult<ProductDto>>
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetPagedProductsQueryHandler : IRequestHandler<GetPagedProductsQuery, PaginatedResult<ProductDto>>
{
    private readonly IGenericRepository<Product> _productRepository;

    public GetPagedProductsQueryHandler(IGenericRepository<Product> productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PaginatedResult<ProductDto>> Handle(GetPagedProductsQuery request, CancellationToken cancellationToken)
    {
        return await _productRepository.GetPagedAsync(
            filter: null,
            selector: p => new ProductDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                Quantity = p.Quantity,
                Stock = p.Stock,
                ImageUrl = p.ImageUrl,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty
            },
            pageNumber: request.PageNumber,
            pageSize: request.PageSize);
    }
}