# autogestor

Sistema de autogestão corporativa construído com Clean Architecture, DDD e .NET Aspire.

## Stack Tecnológica
- **Backend**: .NET 10 (C#)
- **Frontend**: Blazor WebAssembly (WASM) com MudBlazor
- **Orquestração & Resiliência**: .NET Aspire
- **Banco de Dados**: PostgreSQL 18
- **Testes**: xUnit

## Estrutura do Projeto
```
autogestor/
├── src/                              # Código-fonte de produção
│   ├── autogestor.core/              # Domain — Entidades, Value Objects, Interfaces
│   ├── autogestor.app/               # Application — Casos de uso, DTOs, Mediator
│   ├── autogestor.infra/             # Infrastructure — EF Core, Repositórios, Serviços
│   ├── autogestor.api/               # Presentation — Minimal API endpoints
│   ├── autogestor.web/               # Frontend — Blazor WASM + MudBlazor
│   ├── autogestor.AppHost/           # Aspire — Orquestrador de serviços
│   └── autogestor.ServiceDefaults/   # Aspire — Telemetria, Health Checks, Resiliência
├── test/                             # Código de testes
│   └── autogestor.test/              # xUnit — Unitários, Integração, Arquitetura
├── db/                               # Banco de dados
│   └── autogestor.db/                # Scripts SQL (PostgreSQL 18)
├── .agents/                          # Regras e documentação técnica para agentes
│   ├── AGENTS.md                     # Convenções de código e commit
│   └── architecture.md               # Mapa de dependências e camadas
├── autogestor.sln                    # Arquivo de solução .NET
├── .gitignore
└── README.md
```

## Como Iniciar e Desenvolver
| Objetivo | Comando |
|---|---|
| Compilar a Solução | `dotnet build` |
| Executar o Aspire (Ambiente Dev) | `dotnet run --project src/autogestor.AppHost` |
| Rodar a Suíte de Testes | `dotnet test` |
| Verificar/Atualizar Pacotes NuGet | `dotnet outdated -u` |

## Documentação Adicional
Para garantir consistência e evitar retrabalho, consulte os guias de design e regras do projeto:
- **Diretrizes para Desenvolvedores e IAs**: Convenções de código, regras de commit e TDD em [.agents/AGENTS.md](.agents/AGENTS.md).
- **Mapa da Arquitetura**: Fluxo de dependências e responsabilidades de cada camada em [.agents/architecture.md](.agents/architecture.md).

