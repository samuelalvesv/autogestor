# Workflow: Propagar Criação/Alteração de Domínio

Workflow para guiar a IA na propagação de uma entidade de domínio C# para as camadas de infraestrutura (EF Core), persistência e testes de unidade (xUnit), assegurando o isolamento de multi-tenancy e integridade arquitetural.

## Como Usar
Invoque este workflow utilizando o comando:
```bash
/propagate-domain [NomeDaEntidade]
```

---

## Steps de Execução

### 1. Análise e Design da Entidade de Domínio
- **Localização**: `src/Autogestor.Domain/Entities/[NomeDaEntidade].cs`.
- **Regras Aplicáveis**:
  - [.agents/rules/domain-rules.md](../rules/domain-rules.md) (Design rico, métodos de fábrica e encapsulamento).
  - [.agents/rules/csharp-conventions.md](../rules/csharp-conventions.md) (Imutabilidade, argumentos nomeados e primary constructors).
- **Ferramentas por Origem e Plugin**:
  - **Submódulo `ponytail`**: skill `ponytail` (design enxuto e estritamente necessário - YAGNI).
  - **Plugin `dotnet-diag`**: subagente `optimizing-dotnet-performance` e skill `analyzing-dotnet-performance` (avaliação de alocações e eficiência no domínio).
- **Checklist**:
  - A entidade herda de `AuditableEntity` (dados globais auditados), `TenantEntity` (dados com escopo multi-tenant auditados) ou `Entity` (casos especiais não auditados)?
  - Construtores são privados/protegidos com métodos de fábrica (`Create`, `From`) expressivos?
  - Não há propriedades de auditoria (`CreatedBy`, `CreatedAt`, `UpdatedBy`, `UpdatedAt`) expostas na assinatura do factory method (preenchimento é 100% automatizado na infraestrutura)?

### 2. Configuração da Fluent API (EF Core)
- **Localização**: `src/Autogestor.Infrastructure/Persistence/Configurations/[NomeDaEntidade]Configuration.cs`.
- **Regras Aplicáveis**:
  - [.agents/rules/infrastructure-rules.md](../rules/infrastructure-rules.md) (Mapeamento EF Core, tipos PostgreSQL e herança de configurações base).
  - [.agents/rules/database-rules.md](../rules/database-rules.md) (Convenções de escrita e tipos do PostgreSQL 18).
- **Ferramentas por Origem e Plugin**:
  - **Plugin `dotnet-data`**: skill `optimizing-ef-core-queries` (mapeamento com índices adequados e modelagem eficiente).
  - **Submódulo `postgres-skills`**: skill `postgres-best-practices` (validação de tipos nativos e integridade referencial no PostgreSQL).
  - **Submódulo `agent-skills`**: servidor MCP `neon` (inspeção de constraints e tipagens no schema do PostgreSQL).
- **Checklist**:
  - A classe herda de `AuditableEntityConfiguration<[NomeDaEntidade]>`, `TenantEntityConfiguration<[NomeDaEntidade]>` ou `EntityConfiguration<[NomeDaEntidade]>`?
  - Mapeamento de tipos C#:
    - Nulabilidade: Propriedades anuláveis (`string?`, `int?`) com `.IsRequired(false)`. Obrigatórias com `.IsRequired()`.
    - Strings: `.HasColumnType("text")`.
    - Data/Hora: `.HasColumnType("timestamptz")`.
    - Decimais: `.HasColumnType("numeric(18,2)")`.
    - Chaves estrangeiras: Configurar delete behavior semântico (`DeleteBehavior.Restrict` para tabelas de lookup/apoio, `DeleteBehavior.Cascade` para agregados filhos órfãos).
    - Não usar `.ToTable()` ou `.HasColumnName()` manuais (a convenção `snake_case` age globalmente).

### 3. Registro no DbContext e Isolamento Multi-Tenancy
- **Localização**: `src/Autogestor.Infrastructure/Persistence/AppDbContext.cs`.
- **Regras Aplicáveis**:
  - [.agents/rules/infrastructure-rules.md](../rules/infrastructure-rules.md) (Centralização do DbContext).
  - [.agents/rules/identity-multitenancy.md](../rules/identity-multitenancy.md) (Global Query Filters por `TenantId`).
- **Ferramentas por Origem e Plugin**:
  - **Plugin `dotnet-data`**: skill `optimizing-ef-core-queries` (garantia de consultas indexadas eficientes com tenant).
  - **Submódulo `agent-skills`**: servidor MCP `neon` (inspeção de constraints e integridade das tabelas no PostgreSQL).
- **Checklist**:
  - Declarar a propriedade `DbSet<[NomeDaEntidade]>` correspondente no `AppDbContext`.
  - Se a entidade herdar de `TenantEntity`, certificar-se de que o Global Query Filter de `TenantId` seja aplicado automaticamente pelo método base/convenção do contexto.

### 4. Escrita e Auditoria dos Testes Unitários (xUnit)
- **Localização**: `test/Autogestor.UnitTests/Domain/Entities/[NomeDaEntidade]Tests.cs`.
- **Regras Aplicáveis**:
  - [.agents/rules/unit-testing-rules.md](../rules/unit-testing-rules.md) (Isolamento total em memória, convenções de testes xUnit, TDD e YAGNI).
- **Ferramentas por Origem e Plugin**:
  - **Plugin `dotnet-test`**: subagente `test-quality-auditor`; skills `assertion-quality`, `test-anti-patterns`, `test-gap-analysis` (auditoria de qualidade, profundidade de asserções e mutações).
- **Checklist**:
  - Testes cobrindo instanciação válida via factory method e validação de estado resultante.
  - Testes cobrindo cenários de falha para cada invariante e regra de validação de negócio da entidade.

### 5. Compilação e Validação
- **Execução CLI**:
  ```bash
  rtk dotnet build Autogestor.slnx
  rtk dotnet test test/Autogestor.UnitTests --filter "[NomeDaEntidade]Tests"
  ```
- **Em caso de erro de build**:
  - **Plugin `dotnet-msbuild`**: subagente `msbuild`, skill `binlog-failure-analysis` e servidor MCP `binlog` (investigação de falhas de compilação e inspeção analítica do build).
