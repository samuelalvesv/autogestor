using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using Autogestor.Contract;

namespace Autogestor.ArchitectureTests;

public class ContractArchitectureTests
{
    private static readonly Assembly ContractAssembly = typeof(ContractDefaults).Assembly;

    [Fact]
    public void All_Dto_Types_Should_Have_DataContractAttribute()
    {
        IEnumerable<Type> dtoTypes = ContractAssembly.GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract &&
                        (t.Namespace?.StartsWith("Autogestor.Contract.Requests", StringComparison.Ordinal) == true ||
                         t.Namespace?.StartsWith("Autogestor.Contract.Responses", StringComparison.Ordinal) == true));

        foreach (Type? type in dtoTypes)
        {
            DataContractAttribute? dataContract = type.GetCustomAttribute<DataContractAttribute>(inherit: false);
            Assert.True(dataContract is not null, $"O DTO '{type.FullName}' deve ser decorado com [DataContract].");
        }
    }

    [Fact]
    public void All_DataMember_Orders_In_Hierarchy_Should_Be_Positive_And_Unique()
    {
        IEnumerable<Type> dtoTypes = ContractAssembly.GetExportedTypes()
            .Where(t => t.IsClass && !t.IsAbstract &&
                        (t.Namespace?.StartsWith("Autogestor.Contract.Requests", StringComparison.Ordinal) == true ||
                         t.Namespace?.StartsWith("Autogestor.Contract.Responses", StringComparison.Ordinal) == true));

        foreach (Type? type in dtoTypes)
        {
            var propertiesWithDataMember = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Select(p => new { Property = p, Attr = p.GetCustomAttribute<DataMemberAttribute>(inherit: true) })
                .Where(x => x.Attr is not null)
                .ToList();

            foreach (var item in propertiesWithDataMember)
            {
                Assert.True(item.Attr!.Order > 0,
                    $"A propriedade '{item.Property.Name}' no tipo '{type.FullName}' possui Order inválida não positiva: {item.Attr.Order}.");
            }

            var duplicateOrders = propertiesWithDataMember
                .GroupBy(x => x.Attr!.Order)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            Assert.True(duplicateOrders.Count == 0,
                $"O tipo '{type.FullName}' contém valores duplicados de Order em [DataMember]: {string.Join(", ", duplicateOrders)}.");
        }
    }

    [Fact]
    public void All_Service_Contracts_Should_Be_Interfaces_With_ServiceContractAttribute()
    {
        IEnumerable<Type> serviceTypes = ContractAssembly.GetExportedTypes()
            .Where(t => t.Namespace?.StartsWith("Autogestor.Contract.Services", StringComparison.Ordinal) == true);

        foreach (Type? type in serviceTypes)
        {
            Assert.True(type.IsInterface, $"O tipo de serviço '{type.FullName}' deve ser uma interface.");

            ServiceContractAttribute? serviceContract = type.GetCustomAttribute<ServiceContractAttribute>(inherit: false);
            Assert.True(serviceContract is not null, $"O contrato de serviço '{type.FullName}' deve ser decorado com [ServiceContract].");

            foreach (MethodInfo method in type.GetMethods())
            {
                OperationContractAttribute? operationContract = method.GetCustomAttribute<OperationContractAttribute>(inherit: false);
                Assert.True(operationContract is not null, $"O método '{method.Name}' em '{type.FullName}' deve ser decorado com [OperationContract].");
            }
        }
    }
}
