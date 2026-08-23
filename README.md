# autogestor

Sistema SaaS de autogestão corporativa e financeira construído com Clean Architecture, Domain-Driven Design (DDD), .NET 10, gRPC-Web (Code-First), MudBlazor (WASM PWA) e orquestrado com .NET Aspire.

---

## 🚀 Stack Tecnológica

- **Backend**: .NET 10 (C# moderno)
- **Contratos & Comunicação**: gRPC Code-First (`protobuf-net.Grpc`) com suporte a gRPC-Web e AOT
- **UI Compartilhada**: Razor Class Library (RCL — `Autogestor.UI`) com MudBlazor
- **Frontend Host**: Blazor WebAssembly (WASM — `Autogestor.Web`) com suporte a PWA (shell fino) e preparado para MAUI (`Autogestor.Hybrid`)
- **Isolamento & Multi-Tenancy**: ASP.NET Identity + `TenantEntity` (Global Query Filters por `TenantId` no EF Core) e autorização granular por filiais (`IBranchAuthorizationService`)
- **Orquestração & Observabilidade**: .NET Aspire, OpenTelemetry (OTel), Health Checks e Resiliência HTTP/gRPC (`Autogestor.ServiceDefaults`)
- **Banco de Dados**: PostgreSQL 18 (Neon Serverless Postgres / EF Core com Npgsql e Naming Conventions snake_case)
- **Gerenciamento de Dependências**: NuGet Central Package Management (CPM com `Directory.Packages.props`)
- **Testes Automatizados**: xUnit, NetArchTest (ArchitectureTests), Coverlet (cobertura)

---

## 🏛️ Arquitetura e Mapa de Dependências

O projeto adota os princípios de Clean Architecture e DDD, garantindo que as regras de negócio permaneçam isoladas de detalhes de infraestrutura e frameworks.

### Mapa de Dependências

```text
Autogestor.Contract        ← Nenhuma dependência (DTOs leves e interfaces gRPC)
Autogestor.Domain          ← Nenhuma dependência (camada mais interna e pura)
  ↑
Autogestor.Application     ← Domain, Contract
  ↑
Autogestor.Infrastructure  ← Domain, Application
  ↑
Autogestor.Api             ← Domain, Application, Infrastructure, ServiceDefaults, Contract
Autogestor.UI              ← Contract (RCL — componentes Razor compartilhados)
  ↑
Autogestor.Web             ← UI, Contract (host WASM + PWA — shell fino)
[Autogestor.Hybrid]        ← UI, Contract (futuro host MAUI — apps nativos)
Autogestor.Tests           ← Domain, Application, Infrastructure, Api, UI, Web, Contract
Autogestor.AppHost         ← Api, Web (orquestrador Aspire)
Autogestor.ServiceDefaults ← Nenhuma (biblioteca compartilhada de telemetria)
```

### Regra de Ouro (Golden Rule)
> **A dependência SEMPRE aponta para dentro (em direção ao `Domain` e `Contract`).**  
> O `Domain` e o `Contract` nunca referenciam nenhum outro projeto ou framework pesado.

### Fluxo de uma Requisição

```text
[Usuário / WASM Client] 
       │ (gRPC-Web / HTTP/2)
       ▼
[Autogestor.Api] (Serviço gRPC / Interceptor de Tenant)
       │
       ▼
[Autogestor.Application] (Use Cases / Handlers / Validações)
       │
       ├───────────────► [Autogestor.Domain] (Entidades ricas & Invariantes de negócio)
       │
       ▼
[Autogestor.Infrastructure] (EF Core / Repositórios com Query Filters por TenantId)
       │
       ▼
[PostgreSQL 18 (Neon)]
```

---

## 📁 Estrutura do Projeto

```text
autogestor/
├── src/                                  # Código-fonte de produção
│   ├── Autogestor.Contract/              # DTOs, Requests, Responses e Interfaces gRPC Code-First
│   ├── Autogestor.Domain/                # Entidades ricas, Value Objects, Enums e Interfaces
│   ├── Autogestor.Application/           # Casos de uso (Use Cases), Handlers, DTOs e Validações
│   ├── Autogestor.Infrastructure/        # EF Core DbContext, Repositórios, Npgsql, Serviços Externos
│   ├── Autogestor.Api/                   # Serviços gRPC, gRPC-Web, DI Global, Middlewares
│   ├── Autogestor.UI/                    # UI (RCL) — Componentes Razor compartilhados e MudBlazor
│   ├── Autogestor.Web/                   # Frontend Host — Blazor WASM + PWA (shell fino)
│   ├── Autogestor.AppHost/               # Aspire — Orquestrador de serviços e recursos
│   └── Autogestor.ServiceDefaults/       # Aspire — Telemetria (OTel), Health Checks e Resiliência
├── test/                                 # Código de testes automatizados
│   ├── Autogestor.UnitTests/             # Testes unitários (Domain, Application, Contract)
│   ├── Autogestor.IntegrationTests/      # Testes de integração (Infraestrutura, Endpoints, Banco)
│   └── Autogestor.ArchitectureTests/     # Testes de arquitetura e fronteiras de camadas (NetArchTest)
├── db/                                   # Banco de dados nativo
│   ├── functions/                        # Funções SQL PostgreSQL
│   ├── procedures/                       # Procedures SQL
│   ├── scripts/                          # Scripts de migração e utilitários
│   ├── triggers/                         # Triggers SQL
│   └── views/                            # Views SQL
├── .agents/                              # Governança, regras e inteligência para agentes AI
│   ├── doc/                              # Documentação de customizações e capacidades do agente
│   ├── rules/                            # Regras de arquitetura, C#, testes, multi-tenancy e git
│   ├── scripts/                          # Scripts de validação e CI local (ex: verify-build.sh)
│   ├── skills/                           # Skills especializadas (ex: neon, code-review)
│   └── workflows/                        # Workflows automatizados (ex: propagate-domain.md)
├── AGENTS.md                             # Identidade do agente, convenções de código e diretrizes
├── Autogestor.slnx                       # Arquivo de solução .NET
├── Directory.Build.props                 # Configurações globais de compilação MSBuild
├── Directory.Packages.props              # Central Package Management (CPM) para NuGet
├── .gitignore
└── README.md
```

---

## 🛠️ Como Iniciar e Desenvolver

### Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Carga de trabalho do Aspire instalada: `dotnet workload install aspire`
- Docker ou Podman (para orquestração de containers via Aspire, opcional se usar Neon)

### Comandos Essenciais

| Objetivo | Comando |
| --- | --- |
| **Restaurar Dependências** | `dotnet restore Autogestor.slnx` |
| **Compilar a Solução** | `dotnet build Autogestor.slnx` |
| **Executar Aspire Dashboard (Dev)** | `dotnet run --project src/Autogestor.AppHost` |
| **Executar Todos os Testes** | `dotnet test Autogestor.slnx` |
| **Executar Testes Unitários** | `dotnet test test/Autogestor.UnitTests` |
| **Executar Testes de Integração** | `dotnet test test/Autogestor.IntegrationTests` |
| **Executar Testes de Arquitetura** | `dotnet test test/Autogestor.ArchitectureTests` |
| **Formatar Código (dotnet format)** | `dotnet format` |
| **Validar Build e Integridade** | `./.agents/scripts/verify-build.sh` |
| **Verificar/Atualizar Pacotes NuGet** | `dotnet outdated -u --pre-release Never` |

---

## 🔒 Multi-Tenancy & Segurança

O sistema adota isolamento em **dois níveis**:

1. **Nível 1 — Isolamento de Tenant (Automático)**:
   - Toda entidade derivada de `TenantEntity` possui filtro automático `HasQueryFilter(x => x.TenantId == currentTenantId)` configurado no `AppDbContext`.
   - Nenhuma query manual precisa se preocupar em filtrar por `TenantId`, prevenindo vazamento de dados entre empresas.

2. **Nível 2 — Controle de Acesso por Filial/Branch (Explícito)**:
   - Controlado no Application Layer via `IBranchAuthorizationService`.
   - Valida os vínculos de funcionários através da tabela associativa `UserBranchAccess` e perfis (`BranchRole`).

---

## 📚 Documentação Adicional

Para entender em detalhes os padrões do projeto, consulte a documentação técnica:

- **[AGENTS.md](AGENTS.md)**: Convenções de código C#, identidade do agente e regras fundamentais.
- **[Arquitetura](.agents/rules/architecture.md)**: Detalhamento de camadas, gRPC-Web Code-First e AOT.
- **[Identity & Multi-Tenancy](.agents/rules/identity-multitenancy.md)**: Modelo de Tenants, Branches e cardinalidades.
- **[Convenções C#](.agents/rules/csharp-conventions.md)**: Padrões de escrita em C# moderno.
- **[Git Flow](.agents/rules/git-flow.md)** e **[Git Commit](.agents/rules/git-commit.md)**: Estratégia de branches e convenções semânticas de commit.
