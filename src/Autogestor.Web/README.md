# Autogestor.Web (Host WASM + PWA)

Host Blazor WebAssembly que serve a aplicação no browser com suporte a PWA. Consome os componentes da RCL `Autogestor.UI`. Ver [architecture.md](../../.agents/architecture.md) para o mapa completo.

## Estrutura de Pastas Esperada

```text
Autogestor.Web/
├── App.razor          # Router com referência às assemblies da RCL
├── Program.cs         # Bootstrap WASM + registro de serviços
├── _Imports.razor     # Imports globais (inclui namespaces da RCL)
└── wwwroot/           # Assets exclusivos do host web
    ├── index.html     # HTML host do WASM
    ├── manifest.webmanifest  # Manifest PWA
    ├── service-worker.js     # Service Worker (dev)
    ├── service-worker.published.js  # Service Worker (produção)
    ├── favicon.png, icon-*.png      # Ícones PWA
    └── lib/           # Bibliotecas CSS de terceiros (Bootstrap)
```

## Regras Específicas

- Referencia `Autogestor.UI` (a RCL) e `Autogestor.Contracts` (para obter as assinaturas e DTOs gRPC).
- **Não contém** páginas, componentes ou layouts — esses vivem na `Autogestor.UI`.
- Responsável pelo bootstrap WASM (`Program.cs`), PWA (Service Worker, manifest) e configuração do **canal gRPC** (`GrpcChannel` encapsulado com `GrpcWebHandler` para permitir chamadas gRPC-Web no browser).
- Registra no DI as implementações web-specific das interfaces de serviços e clientes gRPC do `Autogestor.Contracts`.
- **Compilação AOT (Ahead-Of-Time)**: A publicação para produção do host Web deve obrigatoriamente habilitar o AOT (`RunAOTCompilation = true`) no arquivo `.csproj` ou comando CLI para compilar os serializadores binários de Protobuf diretamente em instruções WebAssembly de máquina, evitando gargalos de CPU/reflexão.

