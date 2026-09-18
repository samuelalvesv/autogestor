using Autogestor.Application.UseCases.Transactions.Commands.UpdateTransaction;
using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.UnitTests.Common.Fakes;
using DomainTransactionType = Autogestor.Domain.Enums.ETransactionType;

namespace Autogestor.UnitTests.Application.UseCases.Transactions.Commands;

public sealed class UpdateTransactionUseCaseTests
{
    [Theory]
    [InlineData(ETransactionType.Deposit)]
    [InlineData(ETransactionType.Withdraw)]
    public async Task ExecuteAsync_WithValidRequest_ReturnsSuccessResponseAndUpdatesTransaction(ETransactionType newType)
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork);

        var initialCategory = Category.Create(title: "Serviços", description: "Serviços prestados");
        await categoryRepository.AddAsync(category: initialCategory, cancellationToken: TestContext.Current.CancellationToken);

        var targetCategory = Category.Create(title: "Consultoria", description: "Serviços de consultoria");
        await categoryRepository.AddAsync(category: targetCategory, cancellationToken: TestContext.Current.CancellationToken);

        var transaction = Transaction.Create(
            title: "Serviço A",
            type: DomainTransactionType.Deposit,
            amount: 500.00m,
            categoryId: initialCategory.Id);
        await transactionRepository.AddAsync(transaction: transaction, cancellationToken: TestContext.Current.CancellationToken);

        var request = new UpdateTransactionRequest
        {
            Id = transaction.Id,
            Title = "Consultoria Especializada",
            Type = newType,
            Amount = 1500.00m,
            CategoryId = targetCategory.Id
        };

        // Act
        Response<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Equal(expected: "Transação atualizada com sucesso.", actual: response.Message);
        Assert.Equal(expected: request.Id, actual: response.Data.Id);
        Assert.Equal(expected: request.Title, actual: response.Data.Title);
        Assert.Equal(expected: request.Type, actual: response.Data.Type);
        Assert.Equal(expected: request.Amount, actual: response.Data.Amount);
        Assert.Equal(expected: request.CategoryId, actual: response.Data.CategoryId);
        Assert.True(condition: response.Data.Active, userMessage: "A transação deve permanecer ativa após atualização.");
        Assert.Equal(expected: transaction.TenantId, actual: response.Data.TenantId);

        Assert.Single(collection: transactionRepository.Transactions);
        Assert.Equal(expected: request.Title, actual: transactionRepository.Transactions[0].Title);
        Assert.Equal(expected: (DomainTransactionType)request.Type, actual: transactionRepository.Transactions[0].Type);
        Assert.Equal(expected: request.Amount, actual: transactionRepository.Transactions[0].Amount);
        Assert.Equal(expected: request.CategoryId, actual: transactionRepository.Transactions[0].CategoryId);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTransactionNotFound_ReturnsFailureResponse()
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork);

        var request = new UpdateTransactionRequest
        {
            Id = Guid.NewGuid(),
            Title = "Transação Inexistente",
            Type = ETransactionType.Deposit,
            Amount = 100.00m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        Response<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Null(@object: response.Data);
        Assert.Equal(expected: "Transação não encontrada.", actual: response.Message);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryNotFound_ReturnsFailureResponse()
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork);

        var category = Category.Create(title: "Operacional", description: "Despesas operacionais");
        await categoryRepository.AddAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);

        var transaction = Transaction.Create(
            title: "Material de Escritório",
            type: DomainTransactionType.Withdraw,
            amount: 80.00m,
            categoryId: category.Id);
        await transactionRepository.AddAsync(transaction: transaction, cancellationToken: TestContext.Current.CancellationToken);

        var request = new UpdateTransactionRequest
        {
            Id = transaction.Id,
            Title = "Material Atualizado",
            Type = ETransactionType.Withdraw,
            Amount = 120.00m,
            CategoryId = Guid.NewGuid()
        };

        // Act
        Response<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Null(@object: response.Data);
        Assert.Equal(expected: "Categoria não encontrada para o tenant atual.", actual: response.Message);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public async Task ExecuteAsync_WithInvalidTitle_ThrowsArgumentException(string? invalidTitle)
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork);

        var category = Category.Create(title: "Alimentação", description: "Gastos com comida");
        await categoryRepository.AddAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);

        var transaction = Transaction.Create(
            title: "Almoço",
            type: DomainTransactionType.Withdraw,
            amount: 45.00m,
            categoryId: category.Id);
        await transactionRepository.AddAsync(transaction: transaction, cancellationToken: TestContext.Current.CancellationToken);

        var request = new UpdateTransactionRequest
        {
            Id = transaction.Id,
            Title = invalidTitle!,
            Type = ETransactionType.Withdraw,
            Amount = 50.00m,
            CategoryId = category.Id
        };

        // Act & Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(expected: "title", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "O título da transação não pode ser vazio.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(-1.00)]
    [InlineData(-50.00)]
    public async Task ExecuteAsync_WithZeroOrNegativeAmount_ThrowsArgumentException(double invalidAmountDouble)
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork);

        var category = Category.Create(title: "Transporte", description: "Combustível");
        await categoryRepository.AddAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);

        var transaction = Transaction.Create(
            title: "Gasolina",
            type: DomainTransactionType.Withdraw,
            amount: 200.00m,
            categoryId: category.Id);
        await transactionRepository.AddAsync(transaction: transaction, cancellationToken: TestContext.Current.CancellationToken);

        var request = new UpdateTransactionRequest
        {
            Id = transaction.Id,
            Title = "Gasolina Comum",
            Type = ETransactionType.Withdraw,
            Amount = (decimal)invalidAmountDouble,
            CategoryId = category.Id
        };

        // Act & Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(expected: "amount", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "O valor da transação deve ser maior que zero.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Theory]
    [InlineData((ETransactionType)0)]
    [InlineData((ETransactionType)99)]
    [InlineData((ETransactionType)(-1))]
    public async Task ExecuteAsync_WithInvalidType_ThrowsArgumentException(ETransactionType invalidType)
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork);

        var category = Category.Create(title: "Lazer", description: "Cinema e viagens");
        await categoryRepository.AddAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);

        var transaction = Transaction.Create(
            title: "Cinema",
            type: DomainTransactionType.Withdraw,
            amount: 60.00m,
            categoryId: category.Id);
        await transactionRepository.AddAsync(transaction: transaction, cancellationToken: TestContext.Current.CancellationToken);

        var request = new UpdateTransactionRequest
        {
            Id = transaction.Id,
            Title = "Cinema IMAX",
            Type = invalidType,
            Amount = 90.00m,
            CategoryId = category.Id
        };

        // Act & Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(expected: "type", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "Tipo de transação inválido.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithCancellationToken_PropagatesTokenToDependencies()
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork);

        var category = Category.Create(title: "Salário", description: "Rendimentos mensais");
        await categoryRepository.AddAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);

        var transaction = Transaction.Create(
            title: "Adiantamento",
            type: DomainTransactionType.Deposit,
            amount: 1000.00m,
            categoryId: category.Id);
        await transactionRepository.AddAsync(transaction: transaction, cancellationToken: TestContext.Current.CancellationToken);

        var request = new UpdateTransactionRequest
        {
            Id = transaction.Id,
            Title = "Salário Integral",
            Type = ETransactionType.Deposit,
            Amount = 5000.00m,
            CategoryId = category.Id
        };

        using var cts = new CancellationTokenSource();
        CancellationToken token = cts.Token;

        // Act
        await useCase.ExecuteAsync(request: request, cancellationToken: token);

        // Assert
        Assert.Equal(expected: token, actual: transactionRepository.PassedCancellationToken);
        Assert.Equal(expected: token, actual: categoryRepository.PassedCancellationToken);
        Assert.Equal(expected: token, actual: unitOfWork.PassedCancellationToken);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyCategoryId_ThrowsArgumentException()
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new UpdateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork);

        var category = Category.Create(title: "Operacional", description: "Custos operacionais");
        await categoryRepository.AddAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);

        var transaction = Transaction.Create(
            title: "Material de Escritório",
            type: DomainTransactionType.Withdraw,
            amount: 80.00m,
            categoryId: category.Id);
        await transactionRepository.AddAsync(transaction: transaction, cancellationToken: TestContext.Current.CancellationToken);

        var request = new UpdateTransactionRequest
        {
            Id = transaction.Id,
            Title = "Material Atualizado",
            Type = ETransactionType.Withdraw,
            Amount = 100.00m,
            CategoryId = Guid.Empty
        };

        // Act & Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(expected: "categoryId", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "Categoria inválida.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }
}
