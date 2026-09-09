# Workflow: Propagar Criação/Alteração de Domínio

Workflow acionável para guiar a propagação consistente de uma entidade de domínio através de todas as camadas impactadas do sistema: contratos gRPC Code-First (DTOs e serviços), infraestrutura de persistência (EF Core), isolamento multi-tenant e testes automatizados (unitários e de arquitetura).

## Como Usar
Invoque este workflow utilizando o comando:
```bash
/propagate-domain [NomeDaEntidade]
```

---

## Steps de Execução

### 1. Análise e Modelagem da Entidade de Domínio
- **Localização**: `src/Autogestor.Domain/Entities/[NomeDaEntidade].cs`.
- **Regras Aplicáveis**:
  - [.agents/rules/domain-rules.md](../rules/domain-rules.md) (Design rico, métodos de fábrica e encapsulamento de invariantes).
  - [.agents/rules/architecture.md](../rules/architecture.md) (Isolamento total da camada mais interna).
  - [.agents/rules/csharp-conventions.md](../rules/csharp-conventions.md) (Imutabilidade, argumentos nomeados e primary constructors).
- **Ferramentas por Origem e Plugin**:
  - **Submódulo `ponytail`**: skill `ponytail` (design enxuto e estritamente necessário — YAGNI).
  - **Plugin `dotnet-diag`**: subagente `optimizing-dotnet-performance` e skill `analyzing-dotnet-performance` (avaliação de alocações e eficiência no domínio).
- **Checklist**:
  - Herança de classe base correspondente à natureza da entidade (entidade pura com identificador, entidade global auditada ou entidade com escopo multi-tenant auditada).
  - Construtores privados ou protegidos combinados com métodos de fábrica expressivos que protegem o estado contra instanciações inválidas.
  - Ausência de propriedades de infraestrutura ou auditoria na assinatura pública do método de fábrica (preenchimento delegado exclusivamente aos mecanismos de persistência).

### 2. Propagação e Modelagem de Contratos gRPC (Autogestor.Contract)
- **Localização**:
  - `src/Autogestor.Contract/Responses/[Feature]/[NomeDaEntidade]Response.cs` (DTO de resposta).
  - `src/Autogestor.Contract/Requests/[Feature]/` (DTOs de requisição para operações de comando e consulta).
  - `src/Autogestor.Contract/Services/I[Feature]Service.cs` (Contrato de serviço Code-First gRPC).
- **Regras Aplicáveis**:
  - [.agents/rules/contracts-rules.md](../rules/contracts-rules.md) (DTOs imutáveis, sequencialidade Protobuf e tipagem forte).
  - [.agents/rules/architecture.md](../rules/architecture.md) (Isolamento total de dependências internas e de infraestrutura).
  - [.agents/rules/csharp-conventions.md](../rules/csharp-conventions.md) (Imutabilidade e propriedades obrigatórias).
- **Ferramentas por Origem e Plugin**:
  - **Plugin `dotnet-upgrade`**: skill `dotnet-aot-compat` (compatibilidade com compilação Native AOT, trimming e serialização).
  - **Plugin `dotnet-diag`**: subagente `optimizing-dotnet-performance` e skill `analyzing-dotnet-performance` (avaliação de alocações em DTOs e coleções).
  - **Submódulo `ponytail`**: skill `ponytail` (modelagem enxuta de contratos, eliminando dados redundantes já providos pelo contexto de autenticação).
- **Checklist**:
  - DTOs de Request e Response modelados como registros imutáveis com propriedades obrigatórias e sem construtores parametrizados customizados.
  - Herança dos DTOs de resposta alinhada à natureza da entidade (resposta de entidade com identificador, resposta de entidade auditada ou resposta de entidade com escopo de tenant).
  - Atributos explícitos de contrato de dados e ordenação de membros do Protobuf mantendo numeração positiva, contínua e estritamente única ao longo de toda a hierarquia de herança.
  - Contratos de serviço em interfaces dedicadas decoradas com atributos de serviço e operação para gRPC Code-First.
  - Coleções de retorno tipadas com interfaces de leitura indexadas para otimização de transporte e renderização no frontend.

### 3. Configuração de Mapeamento e Persistência (EF Core)
- **Localização**: `src/Autogestor.Infrastructure/Persistence/Configurations/[NomeDaEntidade]Configuration.cs`.
- **Regras Aplicáveis**:
  - [.agents/rules/infrastructure-rules.md](../rules/infrastructure-rules.md) (Mapeamento EF Core, tipos PostgreSQL e herança de configurações base).
  - [.agents/rules/database-rules.md](../rules/database-rules.md) (Convenções de schema, tipos e índices do PostgreSQL).
