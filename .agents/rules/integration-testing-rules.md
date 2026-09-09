---
name: integration-testing-rules
description: Rules for Integration Tests, database persistence checks, PostgreSQL validation using Testcontainers, and cancellation handling.
applyTo: "test/Autogestor.IntegrationTests/**/*.cs"
---

# Regras de Testes de Integração (Autogestor.IntegrationTests)

## Diretrizes e Responsabilidades
- **Foco**: Testar a persistência real no PostgreSQL (mapeamentos do EF Core, repositórios concretos e Global Query Filters) e APIs mínimas/controladores da camada de apresentação.
- **Acesso Real ao Banco**: Os testes devem interagir com um banco de dados PostgreSQL real (utilizando **Testcontainers** para instanciar contêineres Docker sob demanda durante a execução dos testes).
- **Sem Mocks para Banco**: Proibido mockar o `DbContext` ou as classes de repositórios. O objetivo é testar a integração real.
- **Isolamento de Estado**: Cada teste deve garantir a limpeza ou reversão de dados criados para que a execução de um teste não afete o resultado do próximo.
- **Tratamento de Cancelamento**: Validar que os métodos de infraestrutura e controladores propagam e respeitam o `CancellationToken` quando requisitado.
- **Princípio YAGNI e Escopo Real**: Focar estritamente em integrações reais (queries no banco, persistência, interceptors e fluxos ponta a ponta). É proibido criar testes de integração para checagens sintáticas, validações de nulidade já garantidas pelo framework ou comportamentos intrínsecos do C#.
- **Convenção de Nomenclatura para Recursos Compartilhados**: Classes responsáveis pela inicialização e gerenciamento do ciclo de vida de contêineres e dependências externas compartilhadas devem utilizar obrigatoriamente o sufixo `Fixture`.
- **Escopo Exclusivo de Infraestrutura Real**: Este projeto é reservado estritamente para validações que exigem a integração efetiva com recursos reais em contêineres. É expressamente proibido alocar neste projeto testes que utilizem dublês de repositório em substituição ao banco de dados ou testes unitários de regras lógicas que não exerçam persistência real.

## Ferramentas

- **Plugin `dotnet-test`**:
  - Subagente `test-quality-auditor`: Auditoria da suíte de testes de integração e robustez dos fluxos ponta a ponta.
  - Skills `test-anti-patterns`, `assertion-quality`: Diagnóstico de fragilidades e validação de estado efetivamente persistido no banco.
  - Skills `test-gap-analysis`, `test-analysis-extensions`: Análise de mutações e extensões de asserções em fluxos integrados.
  - Skills `find-untested-sources`, `coverage-analysis`: Mapeamento de repositórios e interceptadores sem cobertura de integração.
- **Plugin `dotnet-data`**:
  - Skill `optimizing-ef-core-queries`: Inspeção do SQL gerado nas consultas de integração para evitar queries lentas e múltiplos round-trips.
- **Plugin `dotnet-experimental`**:
  - Skill `exp-test-maintainability`: Reutilização limpa e redução de duplicação de setup em classes de `Fixture`.
- **Submódulo `postgres-skills`**:
  - Skill `postgres-best-practices`: Validação das definições de tabelas, índices e tipos de dados no PostgreSQL do Testcontainers.
- **Submódulo `agent-skills`**:
  - Skill `neon-postgres-branches`: Criação de branches efêmeras de banco para simular e validar migrações e persistência de integração.
  - Servidor MCP `neon`: Comparação entre os mapeamentos executados no Testcontainers e o schema esperado no Lakebase Postgres.
