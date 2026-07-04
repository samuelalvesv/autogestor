# Arquitetura — autogestor

## Visão Geral
Sistema construído com Clean Architecture + DDD, orquestrado pelo .NET Aspire.

## Mapa de Dependências

```
autogestor.core          ← Nenhuma dependência (camada mais interna)
  ↑
autogestor.app           ← core
  ↑
autogestor.infra         ← core, app
  ↑
autogestor.api           ← core, app, infra, ServiceDefaults
autogestor.web           ← core (Blazor WebAssembly — roda no navegador)
autogestor.test          ← core, app, infra, api, web
autogestor.AppHost       ← api, web (orquestrador Aspire)
autogestor.ServiceDefaults ← Nenhuma (biblioteca compartilhada de telemetria)
```

## Camadas e Responsabilidades

| Camada | Projeto | O que vai aqui | O que NÃO vai aqui |
|---|---|---|---|
| **Domain** | `src/autogestor.core` | Entidades, Value Objects, Enums de domínio, interfaces de repositório (`IXxxRepository`), Domain Events | DTOs, frameworks, pacotes NuGet externos |
| **Application** | `src/autogestor.app` | Use Cases / Handlers (Commands, Queries), DTOs, interfaces de serviços externos (`IEmailService`), validações de entrada | Acesso a banco, HTTP, Entity Framework |
| **Infrastructure** | `src/autogestor.infra` | Implementações de repositórios, DbContext (EF Core), clients HTTP, serviços de e-mail/SMS | Lógica de negócio, Controllers |
| **Presentation** | `src/autogestor.api` | Minimal API endpoints, injeção de dependência, middlewares | Lógica de negócio, acesso direto a banco |
| **Frontend** | `src/autogestor.web` | Componentes Blazor WebAssembly, páginas, chamadas HTTP à API | Acesso a banco, ServiceDefaults |
| **Orchestration** | `src/autogestor.AppHost` | Registro de projetos e recursos no Aspire | Lógica de negócio |
| **Shared** | `src/autogestor.ServiceDefaults` | OpenTelemetry, Health Checks, Resiliência HTTP | Lógica de negócio |
| **Tests** | `test/autogestor.test` | Testes unitários, de integração e de arquitetura | Código de produção |

## Fluxo de uma Requisição

```
[Usuário] → api (endpoint) → app (use case) → core (regra de negócio)
                                  ↓
                              infra (persiste no banco / chama serviço externo)
```

## Regra de Ouro
> A dependência SEMPRE aponta para dentro (em direção ao `core`).
> O `core` nunca referencia nenhum outro projeto.
