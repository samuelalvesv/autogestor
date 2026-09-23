using Autogestor.Application.UseCases.Transactions.Commands.CreateTransaction;
using Autogestor.Application.UseCases.Transactions.Commands.DeleteTransaction;
using Autogestor.Application.UseCases.Transactions.Commands.UpdateTransaction;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Contract.Services;

namespace Autogestor.Api.Services;

public sealed class TransactionService(
    ICreateTransactionUseCase createTransactionUseCase,
    IUpdateTransactionUseCase updateTransactionUseCase,
    IDeleteTransactionUseCase deleteTransactionUseCase) : ITransactionService
{
    public Task<Response<TransactionResponse>> CreateAsync(
        CreateTransactionRequest request,
        CancellationToken cancellationToken = default) =>
        createTransactionUseCase.ExecuteAsync(
            request: request,
            cancellationToken: cancellationToken);

    public Task<Response<DeleteResponse>> DeleteAsync(
        DeleteTransactionRequest request,
        CancellationToken cancellationToken = default) =>
        deleteTransactionUseCase.ExecuteAsync(
            request: request,
            cancellationToken: cancellationToken);

    public Task<PagedResponse<TransactionResponse>> GetAllAsync(
        GetAllTransactionsRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(result: new PagedResponse<TransactionResponse>
        {
            Data = [],
            Message = "Implementação pendente.",
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        });

    public Task<Response<TransactionResponse>> GetByIdAsync(
        GetTransactionByIdRequest request,
        CancellationToken cancellationToken = default) =>
        Task.FromResult(result: new Response<TransactionResponse>
        {
            Data = null,
            Message = "Implementação pendente."
        });

    public Task<Response<TransactionResponse>> UpdateAsync(
        UpdateTransactionRequest request,
        CancellationToken cancellationToken = default) =>
        updateTransactionUseCase.ExecuteAsync(
            request: request,
            cancellationToken: cancellationToken);
}
