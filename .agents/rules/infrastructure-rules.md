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
- **Auditoria Automatizada (Abordagem B)**: As propriedades de auditoria (`CreatedBy`, `CreatedAt`, `UpdatedBy`, `UpdatedAt`) de `AuditableEntity` e `TenantEntity` são preenchidas de forma 100% automatizada na infraestrutura através de interceptadores do EF Core no `AppDbContext` obtendo o usuário logado do contexto. Construtores e factory methods do Domínio **não** devem receber ou inicializar essas propriedades em suas assinaturas.
- **Configurações da Fluent API**: Toda classe de configuração de entidade em `Persistence/Configurations/` deve herdar da respectiva classe base de configuração de acordo com a entidade: `AuditableEntityConfiguration<TEntity>` (para `AuditableEntity`), `TenantEntityConfiguration<TEntity>` (para `TenantEntity`) ou `EntityConfiguration<TEntity>` (para entidades simples herdadas diretamente de `Entity`).
- **Consultas Eficientes**:
  - Usar `.AsNoTracking()` em todas as consultas que sejam estritamente para leitura.
  - Repositórios **nunca** devem retornar `IQueryable`. Toda consulta deve ser materializada na camada de infraestrutura (retornando `IReadOnlyList<T>`, `IEnumerable<T>` ou `T?`) para evitar vazamento de complexidade de banco (como N+1 queries) para a camada de Application.
- **Convenção de Nomenclatura (snake_case)**:
  - A tradução para snake_case é automática (`UseSnakeCaseNamingConvention()`). Não utilizar `ToTable` ou `HasColumnName` nas classes de mapeamento para este propósito.
- **Tipos de Coluna (PostgreSQL)**:
  - **Data e Hora**: Mapear propriedades `DateTime` com o tipo de coluna `"timestamptz"` para suporte correto a UTC global.
  - **Texto**: Mapear propriedades string com o tipo de coluna `"text"`.
- **Ciclo de Vida e Registro de Interceptadores**: Interceptadores do EF Core que dependem de serviços com ciclo de vida com escopo devem ser registrados obrigatoriamente no contêiner de injeção de dependência e resolvidos dinamicamente na configuração do contexto de banco de dados. É proibido instanciá-los manualmente com operador de instanciação direta ou mantê-los como campos estáticos no contexto.
- **Desacoplamento de Entidades em Interceptadores**: Interceptadores de infraestrutura devem operar exclusivamente sobre contratos ou classes base genéricas de domínio. É estritamente proibido acoplar a execução a tipos concretos, verificações de tipos derivados ou condicionais específicos para entidades individuais.
- **Isolamento de Banco (Multi-tenant)**: Consultar regra `identity-multitenancy` para detalhes de filtros globais.
