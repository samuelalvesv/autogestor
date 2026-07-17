---
name: testing-rules
description: Rules for Unit Tests, Integration Tests (NSubstitute, Testcontainers), Architecture Tests (NetArchTest), and TDD cycle.
applyTo: "test/**/*.cs"
---

# Regras de Testes (Autogestor.Tests)

## 1. Testes de Unidade (Autogestor.UnitTests)
- **Escopo**: Domain e Application.
- **Isolamento Total**: Proibido acessar banco de dados, rede (HTTP) ou ler arquivos do disco. Tudo roda estritamente em memória.
- **Uso de Mocks**: Usar mocks (NSubstitute/Moq) para isolar as dependências externas da camada de Application (ex: repositórios e serviços externos).
- **TDD (Test-Driven Development)**: Aplicar TDD rigoroso para novas regras de domínio e casos de uso (Red ➜ Green ➜ Refactor).
- **CancellationToken**: Sempre testar a propagação e tratamento do token em handlers assíncronos.

## 2. Testes de Integração (Autogestor.IntegrationTests)
- **Escopo**: Infrastructure e Api (Presentation).
- **Acesso Real ao Banco**: Usar banco de dados PostgreSQL real (via Testcontainers com contêineres Docker sob demanda).
- **Sem Mocks para Banco**: Proibido mockar o `DbContext` ou classes de repositórios.
- **Isolamento de Estado**: Cada teste deve garantir a limpeza/reversão de dados criados para não impactar os outros testes.

## 3. Testes de Arquitetura (Autogestor.ArchitectureTests)
- **Escopo**: Validar programaticamente o mapa de dependências e regras da Clean Architecture.
- **Tecnologia**: Usar `NetArchTest.eNet` para as asserções.
- **Regras**:
  - `Domain` não pode referenciar nenhum outro projeto ou pacote NuGet de infra (ex: EF Core).
  - `Application` referencia apenas `Domain` (proibido referenciar `Infrastructure`, `Api` ou `Web`).
  - Classes em Domain/Application devem ser `sealed` por padrão.