- **Ferramentas por Origem e Plugin**:
  - **Plugin `dotnet-data`**: skill `optimizing-ef-core-queries` (mapeamento com índices adequados e eficiência de acesso a dados).
  - **Submódulo `postgres-skills`**: skill `postgres-best-practices` (validação de tipos nativos e integridade referencial).
  - **Submódulo `agent-skills`**: servidor MCP `neon` (inspeção de constraints e tipagens no PostgreSQL).
- **Checklist**:
  - Herança da configuração base correspondente à natureza da entidade (configuração de entidade pura, auditada ou com escopo de tenant).
  - Mapeamento explícito de tipos nativos de banco para strings, valores temporais em UTC, números monetários com precisão adequada e identificadores únicos.
  - Definição semântica de regras de integridade referencial e exclusão para chaves estrangeiras (restrição para dados de apoio e cascata para agregados dependentes).
  - Adoção uniforme das convenções globais de nomenclatura sem anotações de tabela ou coluna manuais redundantes.

### 4. Registro no DbContext e Isolamento Multi-Tenancy
- **Localização**: `src/Autogestor.Infrastructure/Persistence/AppDbContext.cs`.
- **Regras Aplicáveis**:
  - [.agents/rules/infrastructure-rules.md](../rules/infrastructure-rules.md) (Centralização do DbContext).
  - [.agents/rules/identity-multitenancy.md](../rules/identity-multitenancy.md) (Filtros globais de consulta por tenant).
- **Ferramentas por Origem e Plugin**:
  - **Plugin `dotnet-data`**: skill `optimizing-ef-core-queries` (garantia de consultas eficientes com índices de tenant).
  - **Submódulo `agent-skills`**: servidor MCP `neon` (inspeção de constraints e integridade relacional).
- **Checklist**:
  - Exposição do conjunto de entidades (`DbSet`) correspondente no `AppDbContext`.
  - Garantia de que entidades com escopo de tenant recebam o filtro global de isolamento de forma automática e transparente.

### 5. Escrita e Auditoria dos Testes Automatizados
- **Localização**:
  - `test/Autogestor.UnitTests/Domain/Entities/[NomeDaEntidade]Tests.cs` (Testes de unidade do domínio).
  - `test/Autogestor.UnitTests/Contract/Responses/[Feature]/[NomeDaEntidade]ResponseTests.cs` (Testes de unidade de contratos).
  - `test/Autogestor.ArchitectureTests/ContractArchitectureTests.cs` (Testes arquiteturais de contratos).
- **Regras Aplicáveis**:
  - [.agents/rules/unit-testing-rules.md](../rules/unit-testing-rules.md) (Isolamento total em memória, convenções xUnit e TDD).
  - [.agents/rules/architecture-testing-rules.md](../rules/architecture-testing-rules.md) (Validação arquitetural contínua com NetArchTest).
  - [.agents/rules/csharp-conventions.md](../rules/csharp-conventions.md) (Argumentos nomeados obrigatórios).
  - [AGENTS.md](../../AGENTS.md) (Mensagens de asserção de teste em português brasileiro).
- **Ferramentas por Origem e Plugin**:
  - **Plugin `dotnet-test`**: subagente `test-quality-auditor`; skills `assertion-quality`, `test-anti-patterns`, `test-gap-analysis` (auditoria de profundidade de asserções, cobertura de mutações e conformidade de testes).
- **Checklist**:
  - Testes cobrindo instanciação válida via método de fábrica e verificação de estado resultante no domínio.
  - Testes cobrindo cenários de exceção para cada invariante e regra de negócio da entidade.
  - Testes cobrindo a integridade, imutabilidade e atribuição de campos dos DTOs de contrato.
  - Validação pelos testes de arquitetura da unicidade de ordens de serialização e presença de atributos obrigatórios em toda a cadeia de contratos.
  - Uso estrito de argumentos nomeados e mensagens descritivas em português brasileiro em todas as asserções.

### 6. Compilação e Validação da Solução
- **Execução CLI com prefixo `rtk`**:
  ```bash
  rtk dotnet build Autogestor.slnx
  rtk dotnet test
  ```
- **Em caso de erro de build**:
  - **Plugin `dotnet-msbuild`**: subagente `msbuild`, skill `binlog-failure-analysis` e servidor MCP `binlog` (investigação analítica de falhas de compilação).
