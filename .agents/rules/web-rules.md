---
name: web-rules
description: Blazor WebAssembly host configuration, PWA assets, service-worker, gRPC-Web channel connection, and AOT compilation.
applyTo: "src/Autogestor.Web/**/*.cs, src/Autogestor.Web/**/*.razor"
---

# Regras do Host Web (Autogestor.Web)

## Estrutura de Pastas
- `App.razor`: Roteador Blazor referenciando a assembleia da RCL.
- `Program.cs`: Bootstrap do WASM e injeção de serviços locais.
- `wwwroot/`: index.html, manifest PWA, service workers e assets exclusivos da Web.

## Diretrizes do Host Web
- **Bootstrap e Configuração**:
  - Referencia `Autogestor.UI` e `Autogestor.Contracts`.
  - Não contém páginas ou componentes (estes residem na RCL).
  - Configura o `GrpcChannel` com `GrpcWebHandler` para viabilizar chamadas gRPC-Web a partir do browser.
  - Registra no DI as implementações web concretas das interfaces exigidas pela RCL.
- **Compilação AOT (Ahead-Of-Time)**:
  - A publicação em produção deve ter `<RunAOTCompilation>true</RunAOTCompilation>` no `.csproj` para otimizar os serializadores de Protobuf e evitar reflexão pesada no browser.
