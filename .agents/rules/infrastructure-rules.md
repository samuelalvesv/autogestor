---
name: infrastructure-rules
description: EF Core queries, mapping conventions, repository materialization guidelines, and PostgreSQL column types.
applyTo: "src/Autogestor.Infrastructure/**/*.cs"
---

# Regras de Infraestrutura (Autogestor.Infrastructure)

## Estrutura de Pastas
- `Persistence/AppDbContext.cs`: Configuração central do EF Core.
- `Persistence/Configurations/`: Configurações da Fluent API (`IEntityTypeConfiguration<T>`).
- `Persistence/Repositories/`: Implementações concretas de repositórios.
- `Services/`: Serviços de infraestrutura (Email, Storage, etc.).
- `DependencyInjection.cs`: Registro unificado dos serviços de infra.

## Diretrizes de EF Core e Persistência
- **Consultas Eficientes**:
  - Usar `.AsNoTracking()` em todas as consultas que sejam estritamente para leitura.
  - Repositórios **nunca** devem retornar `IQueryable`. Toda consulta deve ser materializada na camada de infraestrutura (retornando `IReadOnlyList<T>`, `IEnumerable<T>` ou `T?`) para evitar vazamento de complexidade de banco (como N+1 queries) para a camada de Application.
- **Convenção de Nomenclatura (snake_case)**:
  - A tradução para snake_case é automática (`UseSnakeCaseNamingConvention()`). Não utilizar `ToTable` ou `HasColumnName` nas classes de mapeamento para este propósito.
- **Tipos de Coluna (PostgreSQL)**:
  - **Data e Hora**: Mapear propriedades `DateTime` com o tipo de coluna `"timestamptz"` para suporte correto a UTC global.
  - **Texto**: Mapear propriedades string com o tipo de coluna `"text"`.
- **Isolamento de Banco (Multi-tenant)**: Consultar regra `identity-multitenancy` para detalhes de filtros globais.
