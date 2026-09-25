using Autogestor.Application.Interfaces;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.Domain.Interfaces;

namespace Autogestor.Application.UseCases.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryUseCase(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IUpdateCategoryUseCase
{
    public async Task<CategoryResponse> ExecuteAsync(
        UpdateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        Category? category = await categoryRepository.GetByIdAsync(
            id: request.Id,
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException(message: "Categoria não encontrada.");

        category.Update(
            title: request.Title,
            description: request.Description);

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
