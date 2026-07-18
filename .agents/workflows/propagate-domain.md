# Propagar Criação/Alteração de Domínio

Workflow para guiar a IA na propagação de uma entidade de domínio C# para as camadas de infraestrutura (EF Core) e testes de unidade (xUnit).

## Como Usar
Invoque este workflow utilizando o comando `/propagate-domain [NomeDaEntidade]`.

## Steps

1. **Análise da Entidade de Domínio**:
   - Localize e leia o arquivo da entidade em `src/Autogestor.Domain/Entities/[NomeDaEntidade].cs`.
   - Consulte e siga as diretrizes em [.agents/rules/domain-rules.md](../rules/domain-rules.md), validando o design da entidade e ajustando o arquivo do domínio caso identifique alguma desconformidade.
   - Garanta que a entidade herde de `AuditableEntity` (dados globais auditados), `TenantEntity` (dados com escopo multi-tenant) ou diretamente de `Entity` (casos de exceção não auditados, como o próprio usuário, logs ou tabelas estáticas).

2. **Configuração da Fluent API (EF Core)**:
   - Crie ou atualize o arquivo de configuração em `src/Autogestor.Infrastructure/Persistence/Configurations/[NomeDaEntidade]Configuration.cs`.
   - Garanta que a classe de configuração herde de: `AuditableEntityConfiguration<[NomeDaEntidade]>` (se herdar de `AuditableEntity`), `TenantEntityConfiguration<[NomeDaEntidade]>` (se herdar de `TenantEntity`) ou `EntityConfiguration<[NomeDaEntidade]>` (se herdar diretamente de `Entity`).
   - Mapeia as propriedades da entidade no EF Core respeitando:
     - Nulabilidade C#: Propriedades anuláveis (ex: `string?`, `int?`) devem usar `.IsRequired(false)`. Propriedades obrigatórias (ex: `string`, `int`) devem usar `.IsRequired()`.
     - Strings: `.HasColumnType("text")`.
     - DateTime: `.HasColumnType("timestamptz")`.
     - Decimais: `.HasColumnType("numeric(18,2)")`.
     - Relacionamentos (Chaves Estrangeiras): Configurar o comportamento do delete lógico ao negócio (ex: `DeleteBehavior.Restrict` para tabelas de apoio/categorias, ou `DeleteBehavior.Cascade` para exclusão de agregados filhos órfãos).
     - Não use `.ToTable()` ou `.HasColumnName()` para formatação (deixe o `snake_case` automático agir).

3. **Registro no DbContext**:
   - Abra `src/Autogestor.Infrastructure/Persistence/AppDbContext.cs`.
   - Adicione a propriedade `DbSet<[NomeDaEntidade]>` correspondente à nova entidade no DbContext.

4. **Escrita dos Testes Unitários (xUnit)**:
   - Crie ou atualize o arquivo de teste em `test/Autogestor.UnitTests/Domain/Entities/[NomeDaEntidade]Tests.cs`.
   - Escreva testes de unidade cobrindo:
     - Criação com parâmetros válidos (verificando o estado resultante e valores padrão).
     - Cenários de falha para cada parâmetro validado pelo método de fábrica (ex: strings nulas/vazias, valores inválidos, GUIDs vazios), garantindo a validação de disparo de `ArgumentException` e do nome do parâmetro (`exception.ParamName`).
   - Utilize as ferramentas e diretrizes do plugin `dotnet-test` para auxiliar no scaffold e na estruturação das asserções de teste.
