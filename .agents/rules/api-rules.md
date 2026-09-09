---
name: api-rules
description: gRPC/gRPC-Web services, endpoints mapping, HTTP gateways, middlewares, and startup configuration.
applyTo: "src/Autogestor.Api/**/*.cs"
---

# Regras do Ponto de Entrada API (Autogestor.Api)

## Estrutura de Pastas
- `Services/`: Implementações de serviços gRPC (`[Feature]Service.cs`).
- `Middlewares/`: Interceptadores gRPC e handlers de exceção.
- `Extensions/`: Métodos de extensão de injeção de dependência e Kestrel.
- `Program.cs`: Composição raiz do projeto.

## Diretrizes gRPC e Configuração
- **Serviços gRPC**:
  - Implementam as interfaces do projeto `Autogestor.Contract`.
  - Recebem requisições fortemente tipadas, acionam a camada `Application` via MediatR (`_mediator.Send`) e retornam os DTOs do `Contract`.
  - Não devem conter lógica de negócios.
- **Configuração gRPC-Web**:
  - Habilitar suporte a gRPC-Web no `Program.cs` com `app.UseGrpcWeb()`.
  - Mapear serviços com `app.MapGrpcService<T>().EnableGrpcWeb()`.
- **Injeção de Dependências**: O `Program.cs` deste projeto é o único que conhece todas as camadas concretas da aplicação para poder compor o contêiner de DI.

## Ferramentas

- **Plugin `dotnet-aspnetcore`**:
  - Skills `dotnet-webapi`, `configuring-opentelemetry-dotnet`, `minimal-api-file-upload`: Padrões de semântica HTTP/gRPC, tratamento global de exceções, observabilidade OTLP e streaming seguro de arquivos.
- **Plugin `dotnet-diag`**:
  - Subagente `optimizing-dotnet-performance`: Profiling de throughput e latência de endpoints gRPC e middlewares.
  - Skill `analyzing-dotnet-performance`: Identificação de gargalos assíncronos e alocações no pipeline Kestrel e serialização.
- **Plugin `dotnet-data`**:
  - Skill `create-datadriven-aspnetcore`: Mapeamento rápido e estruturação de rotas e endpoints orientados a dados.
- **Plugin `dotnet-nuget`**:
  - Skill `convert-to-cpm`: Governança de versões centralizadas para dependências do projeto API.
- **Submódulo `ponytail`**:
  - Skill `ponytail`: Manter serviços gRPC estritamente finos (thin presentation), delegando orquestrações para Application.
