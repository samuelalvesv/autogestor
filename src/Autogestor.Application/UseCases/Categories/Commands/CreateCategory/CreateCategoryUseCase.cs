using Autogestor.Application.Interfaces;
using Autogestor.Application.Mappers;
using Autogestor.Application.Validators;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;
using FluentValidation;

namespace Autogestor.Application.UseCases.Categories.Commands.CreateCategory;

public sealed class CreateCategoryUseCase(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreateCategoryRequest> validator) : ICreateCategoryUseCase
{
    public async Task<CategoryResponse> ExecuteAsync(
        CreateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateOrThrowAsync(instance: request, cancellationToken: cancellationToken);

        var category = Category.Create(
            title: request.Title,
            description: request.Description);

        categoryRepository.Add(category: category);
        await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

        return CategoryMapper.ToResponse(category: category);
    }
}
