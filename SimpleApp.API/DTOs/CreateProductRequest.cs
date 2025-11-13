namespace SimpleApp.API.DTOs;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int Stock { get; set; }
    public IFormFile? ImageData { get; set; }
    public Guid CategoryId { get; set; }
}