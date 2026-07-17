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
