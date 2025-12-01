using MediatR;
using SimpleApp.Application.Interfaces;
using SimpleApp.Domain.Entities;

namespace SimpleApp.Application.Commands.Products;

public class CreateProductCommand : IRequest<bool>
{
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int Quantity { get; init; }
    public int Stock { get; init; }
    public Stream ImageStream { get; set; }
    public string ImageFileName { get; set; } = string.Empty;
    public Guid CategoryId { get; init; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, bool>
{
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IBlobStorageService _blobStorageService;
    public CreateProductCommandHandler(IGenericRepository<Product> productRepository, IBlobStorageService blobStorageService)
    {
        _productRepository = productRepository;
        _blobStorageService = blobStorageService;
    }
    
    public async Task<bool> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        string uploadedBlobName = string.Empty;
        try
        {
            var imageUrl = string.Empty;
            if (request.ImageStream != null && !string.IsNullOrEmpty(request.ImageFileName))
            {
                imageUrl = await _blobStorageService.UploadImageAsync(request.ImageStream, request.ImageFileName);
                // Extract blob name from URL for cleanup on error
                uploadedBlobName = imageUrl.Split('/').Last();
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Price = request.Price,
                Quantity = request.Quantity,
                Stock = request.Stock,
                CategoryId = request.CategoryId,
                ImageUrl = imageUrl,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception (you can use any logging framework you prefer)
            Console.WriteLine($"Error creating product: {ex.Message}");
            
            // Delete the uploaded image if an error occurred during product creation
            if (!string.IsNullOrEmpty(uploadedBlobName))
            {
                await _blobStorageService.DeleteImageAsync(uploadedBlobName);
            }
            
            return false;
        }        
    }
}