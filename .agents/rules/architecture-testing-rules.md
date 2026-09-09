---
name: architecture-testing-rules
description: Rules for Architecture Tests, NetArchTest constraints, dependencies mapping verification, and naming conventions.
applyTo: "test/Autogestor.ArchitectureTests/**/*.cs"
---

# Regras de Testes de Arquitetura (Autogestor.ArchitectureTests)

## Diretrizes e Responsabilidades
- **Foco**: Testes de arquitetura automatizados para validar programaticamente o mapa de dependências e regras de design da Clean Architecture.
- **Validação de Dependências**:
  - A camada `Autogestor.Domain` não pode referenciar nenhum outro projeto da Solution nem bibliotecas de terceiros de infraestrutura.
  - A camada `Autogestor.Application` deve referenciar apenas `Domain` e `Contract` (proibido referenciar `Infrastructure`, `Api`, `Web`).
  - Classes e tipos expostos em `Domain` e `Application` não devem possuir referências ou dependências com pacotes NuGet específicos de banco ou rede (como `Microsoft.EntityFrameworkCore`).
- **Padrões de Nomenclatura e Design**:
  - Garantir que todas as interfaces comecem com `I` (ex: `IXxxRepository`).
  - Garantir que classes dentro de Domain e Application sejam marcadas como `sealed` por padrão, a menos que herança seja explicitamente intencional.
- **Tecnologia**: Usar a biblioteca **`NetArchTest.eNhancedEdition`** para definir as asserções de arquitetura de forma fluida.

## Ferramentas

- **Plugin `dotnet-test`**:
  - Subagente `test-quality-auditor`: Auditoria das asserções de integridade arquitetural garantindo que os testes de conformidade com NetArchTest sejam infalíveis.
  - Skills `test-anti-patterns`, `assertion-quality`: Garantia de asserções ricas com NetArchTest que falhem explicitamente em caso de desrespeito a regras de dependência.
  - Skill `find-untested-sources`: Garantia de que novos assemblies ou subespaços de nomes criados na solução sejam cobertos pelas regras de testes.
- **Plugin `dotnet-msbuild`**:
  - Subagente `msbuild-code-review`: Verificação estática de referências de projetos nos arquivos `.csproj` para validar limites arquiteturais.
  - Skills `directory-build-organization`, `msbuild-antipatterns`: Auditoria da estrutura de propriedades e prevenção de acoplamentos indevidos.
  - Servidor MCP `binlog`: Inspeção do grafo de dependências compiladas e referências reais de assemblies na solução.
