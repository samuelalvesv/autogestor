using Autogestor.Application.Interfaces;
using Autogestor.Application.Mappers;
using Autogestor.Application.Validators;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses.Categories;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.Domain.Interfaces;
using FluentValidation;

namespace Autogestor.Application.UseCases.Categories.Commands.UpdateCategory;

public sealed class UpdateCategoryUseCase(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IValidator<UpdateCategoryRequest> validator) : IUpdateCategoryUseCase
{
    public async Task<CategoryResponse> ExecuteAsync(
        UpdateCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateOrThrowAsync(instance: request, cancellationToken: cancellationToken);

        Category? category = await categoryRepository.GetByIdAsync(
            id: request.Id,
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException(message: "Categoria não encontrada.");

        category.Update(
            title: request.Title,
            description: request.Description);

        await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

        return CategoryMapper.ToResponse(category: category);
    }
}
