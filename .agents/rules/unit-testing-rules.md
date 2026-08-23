---
name: unit-testing-rules
description: Rules for Unit Tests, pure memory business logic validation, NSubstitute mocking, and TDD cycle (Red-Green-Refactor).
applyTo: "test/Autogestor.UnitTests/**/*.cs"
---

# Regras de Testes de Unidade (Autogestor.UnitTests)

## Diretrizes e Responsabilidades
- **Foco**: Testes unitários de regras de negócio (`Domain`) e casos de uso (`Application`).
- **Isolamento Total**: Proibido acessar banco de dados, realizar chamadas de rede (HTTP) ou ler arquivos físicos do disco. Tudo deve rodar estritamente na memória.
- **Uso de Mocks**: Usar mocks (NSubstitute/Moq) para isolar as dependências da camada `Application` (ex: simular retornos de interfaces `IXxxRepository`, `IEmailService`, etc.).
- **TDD (Test-Driven Development)**: Aplicar TDD rigoroso para novas regras de domínio e fluxos de aplicação.
  - Ciclo: Escrever o teste que falha (Red) ➜ Escrever o código mínimo para passar (Green) ➜ Refatorar o código (Refactor).
- **CancellationToken**: Sempre passar `CancellationToken.None` ou testar fluxos de cancelamento nos Handlers assíncronos.
- **Performance**: Todos os testes devem ser extremamente rápidos (rodando em poucos milissegundos).
