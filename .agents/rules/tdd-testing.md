---
name: tdd-testing
description: Apply Test-Driven Development (TDD) cycle (Red-Green-Refactor), mocking, and test suite conventions.
applyTo: "test/**/*.cs"
---

# Testes e TDD (Test-Driven Development)

- Aplicar TDD para regras de negócio (`Domain`) e casos de uso (`Application`).
- Ciclo obrigatório: Escrever o teste (Red) ➜ Implementar o mínimo (Green) ➜ Refatorar (Refactor).
- Usar mocks (NSubstitute/Moq) para isolar as dependências do `Application`.
- Evitar TDD para UI (Blazor/Web) e configurações de infraestrutura.
