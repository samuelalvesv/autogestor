---
name: ui-rules
description: Razor Class Library (RCL), MudBlazor styling, reusable components, and host-agnostic frontend guidelines.
applyTo: "src/Autogestor.UI/**/*.cs, src/Autogestor.UI/**/*.razor"
---

# Regras de UI / Razor Class Library (Autogestor.UI)

## Estrutura de Pastas
- `Pages/`: Páginas roteáveis da aplicação (`@page`).
- `Components/`: Componentes UI reutilizáveis (diálogos, botões, tabelas).
- `Layout/`: Layout principal e navegação.
- `Services/`: Interfaces de serviços específicos de UI.
- `Theme/`: Configuração de cores e tipografia do MudBlazor.
- `wwwroot/`: Assets estáticos compartilhados.

## Diretrizes de Interface e Design
- **MudBlazor**: Toda a interface é construída com MudBlazor.
- **Componentes Reutilizáveis**: Elementos visuais recorrentes devem ser componentes independentes em `Components/`.
- **Cores e Estilos Centralizados**:
  - Estilos são definidos via `MudTheme` central.
  - Proibido usar estilos inline (`style="..."`) ou cores hardcoded no CSS/HTML. Referenciar sempre os tokens do MudTheme.
- **Host-Agnostic**:
  - Referencia apenas `Autogestor.Contracts`. Nenhuma dependência direta com `Autogestor.Domain`.
  - Não referencia pacotes específicos de hosts (`Microsoft.AspNetCore.Components.WebAssembly` ou `Microsoft.Maui`).
  - Não referencia `Autogestor.ServiceDefaults`.
  - Acesso a APIs externas/nativas deve ser feito via **interfaces** com implementação registrada via injeção de dependência pelo host final.
