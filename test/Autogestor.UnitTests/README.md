# Autogestor.UnitTests

Testes unitários focados nas regras de negócio e lógica pura da aplicação.

## Responsabilidades

- **Domain**: Testes de entidades de domínio, Value Objects, Domain Services e Domain Events.
- **Application**: Testes de Use Cases / Request Handlers (Commands e Queries) e validators de entrada.

## Regras Específicas

- **Isolamento Total**: **Não** é permitido acessar bancos de dados, realizar chamadas de rede (HTTP) ou ler arquivos físicos do disco. Tudo deve rodar estritamente na memória.
- **Uso de Mocks**: Usar mocks (NSubstitute/Moq) para simular dependências da camada `Application` (ex: simular retornos de interfaces `IXxxRepository`, `IEmailService`, etc.).
- **TDD (Test-Driven Development)**: Aplicar TDD rigoroso para novas regras de domínio e fluxos de aplicação.
  - Ciclo: Escrever o teste que falha (Red) ➜ Escrever o código mínimo para passar (Green) ➜ Refatorar o código (Refactor).
- **CancellationToken**: Sempre passar `CancellationToken.None` ou testar fluxos de cancelamento nos Handlers assíncronos.
- **Performance**: Todos os testes devem ser extremamente rápidos (rodando em poucos milissegundos).
