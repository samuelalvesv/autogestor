# Arquitetura — autogestor

## Visão Geral

Sistema construído com Clean Architecture + DDD, orquestrado pelo .NET Aspire.

## Mapa de Dependências

```text
Autogestor.Contracts       ← Nenhuma dependência (DTOs leves e interfaces gRPC)
Autogestor.Domain          ← Nenhuma dependência (camada mais interna e pura)
  ↑
Autogestor.Application     ← Domain, Contracts
  ↑
Autogestor.Infrastructure  ← Domain, Application
  ↑
Autogestor.Api             ← Domain, Application, Infrastructure, ServiceDefaults, Contracts
Autogestor.UI              ← Contracts (RCL — componentes Razor compartilhados)
  ↑
Autogestor.Web             ← UI, Contracts (host WASM + PWA — shell fino)
[Autogestor.Hybrid]        ← UI, Contracts (futuro host MAUI — apps nativos)
Autogestor.Tests           ← Domain, Application, Infrastructure, Api, UI, Web, Contracts
Autogestor.AppHost         ← Api, Web (orquestrador Aspire)
Autogestor.ServiceDefaults ← Nenhuma (biblioteca compartilhada de telemetria)
```

## Camadas e Responsabilidades

| Camada | Projeto | O que vai aqui | O que NÃO vai aqui |
| --- | --- | --- | --- |
| **Contracts** | `src/Autogestor.Contracts` | Interfaces de serviços gRPC com atributos `[ServiceContract]`, DTOs de request e response decorados com `[DataContract]` e ordens explícitas. | Lógica de negócios, Handlers, dependências de banco de dados, pacotes pesados de infraestrutura. |
| **Domain** | `src/Autogestor.Domain` | Entidades ricas, Value Objects, Enums de domínio, interfaces de repositório (`IXxxRepository`), Domain Events. | DTOs, frameworks, atributos de serialização ou pacotes de gRPC. |
| **Application** | `src/Autogestor.Application` | Use Cases / Handlers (Commands, Queries), mapeamento de DTOs do `Contracts` para o Domain, interfaces de serviços externos, validações de entrada. | Acesso a banco, serialização gRPC, Entity Framework. |
| **Infrastructure** | `src/Autogestor.Infrastructure` | Implementações de repositórios, DbContext (EF Core), clients HTTP de terceiros, serviços de e-mail/SMS. | Lógica de negócio, serviços gRPC. |
| **Presentation** | `src/Autogestor.Api` | Implementações de serviços gRPC concretos, configuração gRPC-Web, injeção de dependência global, middlewares. | Lógica de negócio, acesso direto a banco de dados. |
| **UI (RCL)** | `src/Autogestor.UI` | Componentes Razor compartilhados, páginas, layouts, CSS, consumo de serviços fortemente tipados do `Contracts` injetados via DI. | Pacotes de hosting (WASM/MAUI), ServiceDefaults, configuração física de canais de comunicação. |
| **Frontend Host** | `src/Autogestor.Web` | Bootstrap WASM, PWA, `App.razor`, `index.html`, registro do `GrpcChannel` com suporte a gRPC-Web (`GrpcWebHandler`), ativação do AOT. | Componentes Razor compartilhados, páginas, layouts (esses vivem na UI). |
| **Orchestration** | `src/Autogestor.AppHost` | Registro de projetos e recursos no Aspire (incluindo endpoints gRPC). | Lógica de negócio. |
| **Shared** | `src/Autogestor.ServiceDefaults` | OpenTelemetry, Health Checks, Resiliência HTTP/gRPC. | Lógica de negócio. |
| **Tests** | `test/Autogestor.Tests` | Testes unitários, de integração e testes de arquitetura automatizados (com `NetArchTest`) para validar as dependências das camadas. | Código de produção. |

## Fluxo de uma Requisição

```text
[Usuário] → Api (serviço gRPC) → Application (use case) → Domain (regra de negócio)
                                      ↓
                                  Infrastructure (persiste no banco / chama serviço externo)
```

## Estratégia de Frontend Multi-Plataforma

A UI é construída como uma Razor Class Library (`Autogestor.UI`) host-agnostic. Os componentes são consumidos por hosts diferentes conforme a necessidade:

```text
Autogestor.UI (RCL)          ← Componentes, páginas, layouts, CSS compartilhados
    ↑              ↑
Autogestor.Web     [Autogestor.Hybrid]
(Host WASM+PWA)    (Futuro Host MAUI — apps nativos iOS/Android/Windows/macOS)
```

Serviços que dependem de plataforma (ex: notificações, storage) são definidos como **interfaces** na RCL e implementados em cada host via DI.

## Estratégia gRPC-Web (Code-First) & AOT

Para garantir a melhor performance, facilidade de manutenção e integração nativa .NET:
1. **gRPC-Web:** Habilitado no backend (`Autogestor.Api`) para permitir que o Blazor WebAssembly (`Autogestor.Web`) faça chamadas RPC através das restrições do navegador. O futuro aplicativo MAUI (`Autogestor.Hybrid`) ignorará o gRPC-Web e se conectará diretamente usando gRPC nativo sobre HTTP/2 puro.
2. **Code-First:** Todos os contratos gRPC são definidos como interfaces C# clássicas e DTOs `record` dentro do projeto `Autogestor.Contracts`. Isso elimina o uso de arquivos `.proto` e garante tipagem forte e segurança em tempo de compilação de ponta a ponta.
3. **Compilação AOT (Ahead-Of-Time):** A publicação em produção do `Autogestor.Web` será realizada com compilação AOT ativa (`/p:RunAOTCompilation=true`). Isso compila o runtime e o serializador do Protobuf diretamente em instruções WebAssembly de máquina, maximizando o desempenho de CPU no navegador e eliminando latência de reflexão de tipos.

## Regra de Ouro
>
> A dependência SEMPRE aponta para dentro (em direção ao `Domain` e `Contracts`).
> O `Domain` nunca referencia nenhum outro projeto.

