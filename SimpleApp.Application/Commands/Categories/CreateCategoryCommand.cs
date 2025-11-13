using MediatR;
using SimpleApp.Application.Interfaces;
using SimpleApp.Domain.Entities;

namespace SimpleApp.Application.Commands.Categories;

public class CreateCategoryCommand : IRequest<Guid>
{
    public string Name { get; set; } = string.Empty;
}

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Guid>
{
    private readonly IGenericRepository<Category> _categoryRepository;
    public CreateCategoryCommandHandler(IGenericRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<Guid> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
        };

        await _categoryRepository.AddAsync(category);
        await _categoryRepository.SaveChangesAsync();
        return category.Id;
    }
}