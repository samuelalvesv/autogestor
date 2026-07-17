# Autogestor.Infrastructure

Camada de implementações concretas e integração com tecnologias externas. Ver [architecture.md](../../.agents/architecture.md) para o mapa completo.

## Estrutura de Pastas Esperada

```text
Autogestor.Infrastructure/
├── Persistence/
│   ├── AppDbContext.cs         # DbContext do Entity Framework Core
│   ├── Configurations/        # Fluent API (IEntityTypeConfiguration)
│   └── Repositories/          # Implementações dos repositórios definidos em core
├── Services/                  # Implementações de serviços definidos em app (Email, Storage)
└── DependencyInjection.cs     # Extension method para registrar todos os serviços do infra
```text

## Regras Específicas

- Referencia `Autogestor.Domain` e `Autogestor.Application`.
- As classes de repositório implementam interfaces do `Domain` (ex: `IOrderRepository`).
- As classes de serviço implementam interfaces do `Application` (ex: `IEmailService`).
- Expor um único método de extensão `AddInfrastructure(this IServiceCollection)` para registrar tudo.

## Diretrizes do EF Core & Banco

- **Consultas Eficientes**:
  - Usar `.AsNoTracking()` em todas as consultas que sejam estritamente para leitura.
  - Repositórios **nunca** devem retornar `IQueryable`. Toda consulta deve ser materializada na camada de infraestrutura (retornando `IReadOnlyList<T>`, `IEnumerable<T>` ou `T?`) para evitar que consultas complexas ou o problema de N+1 vazem para a camada de Application.
- **Convenção de Nomenclatura (snake_case)**:
  - A tradução de nomes de tabelas e colunas de PascalCase para snake_case é feita automaticamente pela biblioteca `EFCore.NamingConventions` (usando `UseSnakeCaseNamingConvention()` nas opções do DbContext). Portanto, não utilizar `ToTable` ou `HasColumnName` nas classes de mapeamento (Fluent API) para este propósito.
- **Tipos de Coluna Modernos (PostgreSQL)**:
  - **Data e Hora**: Mapear todas as propriedades do tipo `DateTime` com o tipo de coluna `"timestamptz"` para suportar UTC globalmente.
  - **Texto**: Mapear todas as propriedades do tipo string com o tipo de coluna `"text"`.
- **Isolamento de Banco de Dados**: Consultar [identity-multitenancy.md](../../.agents/identity-multitenancy.md) para detalhes sobre a resolução do `TenantProvider` via cabeçalhos gRPC e configuração dos filtros globais no `DbContext`.
