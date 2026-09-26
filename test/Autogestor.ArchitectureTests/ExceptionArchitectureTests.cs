using System.Reflection;
using Autogestor.Domain.Exceptions;
using NetArchTest.Rules;
using TestResult = NetArchTest.Rules.TestResult;

namespace Autogestor.ArchitectureTests;

public sealed class ExceptionArchitectureTests
{
    private static readonly Assembly DomainAssembly = typeof(Domain.Entities.Entity).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(Application.Mappers.CategoryMapper).Assembly;

    [Fact]
    public void CustomExceptions_ShouldInheritFromDomainException()
    {
        TestResult result = Types.InAssemblies(assemblies: [DomainAssembly, ApplicationAssembly])
            .That()
            .HaveNameEndingWith(end: "Exception")
            .And()
            .DoNotHaveName(name: "DomainException")
            .Should()
            .Inherit(type: typeof(DomainException))
            .GetResult();

        Assert.True(
            condition: result.IsSuccessful,
            userMessage: "Todas as exceções em Domain e Application devem herdar de DomainException para garantir o mapeamento no GrpcExceptionInterceptor.");
    }

    [Fact]
    public void AllExceptionsInDomainAndApplication_ShouldInheritFromDomainException()
    {
        TestResult result = Types.InAssemblies(assemblies: [DomainAssembly, ApplicationAssembly])
            .That()
            .Inherit(type: typeof(Exception))
            .And()
            .DoNotHaveName(name: "DomainException")
            .Should()
            .Inherit(type: typeof(DomainException))
            .GetResult();

        Assert.True(
            condition: result.IsSuccessful,
            userMessage: "Todas as classes derivadas de Exception em Domain e Application devem herdar de DomainException.");
    }

    [Fact]
    public void DomainExceptions_ShouldHaveNameEndingWithException()
    {
        TestResult result = Types.InAssemblies(assemblies: [DomainAssembly, ApplicationAssembly])
            .That()
            .Inherit(type: typeof(DomainException))
            .Should()
            .HaveNameEndingWith(end: "Exception")
            .GetResult();

        Assert.True(
            condition: result.IsSuccessful,
            userMessage: "Todas as exceções de domínio devem ter o sufixo 'Exception'.");
    }

    [Fact]
    public void SpecializedDomainExceptions_ShouldBeSealed()
    {
        TestResult result = Types.InAssembly(assembly: DomainAssembly)
            .That()
            .Inherit(type: typeof(DomainException))
            .And()
            .DoNotHaveName(name: "DomainException")
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
            .Where(predicate: static t => t.IsSubclassOf(c: typeof(DomainException))
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
            .Select(selector: static t => t.Name)
            .ToList();

        // Assert
        Assert.True(
            condition: unmappedTypes.Count == 0,
            userMessage: $"As seguintes exceções de domínio não estão mapeadas no GrpcExceptionInterceptor: {string.Join(separator: ", ", values: unmappedTypes)}. Adicione o mapeamento no pattern matching do interceptor.");
    }
}
