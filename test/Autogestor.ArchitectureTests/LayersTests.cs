using System.Reflection;
using NetArchTest.Rules;
using TestResult = NetArchTest.Rules.TestResult;

namespace Autogestor.ArchitectureTests;

public sealed class LayersTests
{
    private const string DomainNamespace = "Autogestor.Domain";
    private const string ContractNamespace = "Autogestor.Contract";
    private const string ApplicationNamespace = "Autogestor.Application";
    private const string InfrastructureNamespace = "Autogestor.Infrastructure";
    private const string ApiNamespace = "Autogestor.Api";
    private const string WebNamespace = "Autogestor.Web";

    private static readonly Assembly DomainAssembly = typeof(Domain.Entities.Entity).Assembly;
    private static readonly Assembly ContractAssembly = typeof(Contract.ContractDefaults).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(Infrastructure.Persistence.AppDbContext).Assembly;
    private static readonly Assembly ApplicationAssembly = Assembly.Load("Autogestor.Application");

    [Fact]
    public void Domain_ShouldNotHaveDependencyOnOtherLayers()
    {
        TestResult result = Types.InAssembly(DomainAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApplicationNamespace, InfrastructureNamespace, ApiNamespace, WebNamespace, ContractNamespace)
            .GetResult();

        Assert.True(condition: result.IsSuccessful, userMessage: "A camada Domain não deve depender de outras camadas.");
    }

    [Fact]
    public void Contract_ShouldNotHaveDependencyOnOtherLayers()
    {
        TestResult result = Types.InAssembly(ContractAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(DomainNamespace, ApplicationNamespace, InfrastructureNamespace, ApiNamespace, WebNamespace)
            .GetResult();

        Assert.True(condition: result.IsSuccessful, userMessage: "A camada Contract não deve depender de outras camadas.");
    }

    [Fact]
    public void Contract_ShouldNotDependOnSystemComponentModelDataAnnotations()
    {
        TestResult result = Types.InAssembly(ContractAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("System.ComponentModel.DataAnnotations")
            .GetResult();

        Assert.True(condition: result.IsSuccessful, userMessage: "A camada Contract não deve depender de DataAnnotations.");
    }

    [Fact]
    public void Application_ShouldNotHaveDependencyOnOuterLayers()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace, ApiNamespace, WebNamespace)
            .GetResult();

        Assert.True(condition: result.IsSuccessful, userMessage: "A camada Application não deve depender de Infrastructure, Api ou Web.");
    }

    [Fact]
    public void Application_ShouldNotDependOnReflectionBasedScanning()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny("FluentValidation.DependencyInjectionExtensions")
            .GetResult();

        Assert.True(condition: result.IsSuccessful, userMessage: "A camada Application não deve depender de pacotes de scanning reflexivo.");
    }

    [Fact]
    public void Infrastructure_ShouldNotHaveDependencyOnApiOrWeb()
    {
        TestResult result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApiNamespace, WebNamespace)
            .GetResult();

        Assert.True(condition: result.IsSuccessful, userMessage: "A camada Infrastructure não deve depender de Api ou Web.");
    }

    [Fact]
    public void Interfaces_ShouldStartWithI()
    {
        TestResult domainResult = Types.InAssembly(DomainAssembly)
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();

        TestResult contractResult = Types.InAssembly(ContractAssembly)
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();

        Assert.True(condition: domainResult.IsSuccessful, userMessage: "As interfaces da camada Domain devem iniciar com 'I'.");
        Assert.True(condition: contractResult.IsSuccessful, userMessage: "As interfaces da camada Contract devem iniciar com 'I'.");
    }

    [Fact]
    public void Validators_ShouldBeSealedAndImplementIValidator()
    {
        TestResult netArchResult = Types.InAssembly(ApplicationAssembly)
            .That()
            .HaveNameEndingWith("Validator")
            .Should()
            .BeSealed()
            .GetResult();

        Assert.True(condition: netArchResult.IsSuccessful, userMessage: "Todos os validadores de request devem ser sealed.");

        Type[] validatorTypes = [.. ApplicationAssembly.GetTypes().Where(static t => t.Name.EndsWith("Validator", StringComparison.Ordinal) && !t.IsNested)];

        Assert.NotEmpty(collection: validatorTypes);

        foreach (Type validatorType in validatorTypes)
        {
            Assert.True(
                condition: typeof(FluentValidation.IValidator).IsAssignableFrom(c: validatorType),
                userMessage: $"O validador '{validatorType.Name}' deve implementar IValidator.");
        }
    }

    [Fact]
    public void Mappers_ShouldBeStaticClasses()
    {
        Type[] mapperTypes = [.. ApplicationAssembly.GetTypes().Where(static t => t.Namespace == "Autogestor.Application.Mappers" && !t.IsNested)];

        Assert.NotEmpty(collection: mapperTypes);

        foreach (Type mapperType in mapperTypes)
        {
            Assert.True(
                condition: mapperType.IsAbstract && mapperType.IsSealed,
                userMessage: $"O mapper '{mapperType.Name}' deve ser uma classe estática (abstract e sealed).");

            MethodInfo[] methods = mapperType.GetMethods(bindingAttr: BindingFlags.Public | BindingFlags.Static);
            Assert.NotEmpty(collection: methods);
        }
    }
}
