using Autogestor.Application.Interfaces;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;

namespace Autogestor.Application.UseCases.Categories.Commands.DeleteCategory;

public sealed class DeleteCategoryUseCase(
    ICategoryRepository categoryRepository,
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork) : IDeleteCategoryUseCase
{
    public async Task<Response<DeleteResponse>> ExecuteAsync(
        DeleteCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        Category? category = await categoryRepository.GetByIdAsync(
            id: request.Id,
            cancellationToken: cancellationToken);

        if (category is null)
            return new Response<DeleteResponse>
            {
                Data = null,
                Message = "Categoria não encontrada."
            };

        bool hasTransactions = await transactionRepository.ExistsByCategoryIdAsync(
            categoryId: request.Id,
            cancellationToken: cancellationToken);

        if (hasTransactions)
            return new Response<DeleteResponse>
            {
                Data = null,
                Message = "Não é possível excluir uma categoria que possui transações vinculadas."
            };

        await categoryRepository.RemoveAsync(category: category, cancellationToken: cancellationToken);

        await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

        return new Response<DeleteResponse>
        {
            Data = new DeleteResponse
            {
                Id = category.Id
            },
            Message = "Categoria excluída com sucesso."
        };
    }
}
