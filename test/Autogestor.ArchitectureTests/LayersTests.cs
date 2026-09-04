using System.Reflection;
using NetArchTest.Rules;

namespace Autogestor.ArchitectureTests;

public class LayersTests
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

        Assert.True(result.IsSuccessful, "A camada Domain não deve depender de outras camadas.");
    }

    [Fact]
    public void Contract_ShouldNotHaveDependencyOnOtherLayers()
    {
        TestResult result = Types.InAssembly(ContractAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(DomainNamespace, ApplicationNamespace, InfrastructureNamespace, ApiNamespace, WebNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, "A camada Contract não deve depender de outras camadas.");
    }

    [Fact]
    public void Application_ShouldNotHaveDependencyOnOuterLayers()
    {
        TestResult result = Types.InAssembly(ApplicationAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(InfrastructureNamespace, ApiNamespace, WebNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, "A camada Application não deve depender de Infrastructure, Api ou Web.");
    }

    [Fact]
    public void Infrastructure_ShouldNotHaveDependencyOnApiOrWeb()
    {
        TestResult result = Types.InAssembly(InfrastructureAssembly)
            .ShouldNot()
            .HaveDependencyOnAny(ApiNamespace, WebNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, "A camada Infrastructure não deve depender de Api ou Web.");
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

        Assert.True(domainResult.IsSuccessful, "As interfaces da camada Domain devem iniciar com 'I'.");
        Assert.True(contractResult.IsSuccessful, "As interfaces da camada Contract devem iniciar com 'I'.");
    }
}
