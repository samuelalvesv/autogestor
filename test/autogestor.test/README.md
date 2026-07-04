# autogestor.test

Projeto de testes da solução. Ver [architecture.md](../../.agents/architecture.md) para o mapa completo.

## Estrutura de Pastas Esperada
```
autogestor.test/
├── Unit/
│   ├── Core/          # Testes de entidades e value objects
│   ├── App/           # Testes de casos de uso (com mocks)
│   └── Api/           # Testes de endpoints
├── Integration/       # Testes com banco de dados real ou em memória
└── Architecture/      # Testes de regras arquiteturais (dependências entre camadas)
```

## Convenções
- Nome do teste: `MethodName_Scenario_ExpectedResult`
- Padrão: Arrange → Act → Assert
- Framework: xUnit
- Usar `Moq` ou `NSubstitute` para mocks.
