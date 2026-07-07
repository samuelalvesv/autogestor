# Autogestor.Application

Camada de casos de uso e orquestração de fluxo. Ver [architecture.md](../../.agents/architecture.md) para o mapa completo.

## Estrutura de Pastas Esperada

```text
Autogestor.Application/
├── UseCases/
│   └── [Feature]/
│       ├── Commands/      # Operações de escrita (Create, Update, Delete)
│       ├── Queries/       # Operações de leitura
│       └── DTOs/          # Objetos de transferência de dados
├── Interfaces/            # Contratos de serviços externos (IEmailService, IStorageService)
└── Validators/            # Validações de entrada (FluentValidation)
```text

## Regras Específicas

- Referencia apenas `Autogestor.Domain`.
- Nunca importar namespaces de infraestrutura (`System.Data`, `EntityFramework`).
- Cada caso de uso deve ser uma classe isolada com um único método público.
- Adotar o padrão **Mediator** (via `MediatR`) para separar a recepção de requisições (API) da execução dos casos de uso (Handlers).
