# Autogestor.Web (Frontend)

Frontend Blazor WebAssembly (roda no navegador do usuário). Ver [architecture.md](../../.agents/architecture.md) para o mapa completo.

## Estrutura de Pastas Esperada
```
Autogestor.Web/
├── Pages/             # Páginas roteáveis (@page)
├── Components/        # Componentes UI reutilizáveis e customizados
├── Layout/            # Layout principal e navegação
├── Services/          # Clientes HTTP para consumir a API
├── Theme/             # Definição e configuração de cores e fontes do MudBlazor
└── wwwroot/           # Assets estáticos (CSS, imagens, manifest)
```

## Regras Específicas
- Referencia apenas `Autogestor.Domain` (para DTOs/modelos compartilhados).
- Não referencia `ServiceDefaults` (incompatível com o runtime `browser-wasm`).
- Toda comunicação com o backend é feita via HTTP (`HttpClient`) apontando para a `Autogestor.Api`.

## Diretrizes de Interface e Design
- **Biblioteca de Componentes**: Usar **MudBlazor** para a construção da interface do usuário (UI).
- **Componentes Reutilizáveis**: Todo elemento visual comum (botões estilizados, diálogos, tabelas de listagem, cartões de métricas) deve ser extraído como um componente em `Components/` para evitar duplicação de marcação.
- **Cores e Estilos Centralizados**:
  - A paleta de cores (primária, secundária, fundos), tipografia e bordas deve ser definida exclusivamente via `MudTheme` centralizado.
  - Proibido o uso de estilos inline (`style="..."`) ou cores hardcoded no CSS/HTML. Toda customização visual deve referenciar os tokens do tema do MudBlazor.
