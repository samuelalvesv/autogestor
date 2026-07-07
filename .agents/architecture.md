# Arquitetura — autogestor

## Visão Geral

Sistema construído com Clean Architecture + DDD, orquestrado pelo .NET Aspire.

## Mapa de Dependências

```text
Autogestor.Domain          ← Nenhuma dependência (camada mais interna)
  ↑
Autogestor.Application     ← Domain
  ↑
Autogestor.Infrastructure  ← Domain, Application
  ↑
Autogestor.Api             ← Domain, Application, Infrastructure, ServiceDefaults
Autogestor.Web             ← Domain (Blazor WebAssembly — roda no navegador)
Autogestor.Tests           ← Domain, Application, Infrastructure, Api, Web
Autogestor.AppHost         ← Api, Web (orquestrador Aspire)
Autogestor.ServiceDefaults  ← Nenhuma (biblioteca compartilhada de telemetria)
```text

## Camadas e Responsabilidades

| Camada | Projeto | O que vai aqui | O que NÃO vai aqui |
| --- | --- | --- | --- |
| **Domain** | `src/Autogestor.Domain` | Entidades, Value Objects, Enums de domínio, interfaces de repositório (`IXxxRepository`), Domain Events | DTOs, frameworks, pacotes NuGet externos |
| **Application** | `src/Autogestor.Application` | Use Cases / Handlers (Commands, Queries), DTOs, interfaces de serviços externos (`IEmailService`), validações de entrada | Acesso a banco, HTTP, Entity Framework |
| **Infrastructure** | `src/Autogestor.Infrastructure` | Implementações de repositórios, DbContext (EF Core), clients HTTP, serviços de e-mail/SMS | Lógica de negócio, Controllers |
| **Presentation** | `src/Autogestor.Api` | Minimal API endpoints, injeção de dependência, middlewares | Lógica de negócio, acesso direto a banco |
| **Frontend** | `src/Autogestor.Web` | Componentes Blazor WebAssembly, páginas, chamadas HTTP à API | Acesso a banco, ServiceDefaults, injeção direta de `IXxxRepository` |
| **Orchestration** | `src/Autogestor.AppHost` | Registro de projetos e recursos no Aspire | Lógica de negócio |
| **Shared** | `src/Autogestor.ServiceDefaults` | OpenTelemetry, Health Checks, Resiliência HTTP | Lógica de negócio |
| **Tests** | `test/Autogestor.Tests` | Testes unitários, de integração e testes de arquitetura automatizados (com `NetArchTest`) para validar as dependências das camadas | Código de produção |

## Fluxo de uma Requisição

```text
[Usuário] → Api (endpoint) → Application (use case) → Domain (regra de negócio)
                                  ↓
                              Infrastructure (persiste no banco / chama serviço externo)
```text

## Regra de Ouro
>
> A dependência SEMPRE aponta para dentro (em direção ao `Domain`).
> O `Domain` nunca referencia nenhum outro projeto.
