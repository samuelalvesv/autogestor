using Autogestor.Application.UseCases.Transactions.Commands.UpdateTransaction;
using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Exceptions;
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
        categoryRepository.Add(category: initialCategory);

        var targetCategory = Category.Create(title: "Consultoria", description: "Serviços de consultoria");
        categoryRepository.Add(category: targetCategory);

        var transaction = Transaction.Create(
            title: "Serviço A",
            type: DomainTransactionType.Deposit,
            amount: 500.00m,
            categoryId: initialCategory.Id);
        transactionRepository.Add(transaction: transaction);

        var request = new UpdateTransactionRequest
        {
            Id = transaction.Id,
            Title = "Consultoria Especializada",
            Type = newType,
            Amount = 1500.00m,
            CategoryId = targetCategory.Id
        };

        // Act
        TransactionResponse response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.Equal(expected: request.Id, actual: response.Id);
        Assert.Equal(expected: request.Title, actual: response.Title);
        Assert.Equal(expected: request.Type, actual: response.Type);
        Assert.Equal(expected: request.Amount, actual: response.Amount);
        Assert.Equal(expected: request.CategoryId, actual: response.CategoryId);
        Assert.True(condition: response.Active, userMessage: "A transação deve permanecer ativa após atualização.");
        Assert.Equal(expected: transaction.TenantId, actual: response.TenantId);

        Assert.Single(collection: transactionRepository.Transactions);
        Assert.Equal(expected: request.Title, actual: transactionRepository.Transactions[0].Title);
        Assert.Equal(expected: (DomainTransactionType)request.Type, actual: transactionRepository.Transactions[0].Type);
        Assert.Equal(expected: request.Amount, actual: transactionRepository.Transactions[0].Amount);
        Assert.Equal(expected: request.CategoryId, actual: transactionRepository.Transactions[0].CategoryId);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
        Assert.Equal(expected: 1, actual: categoryRepository.ExistsCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenTransactionNotFound_ThrowsNotFoundException()
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

        // Act & Assert
        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(expected: "Transação não encontrada.", actual: exception.Message);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
        Assert.Equal(expected: 0, actual: categoryRepository.ExistsCallCount);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryNotFound_ThrowsNotFoundException()
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
        categoryRepository.Add(category: category);

        var transaction = Transaction.Create(
            title: "Material de Escritório",
            type: DomainTransactionType.Withdraw,
            amount: 80.00m,
            categoryId: category.Id);
        transactionRepository.Add(transaction: transaction);

        var request = new UpdateTransactionRequest
        {
            Id = transaction.Id,
            Title = "Material Atualizado",
            Type = ETransactionType.Withdraw,
            Amount = 120.00m,
            CategoryId = Guid.NewGuid()
        };

        // Act & Assert
        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(expected: "Categoria não encontrada para o tenant atual.", actual: exception.Message);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
        Assert.Equal(expected: 1, actual: categoryRepository.ExistsCallCount);
        Assert.Equal(expected: "Material de Escritório", actual: transaction.Title);
        Assert.Equal(expected: DomainTransactionType.Withdraw, actual: transaction.Type);
        Assert.Equal(expected: 80.00m, actual: transaction.Amount);
        Assert.Equal(expected: category.Id, actual: transaction.CategoryId);
    }

    [Fact]
    public async Task ExecuteAsync_WhenCategoryDoesNotExistAndDomainFieldsInvalid_ThrowsNotFoundException()
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
        categoryRepository.Add(category: category);

        var transaction = Transaction.Create(
            title: "Material de Escritório",
            type: DomainTransactionType.Withdraw,
            amount: 80.00m,
            categoryId: category.Id);
        transactionRepository.Add(transaction: transaction);

        var request = new UpdateTransactionRequest
        {
            Id = transaction.Id,
            Title = string.Empty,
            Type = (ETransactionType)999,
            Amount = -100.00m,
            CategoryId = Guid.NewGuid()
        };

        // Act & Assert
        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(expected: "Categoria não encontrada para o tenant atual.", actual: exception.Message);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
        Assert.Equal(expected: 1, actual: categoryRepository.ExistsCallCount);
        Assert.Equal(expected: "Material de Escritório", actual: transaction.Title);
        Assert.Equal(expected: DomainTransactionType.Withdraw, actual: transaction.Type);
        Assert.Equal(expected: 80.00m, actual: transaction.Amount);
        Assert.Equal(expected: category.Id, actual: transaction.CategoryId);
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
        categoryRepository.Add(category: category);

        var transaction = Transaction.Create(
            title: "Almoço",
            type: DomainTransactionType.Withdraw,
            amount: 45.00m,
            categoryId: category.Id);
        transactionRepository.Add(transaction: transaction);

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
        categoryRepository.Add(category: category);

        var transaction = Transaction.Create(
            title: "Gasolina",
            type: DomainTransactionType.Withdraw,
            amount: 200.00m,
            categoryId: category.Id);
        transactionRepository.Add(transaction: transaction);

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
        categoryRepository.Add(category: category);

        var transaction = Transaction.Create(
            title: "Cinema",
            type: DomainTransactionType.Withdraw,
            amount: 60.00m,
            categoryId: category.Id);
        transactionRepository.Add(transaction: transaction);

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
        categoryRepository.Add(category: category);

        var transaction = Transaction.Create(
            title: "Adiantamento",
            type: DomainTransactionType.Deposit,
            amount: 1000.00m,
            categoryId: category.Id);
        transactionRepository.Add(transaction: transaction);

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
    public async Task ExecuteAsync_WithEmptyCategoryId_ThrowsNotFoundException()
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
        categoryRepository.Add(category: category);

        var transaction = Transaction.Create(
            title: "Material de Escritório",
            type: DomainTransactionType.Withdraw,
            amount: 80.00m,
            categoryId: category.Id);
        transactionRepository.Add(transaction: transaction);

        var request = new UpdateTransactionRequest
        {
            Id = transaction.Id,
            Title = "Material Atualizado",
            Type = ETransactionType.Withdraw,
            Amount = 100.00m,
            CategoryId = Guid.Empty
        };

        // Act & Assert
        NotFoundException exception = await Assert.ThrowsAsync<NotFoundException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));

        Assert.Equal(expected: "Categoria não encontrada para o tenant atual.", actual: exception.Message);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }
}
