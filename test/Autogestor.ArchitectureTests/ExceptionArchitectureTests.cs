using System.Reflection;
using Autogestor.Domain.Exceptions;
using NetArchTest.Rules;
using TestResult = NetArchTest.Rules.TestResult;

namespace Autogestor.ArchitectureTests;

public sealed class ExceptionArchitectureTests
{
    private static readonly Assembly DomainAssembly = typeof(Domain.Entities.Entity).Assembly;
    private static readonly Assembly ApplicationAssembly = Assembly.Load("Autogestor.Application");

    [Fact]
    public void CustomExceptions_ShouldInheritFromDomainException()
    {
        TestResult result = Types.InAssemblies([DomainAssembly, ApplicationAssembly])
            .That()
            .HaveNameEndingWith("Exception")
            .And()
            .DoNotHaveName("DomainException")
            .Should()
            .Inherit(typeof(DomainException))
            .GetResult();

        Assert.True(
            condition: result.IsSuccessful,
            userMessage: "Todas as exceções em Domain e Application devem herdar de DomainException para garantir o mapeamento no GrpcExceptionInterceptor.");
    }

    [Fact]
    public void AllExceptionsInDomainAndApplication_ShouldInheritFromDomainException()
    {
        TestResult result = Types.InAssemblies([DomainAssembly, ApplicationAssembly])
            .That()
            .Inherit(typeof(Exception))
            .And()
            .DoNotHaveName("DomainException")
            .Should()
            .Inherit(typeof(DomainException))
            .GetResult();

        Assert.True(
            condition: result.IsSuccessful,
            userMessage: "Todas as classes derivadas de Exception em Domain e Application devem herdar de DomainException.");
    }

    [Fact]
    public void DomainExceptions_ShouldHaveNameEndingWithException()
    {
        TestResult result = Types.InAssemblies([DomainAssembly, ApplicationAssembly])
            .That()
            .Inherit(typeof(DomainException))
            .Should()
            .HaveNameEndingWith("Exception")
            .GetResult();

        Assert.True(
            condition: result.IsSuccessful,
            userMessage: "Todas as exceções de domínio devem ter o sufixo 'Exception'.");
    }

    [Fact]
    public void SpecializedDomainExceptions_ShouldBeSealed()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .That()
            .Inherit(typeof(DomainException))
            .And()
            .DoNotHaveName("DomainException")
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(
            condition: result.IsSuccessful,
            userMessage: "Todas as exceções especializadas de domínio devem ser marcadas como sealed.");
    }

    [Fact]
    public void GrpcExceptionInterceptor_ShouldMapAllDomainExceptionSubclasses()
    {
        // Arrange
        IEnumerable<Type> domainExceptionTypes = DomainAssembly
            .GetTypes()
            .Where(predicate: t => t.IsSubclassOf(c: typeof(DomainException))
                                 && !t.IsAbstract);

        string? interceptorSourcePath = Directory.GetFiles(
                path: Path.GetFullPath(path: Path.Combine(path1: AppContext.BaseDirectory, path2: "../../../../../src/Autogestor.Api")),
                searchPattern: "GrpcExceptionInterceptor.cs",
                searchOption: SearchOption.AllDirectories)
            .FirstOrDefault();

        Assert.NotNull(@object: interceptorSourcePath);

        string interceptorSource = File.ReadAllText(path: interceptorSourcePath);

        // Act
        var unmappedTypes = domainExceptionTypes
            .Where(predicate: t => !interceptorSource.Contains(value: t.Name, comparisonType: StringComparison.Ordinal))
            .Select(selector: t => t.Name)
            .ToList();

        // Assert
        Assert.True(
            condition: unmappedTypes.Count == 0,
            userMessage: $"As seguintes exceções de domínio não estão mapeadas no GrpcExceptionInterceptor: {string.Join(separator: ", ", values: unmappedTypes)}. Adicione o mapeamento no pattern matching do interceptor.");
    }
}
