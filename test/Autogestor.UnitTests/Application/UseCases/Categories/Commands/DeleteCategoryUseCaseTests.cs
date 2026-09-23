using Autogestor.Application.UseCases.Categories.Commands.DeleteCategory;
using Autogestor.Contract.Requests.Categories;
using Autogestor.Contract.Responses;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Enums;
using Autogestor.UnitTests.Common.Fakes;

namespace Autogestor.UnitTests.Application.UseCases.Categories.Commands;

public sealed class DeleteCategoryUseCaseTests
{
    [Fact]
    public async Task ExecuteAsync_WithValidRequest_ReturnsSuccessResponseAndRemovesCategory()
    {
        // Arrange
        var categoryRepository = new CategoryRepositoryFake();
        var transactionRepository = new TransactionRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new DeleteCategoryUseCase(
            categoryRepository: categoryRepository,
            transactionRepository: transactionRepository,
            unitOfWork: unitOfWork);

        var existingCategory = Category.Create(
            title: "Alimentação",
            description: "Despesas com mercados");
        await categoryRepository.AddAsync(
            category: existingCategory,
            cancellationToken: TestContext.Current.CancellationToken);

        var request = new DeleteCategoryRequest
        {
            Id = existingCategory.Id
        };

        // Act
        Response<DeleteResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Equal(expected: "Categoria excluída com sucesso.", actual: response.Message);
        Assert.Equal(expected: existingCategory.Id, actual: response.Data.Id);

        Assert.Empty(collection: categoryRepository.Categories);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryNotFound_ReturnsFailureResponse()
    {
        // Arrange
        var categoryRepository = new CategoryRepositoryFake();
        var transactionRepository = new TransactionRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new DeleteCategoryUseCase(
            categoryRepository: categoryRepository,
            transactionRepository: transactionRepository,
            unitOfWork: unitOfWork);

        var request = new DeleteCategoryRequest
        {
            Id = Guid.NewGuid()
        };

        // Act
        Response<DeleteResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Null(@object: response.Data);
        Assert.Equal(expected: "Categoria não encontrada.", actual: response.Message);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
        Assert.True(condition: transactionRepository.PassedCancellationToken == default, userMessage: "O repositório de transações não deve ser consultado quando a categoria não for encontrada.");
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryHasLinkedTransactions_ReturnsFailureResponse()
    {
        // Arrange
        var categoryRepository = new CategoryRepositoryFake();
        var transactionRepository = new TransactionRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new DeleteCategoryUseCase(
            categoryRepository: categoryRepository,
            transactionRepository: transactionRepository,
            unitOfWork: unitOfWork);

        var existingCategory = Category.Create(
            title: "Transporte",
            description: "Combustível e manutenção");
        await categoryRepository.AddAsync(
            category: existingCategory,
            cancellationToken: TestContext.Current.CancellationToken);

        var linkedTransaction = Transaction.Create(
            title: "Gasolina",
            type: ETransactionType.Withdraw,
            amount: 250.00m,
            categoryId: existingCategory.Id);
        await transactionRepository.AddAsync(
            transaction: linkedTransaction,
            cancellationToken: TestContext.Current.CancellationToken);

        var request = new DeleteCategoryRequest
        {
            Id = existingCategory.Id
        };

        // Act
        Response<DeleteResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Null(@object: response.Data);
        Assert.Equal(expected: "Não é possível excluir uma categoria que possui transações vinculadas.", actual: response.Message);
        Assert.Single(collection: categoryRepository.Categories);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenOtherCategoriesHaveTransactions_RemovesTargetCategorySuccessfully()
    {
        // Arrange
        var categoryRepository = new CategoryRepositoryFake();
        var transactionRepository = new TransactionRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new DeleteCategoryUseCase(
            categoryRepository: categoryRepository,
            transactionRepository: transactionRepository,
            unitOfWork: unitOfWork);

        var targetCategory = Category.Create(
            title: "Categoria Alvo",
            description: "Sem transações vinculadas");
        var otherCategory = Category.Create(
            title: "Outra Categoria",
            description: "Com transações vinculadas");

        await categoryRepository.AddAsync(
            category: targetCategory,
            cancellationToken: TestContext.Current.CancellationToken);
        await categoryRepository.AddAsync(
            category: otherCategory,
            cancellationToken: TestContext.Current.CancellationToken);

        var otherTransaction = Transaction.Create(
            title: "Despesa da Outra Categoria",
            type: ETransactionType.Withdraw,
            amount: 75.00m,
            categoryId: otherCategory.Id);
        await transactionRepository.AddAsync(
            transaction: otherTransaction,
            cancellationToken: TestContext.Current.CancellationToken);

        var request = new DeleteCategoryRequest
        {
            Id = targetCategory.Id
        };

        // Act
        Response<DeleteResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Equal(expected: "Categoria excluída com sucesso.", actual: response.Message);
        Assert.Equal(expected: targetCategory.Id, actual: response.Data.Id);

        Assert.Single(collection: categoryRepository.Categories);
        Assert.Equal(expected: otherCategory.Id, actual: categoryRepository.Categories[0].Id);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenExecutedTwice_ReturnsNotFoundOnSecondCall()
    {
        // Arrange
        var categoryRepository = new CategoryRepositoryFake();
        var transactionRepository = new TransactionRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new DeleteCategoryUseCase(
            categoryRepository: categoryRepository,
            transactionRepository: transactionRepository,
            unitOfWork: unitOfWork);

        var existingCategory = Category.Create(
            title: "Assinaturas",
            description: "Serviços mensais");
        await categoryRepository.AddAsync(
            category: existingCategory,
            cancellationToken: TestContext.Current.CancellationToken);

        var request = new DeleteCategoryRequest
        {
            Id = existingCategory.Id
        };

        // Act 1
        Response<DeleteResponse> firstResponse = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Act 2
        Response<DeleteResponse> secondResponse = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: firstResponse.Data);
        Assert.Equal(expected: "Categoria excluída com sucesso.", actual: firstResponse.Message);
        Assert.Null(@object: secondResponse.Data);
        Assert.Equal(expected: "Categoria não encontrada.", actual: secondResponse.Message);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
        Assert.Empty(collection: categoryRepository.Categories);
    }

    [Fact]
    public async Task ExecuteAsync_WithCancellationToken_PropagatesTokenToDependencies()
    {
        // Arrange
        var categoryRepository = new CategoryRepositoryFake();
        var transactionRepository = new TransactionRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new DeleteCategoryUseCase(
            categoryRepository: categoryRepository,
            transactionRepository: transactionRepository,
            unitOfWork: unitOfWork);

        var existingCategory = Category.Create(
            title: "Lazer",
            description: "Cinema e viagens");
        await categoryRepository.AddAsync(
            category: existingCategory,
            cancellationToken: TestContext.Current.CancellationToken);

        var request = new DeleteCategoryRequest
        {
            Id = existingCategory.Id
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await useCase.ExecuteAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: categoryRepository.PassedCancellationToken);
        Assert.Equal(expected: token, actual: transactionRepository.PassedCancellationToken);
        Assert.Equal(expected: token, actual: unitOfWork.PassedCancellationToken);
    }
}
