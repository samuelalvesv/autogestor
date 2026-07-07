# Autogestor.UI (Razor Class Library)

Biblioteca de componentes Razor compartilhados que podem ser consumidos por múltiplos hosts (Blazor WASM, MAUI Hybrid). Ver [architecture.md](../../.agents/architecture.md) para o mapa completo.

## Estrutura de Pastas Esperada

```text
Autogestor.UI/
├── Pages/             # Páginas roteáveis (@page)
├── Components/        # Componentes UI reutilizáveis e customizados
├── Layout/            # Layout principal e navegação
├── Services/          # Interfaces de serviços de UI (ex: INotificationService)
├── Theme/             # Definição e configuração de cores e fontes do MudBlazor
└── wwwroot/           # Assets estáticos compartilhados (CSS, fontes)
```

## Regras Específicas

- Referencia apenas `Autogestor.Domain` (para DTOs/modelos compartilhados).
- **Não referencia** pacotes de hosting (`Microsoft.AspNetCore.Components.WebAssembly` ou `Microsoft.Maui`).
- **Não referencia** `Autogestor.ServiceDefaults` (incompatível com o runtime `browser-wasm`).
- Todo acesso a APIs do browser ou do device nativo deve ser feito via **interfaces** (definidas aqui) com implementações injetadas pelo host via DI.
- Componentes devem ser **host-agnostic**: não usar APIs exclusivas do browser (ex: `IJSRuntime` para localStorage) diretamente. Criar abstrações.

## Diretrizes de Interface e Design

- **Biblioteca de Componentes**: Usar **MudBlazor** para a construção da interface do usuário (UI).
- **Componentes Reutilizáveis**: Todo elemento visual comum (botões estilizados, diálogos, tabelas de listagem, cartões de métricas) deve ser extraído como um componente em `Components/` para evitar duplicação de marcação.
- **Cores e Estilos Centralizados**:
  - A paleta de cores (primária, secundária, fundos), tipografia e bordas deve ser definida exclusivamente via `MudTheme` centralizado.
  - Proibido o uso de estilos inline (`style="..."`) ou cores hardcoded no CSS/HTML. Toda customização visual deve referenciar os tokens do tema do MudBlazor.

## Consumo pelos Hosts

### Blazor WASM (`Autogestor.Web`)
O host referencia este projeto e configura o `Router` com `AdditionalAssemblies` para descobrir as rotas. Assets estáticos são acessados via `_content/Autogestor.UI/`.

### MAUI Hybrid (`Autogestor.Hybrid` — futuro)
O host MAUI referencia este projeto e renderiza os componentes dentro de um `BlazorWebView`. Implementações nativas de interfaces (ex: `INotificationService`) são registradas no DI do MAUI.
