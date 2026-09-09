---
name: ui-rules
description: Razor Class Library (RCL), MudBlazor styling, reusable components, and host-agnostic frontend guidelines.
applyTo: "src/Autogestor.UI/**/*.{cs,razor}"
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
  - Referencia apenas `Autogestor.Contract`. Nenhuma dependência direta com `Autogestor.Domain`.
  - Não referencia pacotes específicos de hosts (`Microsoft.AspNetCore.Components.WebAssembly` ou `Microsoft.Maui`).
  - Não referencia `Autogestor.ServiceDefaults`.
  - Acesso a APIs externas/nativas deve ser feito via **interfaces** com implementação registrada via injeção de dependência pelo host final.

## Ferramentas

- **Plugin `dotnet-blazor`**:
  - Skills `author-component`, `plan-ui-change`: Diretrizes de ciclo de vida de componentes Blazor, parametrização e decomposição de páginas ricas.
  - Skills `collect-user-input`, `coordinate-components`: Formulários, validações, binding de dados e compartilhamento de estado desacoplado.
  - Skills `fetch-and-send-data`, `support-prerendering`: Ciclo de carregamento assíncrono de contratos e prevenção de flicker no prerender.
  - Skill `use-js-interop`: Padrões seguros de interoperação com JavaScript e descarte assíncrono de referências.
- **Plugin `dotnet-data`**:
  - Skill `create-datadriven-aspnetcore`: Scaffolding de formulários, tabelas e visualizações orientadas a dados integradas aos contratos.
- **Submódulo `ponytail`**:
  - Skill `ponytail`: UI simples e direta, priorizando os recursos nativos do MudBlazor sem complexidade de estado acidental.
