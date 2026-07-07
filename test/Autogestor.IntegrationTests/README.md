# Autogestor.IntegrationTests

Testes de integração focados na validação dos contratos, integrações com o banco de dados e comunicação entre serviços externos.

## Responsabilidades

- **Infrastructure**: Testar a persistência real no PostgreSQL (mapeamentos do EF Core, repositórios concretos e Global Query Filters).
- **Api (Presentation)**: Testar as APIs mínimas (endpoints), middlewares, políticas de autorização e injeção de dependência via `WebApplicationFactory`.

## Regras Específicas

- **Acesso Real ao Banco**: Os testes devem interagir com um banco de dados PostgreSQL real (preferencialmente utilizando **Testcontainers** para instanciar contêineres Docker sob demanda durante a execução dos testes).
- **Sem Mocks para Banco**: Proibido mockar o `DbContext` ou as classes de repositórios. O objetivo desta camada é garantir que a persistência funcione exatamente como em produção.
- **Isolamento de Estado**: Cada teste deve garantir a limpeza ou reversão de dados criados para que a execução de um teste não afete o resultado do próximo.
- **Tratamento de Cancelamento**: Validar que os métodos de infraestrutura e controladores propagam e respeitam o `CancellationToken` quando requisitado.
