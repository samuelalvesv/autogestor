using Autogestor.Application.Interfaces;
using Autogestor.Application.UseCases.Transactions.Commands.CreateTransaction;
using Autogestor.Contract.Enums;
using Autogestor.Contract.Requests.Transactions;
using Autogestor.Contract.Responses;
using Autogestor.Contract.Responses.Transactions;
using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;
using DomainTransactionType = Autogestor.Domain.Enums.ETransactionType;

namespace Autogestor.UnitTests.Application.UseCases.Transactions.Commands;

public sealed class CreateTransactionUseCaseTests
{
    private static void SetPersistenceFields(TenantEntity entity, Guid userId, Guid tenantId, DateTime timestamp)
    {
        typeof(AuditableEntity).GetProperty(name: nameof(AuditableEntity.CreatedBy))!
            .SetValue(obj: entity, value: userId);
        typeof(AuditableEntity).GetProperty(name: nameof(AuditableEntity.CreatedAt))!
            .SetValue(obj: entity, value: timestamp);
        typeof(TenantEntity).GetProperty(name: nameof(TenantEntity.TenantId))!
            .SetValue(obj: entity, value: tenantId);
    }

    private sealed class TransactionRepositoryFake : ITransactionRepository
    {
        public List<Transaction> Transactions { get; } = [];
        public CancellationToken PassedCancellationToken { get; private set; }

        public Task CreateAsync(Transaction transaction, CancellationToken cancellationToken = default)
        {
            PassedCancellationToken = cancellationToken;
            SetPersistenceFields(
                entity: transaction,
                userId: Guid.NewGuid(),
                tenantId: Guid.NewGuid(),
                timestamp: DateTime.UtcNow);
            Transactions.Add(item: transaction);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Transaction transaction, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(result: Transactions.FirstOrDefault(predicate: t => t.Id == id));

        public Task<IReadOnlyList<Transaction>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Transaction>>(result: Transactions.Skip(count: (pageNumber - 1) * pageSize).Take(count: pageSize).ToList().AsReadOnly());
    }

    private sealed class CategoryRepositoryFake : ICategoryRepository
    {
        public List<Category> Categories { get; } = [];
        public CancellationToken PassedCancellationToken { get; private set; }

        public Task CreateAsync(Category category, CancellationToken cancellationToken = default)
        {
            PassedCancellationToken = cancellationToken;
            Categories.Add(item: category);
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Category category, CancellationToken cancellationToken = default)
            => throw new NotSupportedException();

        public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            PassedCancellationToken = cancellationToken;
            return Task.FromResult(result: Categories.FirstOrDefault(predicate: c => c.Id == id));
        }

        public Task<IReadOnlyList<Category>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<Category>>(result: Categories.Skip(count: (pageNumber - 1) * pageSize).Take(count: pageSize).ToList().AsReadOnly());
    }

