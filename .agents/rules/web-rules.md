---
name: web-rules
description: Blazor WebAssembly host configuration, PWA assets, service-worker, gRPC-Web channel connection, and AOT compilation.
applyTo: "src/Autogestor.Web/**/*.{cs,razor}"
---

# Regras do Host Web (Autogestor.Web)

## Estrutura de Pastas
- `App.razor`: Roteador Blazor referenciando a assembleia da RCL.
- `Program.cs`: Bootstrap do WASM e injeção de serviços locais.
- `wwwroot/`: index.html, manifest PWA, service workers e assets exclusivos da Web.

## Diretrizes do Host Web
- **Bootstrap e Configuração**:
  - Referencia `Autogestor.UI` e `Autogestor.Contract`.
  - Não contém páginas ou componentes (estes residem na RCL).
  - Configura o `GrpcChannel` com `GrpcWebHandler` para viabilizar chamadas gRPC-Web a partir do browser.
  - Registra no DI as implementações web concretas das interfaces exigidas pela RCL.
- **Compilação AOT (Ahead-Of-Time)**:
  - A publicação em produção deve ter `<RunAOTCompilation>true</RunAOTCompilation>` no `.csproj` para otimizar os serializadores de Protobuf e evitar reflexão pesada no browser.

## Ferramentas

- **Plugin `dotnet-msbuild`**:
  - Subagentes `build-perf`, `msbuild`: Diagnóstico e otimização dos tempos de compilação Ahead-of-Time (AOT), linking e dependências WebAssembly.
  - Servidor MCP `binlog`: Inspeção analítica das tarefas e tempos de compilação AOT e empacotamento do WebAssembly.
- **Plugin `dotnet-blazor`**:
  - Skills `author-component`, `support-prerendering`, `use-js-interop`: Estruturação do shell `App.razor`, inicialização limpa sem flicker e suporte a PWA.
- **Plugin `dotnet-upgrade`**:
  - Skill `dotnet-aot-compat`: Diagnóstico e resolução de avisos de reflexão dinâmica, compatibilidade e trimming para compilação WebAssembly AOT.
- **Plugin `dotnet-aspnetcore`**:
  - Skill `convert-blazor-server-to-webapp`: Padrões de hospedagem e arquitetura moderna para Blazor WebAssembly no ecossistema .NET 10.
- **Plugin `dotnet-diag`**:
  - Skill `analyzing-dotnet-performance`: Otimização de inicialização, minimização de alocação de heap e redução do payload WASM.
