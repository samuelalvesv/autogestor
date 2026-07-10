# Autogestor.Api (Presentation)

Ponto de entrada HTTP do sistema. Ver [architecture.md](../../.agents/architecture.md) para o mapa completo.

## Estrutura de Pastas Esperada

```text
Autogestor.Api/
├── Services/                   # Implementações dos serviços gRPC (ICategoryGrpcService, etc.)
│   └── [Feature]GrpcService.cs # Classes que implementam as interfaces do Autogestor.Contracts
├── Middlewares/                 # Interceptadores gRPC e Middlewares customizados (ex: ExceptionHandler)
├── Extensions/                 # Métodos de extensão para configuração (DI, Kestrel)
└── Program.cs                  # Composição raiz (DI, middlewares gRPC, registro de serviços gRPC-Web)
```

## Regras Específicas

- Usar gRPC e gRPC-Web.
- No `Program.cs`, registrar o gRPC-Web e expor os serviços usando `app.UseGrpcWeb()` e `app.MapGrpcService<T>().EnableGrpcWeb()`.
- Os serviços gRPC apenas recebem a requisição fortemente tipada, chamam o caso de uso correspondente (Command/Query) na camada `Autogestor.Application` (via MediatR) e retornam o DTO de resposta do `Autogestor.Contracts`.
- Nenhuma lógica de negócio deve existir aqui.
- O `Program.cs` é o único local que conhece todas as camadas para montar a injeção de dependência.
