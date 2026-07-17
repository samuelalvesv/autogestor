---
name: csharp-conventions
description: Apply C# coding conventions, async/await guidelines, cancellation token propagation, memory optimizations, and static analysis.
applyTo: "**/*.cs, **/*.razor"
---

# Convenções de Código C#

## Convenções Gerais

- Usar `record` para DTOs e Value Objects imutáveis.
- Usar `sealed class` por padrão; remover `sealed` apenas se herança for intencional.
- Nunca instanciar dependências com `new`; sempre usar injeção de dependência.
- Nomes de interfaces começam com `I` (ex: `IOrderRepository`).
- **Programação Assíncrona**: Usar obrigatoriamente `async`/`await` de ponta a ponta para todas as operações de I/O e tarefas concorrentes.
- **Propagação de Cancelamento**: Todos os métodos assíncronos (Endpoints da API, Request Handlers e chamadas de I/O no banco/HTTP) devem obrigatoriamente aceitar e propagar um `CancellationToken` para evitar retenção de recursos no servidor.
- **Tratamento de Exceções Nativo**: O handler de exceções globais mencionado nos wrappers deve ser implementado utilizando a interface nativa `IExceptionHandler` (disponível a partir do .NET 8), evitando middlewares customizados pesados.
- **Performance de Alocação**: Em métodos assíncronos que possuem caminhos de execução síncronos frequentes (como checagem de cache ou validações em memória rápidos), preferir `ValueTask` ou `ValueTask<T>` ao invés de `Task` para reduzir alocações na Heap.
- **Tratamento de Data/Hora (UTC)**: É obrigatório instanciar e manipular valores de data e hora sempre em formato UTC na aplicação (ex: utilizando `DateTime.UtcNow`). Para regras de mapeamento de persistência, consultar o README da infraestrutura.
- **Ferramentas e Bibliotecas**: Adotar exclusivamente tecnologias open source consolidadas e amplamente validadas pela comunidade do .NET.
- **Desacoplamento e Reuso (Wrappers)**:
  - Implementar **wrappers** de controle (como o padrão `Result<T>` para fluxos de negócio ou handlers de exceções globais) para evitar a repetição de lógica de tratamento de erro, logs e try-catchs em múltiplos endpoints.
- **Validação Estática em Tempo de Compilação**: Preferir sempre que possível a validação estática de código, detectando erros em tempo de compilação ao invés de em tempo de execução. Isso inclui: uso de tipos fortes ao invés de `string`/`object` genéricos, atributos de análise estática (`[NotNullWhen]`, `[MemberNotNull]`, `[StringSyntax]`), `const` e `readonly` para imutabilidade verificável pelo compilador, nullable reference types habilitados (`<Nullable>enable</Nullable>`), e warnings tratados como erros (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`) para impedir que avisos de análise sejam ignorados.
- **Qualidade de Código e Roslyn**: Forçar padrões rígidos de qualidade, estilo de escrita e formatação de código C# utilizando analisadores do Roslyn configurados via arquivo `.editorconfig` na raiz da solução.

## Diretrizes de Otimização e Performance

- **Source Generators (Geração de Código no Build)**: É proibido o uso de reflexão em tempo de execução (`System.Reflection`).
  - Para serialização/desserialização JSON, utilizar obrigatoriamente **System.Text.Json Source Generation** configurando uma classe parcial que estende `JsonSerializerContext` com os atributos `[JsonSourceGenerationOptions]` e `[JsonSerializable]`.
  - Usar o atributo `[JsonConstructor]` para instruir explicitamente o compilador sobre qual construtor de record/classe imutável utilizar durante a desserialização.
  - Para expressões regulares estáticas, utilizar obrigatoriamente o atributo `[GeneratedRegex]` em métodos parciais.
- **Estruturas de Dados e Passagem por Referência**:
  - Utilizar `readonly struct` para criar tipos de valor imutáveis que não necessitam de alocações na Heap.
  - Ao passar structs grandes como argumentos de método para evitar a cópia de seus dados na Stack, utilizar o modificador de parâmetro `in` (passagem por referência somente leitura).
- **Desvirtualização e Inlining**:
  - Utilizar `sealed class` por padrão para permitir que o compilador JIT realize desvirtualização de chamadas e otimizações de *inlining* agressivas.
