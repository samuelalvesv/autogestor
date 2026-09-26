using Autogestor.Application.Mappers;
using Autogestor.Application.Validators;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.Domain.Interfaces;
using FluentValidation;

namespace Autogestor.Application.UseCases.Transactions.Queries.GetTransactionById;

public sealed class GetTransactionByIdUseCase(
    ITransactionRepository transactionRepository,
    IValidator<GetTransactionByIdRequest> validator) : IGetTransactionByIdUseCase
{
    public async Task<TransactionResponse> ExecuteAsync(
        GetTransactionByIdRequest request,
        CancellationToken cancellationToken = default)
    {
        await validator.ValidateOrThrowAsync(instance: request, cancellationToken: cancellationToken);

        Transaction? transaction = await transactionRepository.GetByIdAsync(
            id: request.Id,
            asNoTracking: true,
            cancellationToken: cancellationToken)
            ?? throw new NotFoundException(message: "Transação não encontrada.");

        return TransactionMapper.ToResponse(transaction: transaction);
    }
}
