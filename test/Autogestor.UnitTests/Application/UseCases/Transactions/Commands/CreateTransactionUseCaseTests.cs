using Autogestor.Application.UseCases.Transactions.Commands.CreateTransaction;
using Autogestor.Application.Validators.Transactions;
using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
using Autogestor.UnitTests.Common.Fakes;
using DomainTransactionType = Autogestor.Domain.Enums.ETransactionType;

namespace Autogestor.UnitTests.Application.UseCases.Transactions.Commands;

public sealed class CreateTransactionUseCaseTests
{
    private readonly CreateTransactionRequestValidator _validator = new();

    [Theory]
    [InlineData(ETransactionType.Deposit)]
    [InlineData(ETransactionType.Withdraw)]
    public async Task ExecuteAsync_WithValidRequest_ReturnsSuccessResponseAndPersistsTransaction(ETransactionType type)
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CreateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork,
            validator: _validator);

        var category = Category.Create(title: "Serviços", description: "Descrição de serviços");
        categoryRepository.Add(category: category);

        var request = new CreateTransactionRequest
        {
            Title = "Venda de Serviços",
            Type = type,
            Amount = 3500.00m,
            CategoryId = category.Id
        };

        // Act
        TransactionResponse response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Equal(expected: request.Title, actual: response.Title);
        Assert.Equal(expected: request.Type, actual: response.Type);
        Assert.Equal(expected: request.Amount, actual: response.Amount);
        Assert.Equal(expected: request.CategoryId, actual: response.CategoryId);
        Assert.NotEqual(expected: Guid.Empty, actual: response.Id);
        Assert.True(condition: response.Active, userMessage: "A transação deve ser criada como ativa por padrão.");
        Assert.NotEqual(expected: Guid.Empty, actual: response.TenantId);
        Assert.Equal(expected: transactionRepository.Transactions[0].TenantId, actual: response.TenantId);
        Assert.Null(@object: response.UpdatedBy);
        Assert.Null(@object: response.UpdatedAt);

        Assert.Single(collection: transactionRepository.Transactions);
        Assert.Equal(expected: request.Title, actual: transactionRepository.Transactions[0].Title);
        Assert.Equal(expected: (DomainTransactionType)request.Type, actual: transactionRepository.Transactions[0].Type);
        Assert.Equal(expected: request.Amount, actual: transactionRepository.Transactions[0].Amount);
        Assert.Equal(expected: request.CategoryId, actual: transactionRepository.Transactions[0].CategoryId);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
        Assert.Equal(expected: 1, actual: categoryRepository.ExistsCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentCategoryId_ThrowsNotFoundException()
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CreateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork,
            validator: _validator);

        var request = new CreateTransactionRequest
        {
            Title = "Venda de Serviços",
            Type = ETransactionType.Deposit,
            Amount = 3500.00m,
            CategoryId = Guid.NewGuid()
        };

        // Act & Assert
        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(expected: "Categoria não encontrada para o tenant atual.", actual: exception.Message);
        Assert.Empty(collection: transactionRepository.Transactions);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
        Assert.Equal(expected: 1, actual: categoryRepository.ExistsCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryDoesNotExistAndDomainFieldsInvalid_ThrowsDomainValidationException()
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CreateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork,
            validator: _validator);

        var request = new CreateTransactionRequest
        {
            Title = string.Empty,
            Type = (ETransactionType)999,
            Amount = -100.00m,
            CategoryId = Guid.NewGuid()
        };

        // Act & Assert
        DomainValidationException exception = await Assert.ThrowsAsync<DomainValidationException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Contains(expectedSubstring: "O título da transação não pode ser vazio.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Empty(collection: transactionRepository.Transactions);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
        Assert.Equal(expected: 0, actual: categoryRepository.ExistsCallCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ExecuteAsync_WithInvalidTitle_ThrowsDomainValidationException(string? invalidTitle)
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CreateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork,
            validator: _validator);

        var category = Category.Create(title: "Serviços", description: "Descrição de serviços");
        categoryRepository.Add(category: category);

        var request = new CreateTransactionRequest
        {
            Title = invalidTitle!,
            Type = ETransactionType.Deposit,
            Amount = 100.00m,
            CategoryId = category.Id
        };

        // Act & Assert
        DomainValidationException exception = await Assert.ThrowsAsync<DomainValidationException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Contains(expectedSubstring: "O título da transação não pode ser vazio.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Empty(collection: transactionRepository.Transactions);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(-1.00)]
    [InlineData(-99.99)]
    public async Task ExecuteAsync_WithZeroOrNegativeAmount_ThrowsDomainValidationException(double invalidAmountDouble)
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CreateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork,
            validator: _validator);

        var category = Category.Create(title: "Serviços", description: "Descrição de serviços");
        categoryRepository.Add(category: category);

        var request = new CreateTransactionRequest
        {
            Title = "Título Válido",
            Type = ETransactionType.Withdraw,
            Amount = (decimal)invalidAmountDouble,
            CategoryId = category.Id
        };

        // Act & Assert
        DomainValidationException exception = await Assert.ThrowsAsync<DomainValidationException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Contains(expectedSubstring: "O valor da transação deve ser maior que zero.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Empty(collection: transactionRepository.Transactions);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyCategoryId_ThrowsDomainValidationException()
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CreateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork,
            validator: _validator);

        var request = new CreateTransactionRequest
        {
            Title = "Título Válido",
            Type = ETransactionType.Withdraw,
            Amount = 100.00m,
            CategoryId = Guid.Empty
        };

        // Act & Assert
        DomainValidationException exception = await Assert.ThrowsAsync<DomainValidationException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Contains(expectedSubstring: "O identificador da categoria é obrigatório.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Empty(collection: transactionRepository.Transactions);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithCancellationToken_PropagatesTokenToDependencies()
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CreateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork,
            validator: _validator);

        var category = Category.Create(title: "Transporte", description: "Descrição de transporte");
        categoryRepository.Add(category: category);

        var request = new CreateTransactionRequest
        {
            Title = "Transporte",
            Type = ETransactionType.Withdraw,
            Amount = 250.00m,
            CategoryId = category.Id
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await useCase.ExecuteAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: categoryRepository.PassedCancellationToken);
        Assert.Equal(expected: token, actual: unitOfWork.PassedCancellationToken);
    }

    [Theory]
    [InlineData((ETransactionType)0)]
    [InlineData((ETransactionType)99)]
    [InlineData((ETransactionType)(-1))]
    public async Task ExecuteAsync_WithInvalidType_ThrowsDomainValidationException(ETransactionType invalidType)
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CreateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork,
            validator: _validator);

        var category = Category.Create(title: "Serviços", description: "Descrição de serviços");
        categoryRepository.Add(category: category);

        var request = new CreateTransactionRequest
        {
            Title = "Título Válido",
            Type = invalidType,
            Amount = 100.00m,
            CategoryId = category.Id
        };

        // Act & Assert
        DomainValidationException exception = await Assert.ThrowsAsync<DomainValidationException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Contains(expectedSubstring: "Tipo de transação inválido.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Empty(collection: transactionRepository.Transactions);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }
}
