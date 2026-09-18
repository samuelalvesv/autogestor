using Autogestor.Application.Interfaces;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;

namespace Autogestor.Application.UseCases.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryUseCase(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IUpdateCategoryUseCase
{
    public async Task<Response<CategoryResponse>> ExecuteAsync(
        UpdateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        Category? category = await categoryRepository.GetByIdAsync(request.Id, cancellationToken);

        if (category is null)
            return new Response<CategoryResponse>
            {
                Data = null,
                Message = "Categoria não encontrada."
            };

        category.Update(
            title: request.Title,
            description: request.Description);

        await categoryRepository.UpdateAsync(
            category: category,
            cancellationToken: cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

        var response = new CategoryResponse
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

        return new Response<CategoryResponse>
        {
            Data = response,
            Message = "Categoria atualizada com sucesso."
        };
    }
}
