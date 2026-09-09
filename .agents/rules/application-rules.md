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
- **Autorização**: Consultar regra `identity-multitenancy` para verificação de permissão operacional com `IBranchAuthorizationService`.

## Ferramentas

- **Plugin `dotnet-test`**:
  - Subagente `test-quality-auditor`: Diagnóstico da qualidade, asserções e cobertura dos testes de casos de uso e orquestrações.
  - Skills `test-anti-patterns`, `assertion-quality`, `test-gap-analysis`: Detecção de testes fracos, validação de asserções completas e cobertura de validações FluentValidation.
- **Plugin `dotnet-diag`**:
  - Subagente `optimizing-dotnet-performance`: Otimização de pipelines de execução e fluxos assíncronos nos handlers do MediatR.
  - Skill `analyzing-dotnet-performance`: Análise de execução assíncrona, propagação de cancelamento e prevenção de alocações em handlers.
- **Plugin `dotnet-experimental`**:
  - Skill `exp-mock-usage-analysis`: Auditoria de dublês de teste e mocks nos handlers para garantir testes realistas e enxutos.
- **Plugin `dotnet-ai`**:
  - Skill `technology-selection`: Seleção e integração de bibliotecas de IA/ML (.NET AI, ONNX, ML.NET) nos casos de uso.
- **Submódulo `ponytail`**:
  - Skills `ponytail`, `ponytail-review`: Eliminação de camadas intermediárias desnecessárias, serviços vazios ou abstrações prematuras nos casos de uso.
- **Submódulo `agent-skills`**:
  - Skill `neon-ai-gateway`: Integração e consumo padronizado de modelos através da infraestrutura unificada do Neon nos fluxos da aplicação.
