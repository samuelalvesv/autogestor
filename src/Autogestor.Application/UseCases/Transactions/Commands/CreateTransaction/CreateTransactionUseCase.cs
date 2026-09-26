using Autogestor.Application.Interfaces;
using Autogestor.Application.Mappers;
using Autogestor.Application.Validators;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.Domain.Interfaces;
using FluentValidation;

namespace Autogestor.Application.UseCases.Transactions.Commands.CreateTransaction;

public sealed class CreateTransactionUseCase(
    ITransactionRepository transactionRepository,
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork,
    IValidator<CreateTransactionRequest> validator) : ICreateTransactionUseCase
{
    public async Task<TransactionResponse> ExecuteAsync(
        CreateTransactionRequest request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateOrThrowAsync(instance: request, cancellationToken: cancellationToken);

        bool categoryExists = await categoryRepository.ExistsAsync(
            id: request.CategoryId,
            cancellationToken: cancellationToken);

        if (!categoryExists)
            throw new NotFoundException(message: "Categoria não encontrada para o tenant atual.");

        var transaction = Transaction.Create(
            title: request.Title,
            type: TransactionMapper.ToDomain(type: request.Type),
            amount: request.Amount,
            categoryId: request.CategoryId);

        transactionRepository.Add(transaction: transaction);
        await unitOfWork.CommitAsync(cancellationToken: cancellationToken);

        return TransactionMapper.ToResponse(transaction: transaction);
    }
}
