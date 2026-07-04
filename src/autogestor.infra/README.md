# autogestor.infra (Infrastructure)

Camada de implementações concretas e integração com tecnologias externas. Ver [architecture.md](../../.agents/architecture.md) para o mapa completo.

## Estrutura de Pastas Esperada
```
autogestor.infra/
├── Persistence/
│   ├── AppDbContext.cs         # DbContext do Entity Framework Core
│   ├── Configurations/        # Fluent API (IEntityTypeConfiguration)
│   └── Repositories/          # Implementações dos repositórios definidos em core
├── Services/                  # Implementações de serviços definidos em app (Email, Storage)
└── DependencyInjection.cs     # Extension method para registrar todos os serviços do infra
```

## Regras Específicas
- Referencia `autogestor.core` e `autogestor.app`.
- As classes de repositório implementam interfaces do `core` (ex: `IOrderRepository`).
- As classes de serviço implementam interfaces do `app` (ex: `IEmailService`).
- Expor um único método de extensão `AddInfrastructure(this IServiceCollection)` para registrar tudo.
