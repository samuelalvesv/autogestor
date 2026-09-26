using Autogestor.Application.Interfaces;
using Autogestor.Application.Validators;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.Domain.Interfaces;
using FluentValidation;

namespace Autogestor.Application.UseCases.Categories.Commands.DeleteCategory;

public sealed class DeleteCategoryUseCase(
    ICategoryRepository categoryRepository,
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork,
    IValidator<DeleteCategoryRequest> validator) : IDeleteCategoryUseCase
{
    public async Task<DeleteResponse> ExecuteAsync(
        DeleteCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateOrThrowAsync(instance: request, cancellationToken: cancellationToken);

        Category? category = await categoryRepository.GetByIdAsync(
            id: request.Id,
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException(message: "Categoria não encontrada.");

        bool hasTransactions = await transactionRepository.ExistsByCategoryIdAsync(
            categoryId: request.Id,
            cancellationToken: cancellationToken);

        if (hasTransactions)
            throw new BusinessRuleException(message: "Não é possível excluir uma categoria que possui transações vinculadas.");

        categoryRepository.Remove(category: category);
        await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

        return new DeleteResponse
        {
            Id = category.Id
        };
    }
}
