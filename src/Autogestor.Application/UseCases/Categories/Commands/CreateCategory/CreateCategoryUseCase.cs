using Autogestor.Application.Interfaces;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;

namespace Autogestor.Application.UseCases.Categories.Commands.CreateCategory;

public sealed class CreateCategoryUseCase(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : ICreateCategoryUseCase
{
    public async Task<CategoryResponse> ExecuteAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var category = Category.Create(
            title: request.Title,
            description: request.Description);

        categoryRepository.Add(category: category);
        await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

        return new CategoryResponse
        {
            Id = category.Id,
            Active = category.Active,
            CreatedBy = category.CreatedBy,
            CreatedAt = category.CreatedAt,
            UpdatedBy = category.UpdatedBy,
            UpdatedAt = category.UpdatedAt,
            TenantId = category.TenantId,
            Title = category.Title,
            Description = category.Description
        };
    }
}
