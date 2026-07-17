---
name: api-rules
description: gRPC/gRPC-Web services, endpoints mapping, HTTP gateways, middlewares, and startup configuration.
applyTo: "src/Autogestor.Api/**/*.cs"
---

# Regras do Ponto de Entrada API (Autogestor.Api)

## Estrutura de Pastas
- `Services/`: Implementações de serviços gRPC (`[Feature]GrpcService.cs`).
- `Middlewares/`: Interceptadores gRPC e handlers de exceção.
- `Extensions/`: Métodos de extensão de injeção de dependência e Kestrel.
- `Program.cs`: Composição raiz do projeto.

## Diretrizes gRPC e Configuração
- **Serviços gRPC**:
  - Implementam as interfaces do projeto `Autogestor.Contracts`.
  - Recebem requisições fortemente tipadas, acionam a camada `Application` via MediatR (`_mediator.Send`) e retornam os DTOs do `Contracts`.
  - Não devem conter lógica de negócios.
- **Configuração gRPC-Web**:
  - Habilitar suporte a gRPC-Web no `Program.cs` com `app.UseGrpcWeb()`.
  - Mapear serviços com `app.MapGrpcService<T>().EnableGrpcWeb()`.
- **Injeção de Dependências**: O `Program.cs` deste projeto é o único que conhece todas as camadas concretas da aplicação para poder compor o contêiner de DI.
