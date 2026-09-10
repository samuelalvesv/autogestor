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

- **Plugin `dotnet-aspnetcore`**: skills `dotnet-webapi`, `configuring-opentelemetry-dotnet`, `minimal-api-file-upload`
- **Plugin `dotnet-diag`**: subagente `optimizing-dotnet-performance`, skill `analyzing-dotnet-performance`
- **Plugin `dotnet-data`**: skill `create-datadriven-aspnetcore`
- **Plugin `dotnet-nuget`**: skill `convert-to-cpm`
- **Submódulo `ponytail`**: skill `ponytail`
