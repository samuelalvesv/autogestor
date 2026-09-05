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
- **Princípio YAGNI e Eficiência de Testes**: É estritamente proibido criar testes unitários para validar comportamentos, restrições ou garantias nativas da linguagem C#, do compilador, do sistema de tipos ou do container de injeção de dependência. Todo teste deve validar exclusivamente comportamento observável de negócio, invariantes de domínio, orquestração de casos de uso e cenários de borda com relevância funcional.
- **Convenção de Nomenclatura e Inicialização de Dublês**:
  - Dublês de teste manuais em memória devem utilizar obrigatoriamente o sufixo `Fake`.
  - Dublês manuais devem ser inicializados em estado funcional e válido por padrão, assegurando que o contexto represente uma execução legítima e prevenindo a necessidade de código defensivo artificial no código de produção.
- **Escopo Exclusivo em Memória**: Toda validação unitária de casos de uso, lógica de domínio e serviços de apresentação isolados via dublês deve residir obrigatoriamente neste projeto.
- **Performance**: Todos os testes devem ser extremamente rápidos (rodando em poucos milissegundos).