    private sealed class UnitOfWorkFake : IUnitOfWork
    {
        public int CommitCount { get; private set; }
        public CancellationToken PassedCancellationToken { get; private set; }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            PassedCancellationToken = cancellationToken;
            CommitCount++;
            return Task.CompletedTask;
        }
    }

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
            unitOfWork: unitOfWork);

        var category = Category.Create(title: "Serviços", description: "Descrição de serviços");
        await categoryRepository.CreateAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);

        var request = new CreateTransactionRequest
        {
            Title = "Venda de Serviços",
            Type = type,
            Amount = 3500.00m,
            CategoryId = category.Id
        };

        // Act
        Response<TransactionResponse> response = await useCase.ExecuteAsync(
            request: request,
            cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        Assert.NotNull(@object: response);
        Assert.NotNull(@object: response.Data);
        Assert.Equal(expected: "Transação criada com sucesso.", actual: response.Message);
        Assert.Equal(expected: request.Title, actual: response.Data.Title);
        Assert.Equal(expected: request.Type, actual: response.Data.Type);
        Assert.Equal(expected: request.Amount, actual: response.Data.Amount);
        Assert.Equal(expected: request.CategoryId, actual: response.Data.CategoryId);
        Assert.NotEqual(expected: Guid.Empty, actual: response.Data.Id);
        Assert.True(condition: response.Data.Active, userMessage: "A transação deve ser criada como ativa por padrão.");
        Assert.NotEqual(expected: Guid.Empty, actual: response.Data.TenantId);
        Assert.Equal(expected: transactionRepository.Transactions[0].TenantId, actual: response.Data.TenantId);
        Assert.Null(@object: response.Data.UpdatedBy);
        Assert.Null(@object: response.Data.UpdatedAt);

        Assert.Single(collection: transactionRepository.Transactions);
        Assert.Equal(expected: request.Title, actual: transactionRepository.Transactions[0].Title);
        Assert.Equal(expected: (DomainTransactionType)request.Type, actual: transactionRepository.Transactions[0].Type);
        Assert.Equal(expected: request.Amount, actual: transactionRepository.Transactions[0].Amount);
        Assert.Equal(expected: request.CategoryId, actual: transactionRepository.Transactions[0].CategoryId);
        Assert.Equal(expected: 1, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithNonExistentCategoryId_ThrowsArgumentException()
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CreateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork);

        var request = new CreateTransactionRequest
        {
            Title = "Venda de Serviços",
            Type = ETransactionType.Deposit,
            Amount = 3500.00m,
            CategoryId = Guid.NewGuid()
        };

        // Act & Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(expected: "CategoryId", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "Categoria não encontrada para o tenant atual.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Empty(collection: transactionRepository.Transactions);
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
        var useCase = new CreateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork);

        var request = new CreateTransactionRequest
        {
            Title = invalidTitle!,
            Type = ETransactionType.Deposit,
            Amount = 100.00m,
            CategoryId = Guid.NewGuid()
        };

        // Act & Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(expected: "title", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "O título da transação não pode ser vazio.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Empty(collection: transactionRepository.Transactions);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Theory]
    [InlineData(0.00)]
    [InlineData(-1.00)]
    [InlineData(-99.99)]
    public async Task ExecuteAsync_WithZeroOrNegativeAmount_ThrowsArgumentException(double invalidAmountDouble)
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CreateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork);

        var request = new CreateTransactionRequest
        {
            Title = "Título Válido",
            Type = ETransactionType.Withdraw,
            Amount = (decimal)invalidAmountDouble,
            CategoryId = Guid.NewGuid()
        };

        // Act & Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(expected: "amount", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "O valor da transação deve ser maior que zero.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Empty(collection: transactionRepository.Transactions);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }

    [Fact]
    public async Task ExecuteAsync_WithEmptyCategoryId_ThrowsArgumentException()
    {
        // Arrange
        var transactionRepository = new TransactionRepositoryFake();
        var categoryRepository = new CategoryRepositoryFake();
        var unitOfWork = new UnitOfWorkFake();
        var useCase = new CreateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork);

        var request = new CreateTransactionRequest
        {
            Title = "Título Válido",
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
            unitOfWork: unitOfWork);

        var category = Category.Create(title: "Transporte", description: "Descrição de transporte");
        await categoryRepository.CreateAsync(category: category, cancellationToken: TestContext.Current.CancellationToken);

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
        Assert.Equal(expected: token, actual: transactionRepository.PassedCancellationToken);
        Assert.Equal(expected: token, actual: unitOfWork.PassedCancellationToken);
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
        var useCase = new CreateTransactionUseCase(
            transactionRepository: transactionRepository,
            categoryRepository: categoryRepository,
            unitOfWork: unitOfWork);

        var request = new CreateTransactionRequest
        {
            Title = "Título Válido",
            Type = invalidType,
            Amount = 100.00m,
            CategoryId = Guid.NewGuid()
        };

        // Act & Assert
        ArgumentException exception = await Assert.ThrowsAsync<ArgumentException>(
            testCode: () => useCase.ExecuteAsync(
                request: request,
                cancellationToken: TestContext.Current.CancellationToken));
        Assert.Equal(expected: "type", actual: exception.ParamName);
        Assert.Contains(expectedSubstring: "Tipo de transação inválido.", actualString: exception.Message, comparisonType: StringComparison.Ordinal);
        Assert.Empty(collection: transactionRepository.Transactions);
        Assert.Equal(expected: 0, actual: unitOfWork.CommitCount);
    }
}
