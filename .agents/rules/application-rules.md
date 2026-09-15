---
name: application-rules
description: Use Cases, Commands/Queries, request validators, and application orchestrations.
applyTo: "src/Autogestor.Application/**/*.cs"
---

# Regras de Aplicação (Autogestor.Application)

## Estrutura de Pastas
- `UseCases/[Feature]/`: 
  - `Commands/`: Operações de escrita.
  - `Queries/`: Operações de leitura.
  - `DTOs/`: Objetos de dados de transferência internos.
- `Interfaces/`: Contratos de serviços externos (IEmailService, IStorageService).
- `Validators/`: Validações de entrada de dados com FluentValidation.

## Diretrizes e Restrições
- **Isolamento de Infraestrutura**: Referencia apenas `Autogestor.Domain` e `Autogestor.Contract`. Proibido importar namespaces de infraestrutura (`System.Data`, `Microsoft.EntityFrameworkCore`, etc.).
- **Orquestração de Casos de Uso e Invocação Direta**:
  - A camada de apresentação consome casos de uso através de injeção direta de dependência (`ICreate[Feature]UseCase`), dispensando mediadores dinâmicos e reflexão em tempo de execução, garantindo total compatibilidade com Native AOT.
  - Cada caso de uso deve ser uma classe isolada (`sealed`) com um único método público (`ExecuteAsync`).
- **Identity & Multi-Tenancy**: Seguir integralmente [.agents/rules/identity-multitenancy.md](identity-multitenancy.md).

## Ferramentas

- **Plugin `dotnet-test`**: subagente `test-quality-auditor`, skills `test-anti-patterns`, `assertion-quality`, `test-gap-analysis`
- **Plugin `dotnet-diag`**: subagente `optimizing-dotnet-performance`, skill `analyzing-dotnet-performance`
- **Plugin `dotnet-experimental`**: skill `exp-mock-usage-analysis`
- **Plugin `dotnet-ai`**: skill `technology-selection`
- **Submódulo `ponytail`**: skills `ponytail`, `ponytail-review`
- **Submódulo `agent-skills`**: skill `neon-ai-gateway`
