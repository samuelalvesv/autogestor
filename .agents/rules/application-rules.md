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
  - `DTOs/`: Objetos de dados de transferência.
- `Interfaces/`: Contratos de serviços externos (IEmailService, IStorageService).
- `Validators/`: Validações de entrada de dados com FluentValidation.

## Diretrizes e Restrições
- **Isolamento de Infraestrutura**: Referencia apenas `Autogestor.Domain`. Proibido importar namespaces de infraestrutura (`System.Data`, `EntityFramework`, etc.).
- **Mediator (MediatR)**:
  - Cada caso de uso é um par Request/Handler (`IRequest<T>` e `IRequestHandler<TRequest, TResponse>`).
  - Cada caso de uso deve ser uma classe isolada com um único método público.
- **Autorização**: Consultar regra `identity-multitenancy` para verificação de permissão operacional com `IBranchAuthorizationService`.
