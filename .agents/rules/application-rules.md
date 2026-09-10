---
name: application-rules
description: Use Cases, MediatR Commands/Queries, request validators, and application orchestrations.
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
- **Mediator (MediatR)**:
  - Cada caso de uso é um par Request/Handler (`IRequest<T>` e `IRequestHandler<TRequest, TResponse>`).
  - Cada caso de uso deve ser uma classe isolada (`sealed`) com um único método público.
- **Identity & Multi-Tenancy**: Seguir integralmente [.agents/rules/identity-multitenancy.md](identity-multitenancy.md).

## Ferramentas

- **Plugin `dotnet-test`**: subagente `test-quality-auditor`, skills `test-anti-patterns`, `assertion-quality`, `test-gap-analysis`
- **Plugin `dotnet-diag`**: subagente `optimizing-dotnet-performance`, skill `analyzing-dotnet-performance`
- **Plugin `dotnet-experimental`**: skill `exp-mock-usage-analysis`
- **Plugin `dotnet-ai`**: skill `technology-selection`
- **Submódulo `ponytail`**: skills `ponytail`, `ponytail-review`
- **Submódulo `agent-skills`**: skill `neon-ai-gateway`
