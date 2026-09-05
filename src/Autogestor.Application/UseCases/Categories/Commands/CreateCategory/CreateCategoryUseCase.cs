using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;

namespace Autogestor.Application.UseCases.Categories.Commands.CreateCategory;

public sealed class CreateCategoryUseCase(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : ICreateCategoryUseCase
{
    public async Task<Response<CategoryResponse>> ExecuteAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        var category = Category.Create(
            title: request.Title,
            description: request.Description,
            userId: request.UserId);

        await categoryRepository.AddAsync(
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
            Title = category.Title,
            Description = category.Description,
            UserId = category.UserId
        };

        return new Response<CategoryResponse>
        {
            Data = response,
            Message = "Categoria criada com sucesso."
        };
    }
}
