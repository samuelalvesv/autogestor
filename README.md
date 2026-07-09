# autogestor

Sistema de autogestão corporativa construído com Clean Architecture, DDD e .NET Aspire.

## Stack Tecnológica

- **Backend**: .NET 10 (C#)
- **UI Compartilhada**: Razor Class Library (RCL) com MudBlazor
- **Frontend Host**: Blazor WebAssembly (WASM) com PWA
- **Orquestração & Resiliência**: .NET Aspire
- **Banco de Dados**: PostgreSQL 18
- **Testes**: xUnit

## Estrutura do Projeto

```text
autogestor/
├── src/                              # Código-fonte de produção
│   ├── Autogestor.Domain/            # Domain — Entidades, Value Objects, Interfaces
│   ├── Autogestor.Application/       # Application — Casos de uso, DTOs, Mediator
│   ├── Autogestor.Infrastructure/    # Infrastructure — EF Core, Repositórios, Serviços
│   ├── Autogestor.Api/               # Presentation — Minimal API endpoints
│   ├── Autogestor.UI/                # UI (RCL) — Componentes Razor compartilhados
│   ├── Autogestor.Web/               # Frontend Host — Blazor WASM + PWA (shell fino)
│   ├── Autogestor.AppHost/           # Aspire — Orquestrador de serviços
│   └── Autogestor.ServiceDefaults/   # Aspire — Telemetria, Health Checks, Resiliência
├── test/                             # Código de testes
│   └── Autogestor.Tests/             # xUnit — Unitários, Integração, Arquitetura
├── db/                               # Banco de dados
│   └── Autogestor.Db/                # Scripts SQL (PostgreSQL 18)
├── .agents/                          # Regras e documentação técnica para agentes
│   ├── AGENTS.md                     # Convenções de código e commit
│   └── architecture.md               # Mapa de dependências e camadas
├── Autogestor.sln                    # Arquivo de solução .NET
├── .gitignore
└── README.md
```

## Como Iniciar e Desenvolver

| Objetivo | Comando |
| --- | --- |
| Compilar a Solução | `dotnet build` |
| Executar o Aspire (Ambiente Dev) | `dotnet run --project src/Autogestor.AppHost` |
| Rodar a Suíte de Testes | `dotnet test` |
| Verificar/Atualizar Pacotes NuGet | `dotnet outdated -u` |

## Documentação Adicional

Para garantir consistência e evitar retrabalho, consulte os guias de design e regras do projeto:

- **Diretrizes para Desenvolvedores e IAs**: Convenções de código, regras de commit e TDD em [.agents/AGENTS.md](.agents/AGENTS.md).
- **Mapa da Arquitetura**: Fluxo de dependências e responsabilidades de cada camada em [.agents/architecture.md](.agents/architecture.md).
