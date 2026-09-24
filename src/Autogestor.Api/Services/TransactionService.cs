using Autogestor.Application.UseCases.Transactions.Commands.CreateTransaction;
using Autogestor.Application.UseCases.Transactions.Commands.DeleteTransaction;
using Autogestor.Application.UseCases.Transactions.Commands.UpdateTransaction;
using Autogestor.Application.UseCases.Transactions.Queries.GetAllTransactions;
using Autogestor.Application.UseCases.Transactions.Queries.GetTransactionById;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Contract.Services;

namespace Autogestor.Api.Services;

public sealed class TransactionService(
    ICreateTransactionUseCase createTransactionUseCase,
    IDeleteTransactionUseCase deleteTransactionUseCase,
    IGetAllTransactionsUseCase getAllTransactionsUseCase,
    IGetTransactionByIdUseCase getTransactionByIdUseCase,
    IUpdateTransactionUseCase updateTransactionUseCase) : ITransactionService
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
        getAllTransactionsUseCase.ExecuteAsync(
            request: request,
            cancellationToken: cancellationToken);

    public Task<Response<TransactionResponse>> GetByIdAsync(
        GetTransactionByIdRequest request,
        CancellationToken cancellationToken = default) =>
        getTransactionByIdUseCase.ExecuteAsync(
            request: request,
            cancellationToken: cancellationToken);

    public Task<Response<TransactionResponse>> UpdateAsync(
        UpdateTransactionRequest request,
        CancellationToken cancellationToken = default) =>
        updateTransactionUseCase.ExecuteAsync(
            request: request,
            cancellationToken: cancellationToken);
}
