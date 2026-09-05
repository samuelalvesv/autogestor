---
name: csharp-conventions
description: Apply C# coding conventions, async/await guidelines, cancellation token propagation, memory optimizations, and static analysis.
applyTo: "**/*.{cs,razor}"
---

# Convenções de Código C#

> O arquivo `.editorconfig` na raiz do repositório é a fonte única de verdade para estilo de escrita de código, formatação, regras de chaves, espaçamento, nomenclatura e analisadores do Roslyn. Este documento especifica exclusivamente diretrizes arquiteturais, semânticas de design e decisões que o `.editorconfig` não é capaz de validar nativamente.

## Convenções Gerais

- Usar `record` para DTOs e Value Objects imutáveis.
- Usar `sealed class` por padrão; remover `sealed` apenas se herança for intencional.
- Nunca instanciar dependências com `new`; sempre usar injeção de dependência.
- **Programação Assíncrona**: Usar obrigatoriamente `async`/`await` de ponta a ponta para todas as operações de I/O e tarefas concorrentes.
- **Propagação de CancellationToken**: Métodos assíncronos devem configurar o token de cancelamento como parâmetro opcional com valor padrão (`default`). É estritamente proibida a invocação manual de `ThrowIfCancellationRequested()` em camadas de orquestração (use cases, serviços e repositórios), delegando a interrupção exclusivamente às operações nativas da BCL e do EF Core através da propagação direta do token.
- **Tratamento de Exceções Nativo**: O handler de exceções globais mencionado nos wrappers deve ser implementado utilizando a interface nativa `IExceptionHandler` (disponível a partir do .NET 8), evitando middlewares customizados pesados.
- **Performance de Alocação**: Em métodos assíncronos que possuem caminhos de execução síncronos frequentes (como checagem de cache ou validações em memória rápidos), preferir `ValueTask` ou `ValueTask<T>` ao invés de `Task` para reduzir alocações na Heap.
- **Tratamento de Data/Hora (UTC)**: É obrigatório instanciar e manipular valores de data e hora sempre em formato UTC na aplicação (ex: utilizando `DateTime.UtcNow`). Para regras de mapeamento de persistência, consultar o README da infraestrutura.
- **Ferramentas e Bibliotecas**: Adotar exclusivamente tecnologias open source consolidadas e amplamente validadas pela comunidade do .NET.
- **Desacoplamento e Reuso (Wrappers)**: Implementar **wrappers** de controle (como o padrão `Result<T>` para fluxos de negócio ou handlers de exceções globais) para evitar a repetição de lógica de tratamento de erro, logs e try-catchs em múltiplos endpoints.
- **Validação Estática em Tempo de Compilação**: Preferir sempre que possível a validação estática de código, detectando erros em tempo de compilação ao invés de em tempo de execução. Isso inclui: uso de tipos fortes ao invés de `string`/`object` genéricos, atributos de análise estática (`[NotNullWhen]`, `[MemberNotNull]`, `[StringSyntax]`), `const` e `readonly` para imutabilidade verificável pelo compilador, nullable reference types habilitados (`<Nullable>enable</Nullable>`), e warnings tratados como erros (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`) para impedir que avisos de análise sejam ignorados.
- **Primary Constructors e Injeção de Dependência**: Utilizar os parâmetros de *primary constructor* diretamente no corpo dos métodos da classe, sem criar campos privados extras nem verificações defensivas de nulidade para dependências resolvidas pelo container de injeção de dependência.
- **Sem Validações Defensivas Redundantes (Anti-Overengineering / YAGNI)**: Proibido adicionar verificações defensivas de nulidade quando essa garantia já for fornecida pelo pipeline do framework, pelo sistema de tipos ou por camadas precedentes. A validação de dados de negócio deve residir de forma única e centralizada na camada de Aplicação/Domínio, sem duplicações em camadas intermediárias.
- **Argumentos Nomeados Obrigatórios**: É estritamente obrigatório utilizar argumentos nomeados em todas as invocações de construtores, métodos, funções e instanciações sempre que a sintaxe da linguagem permitir essa opção. É proibido o uso de argumentos posicionais quando houver a possibilidade de passá-los de forma nomeada, maximizando a clareza semântica e prevenindo erros de inversão de parâmetros.

## Diretrizes de Otimização e Performance

- **Source Generators (Geração de Código no Build)**: É proibido o uso de reflexão em tempo de execução (`System.Reflection`).
  - Para serialização/desserialização JSON, utilizar obrigatoriamente **System.Text.Json Source Generation** configurando uma classe parcial que estende `JsonSerializerContext` com os atributos `[JsonSourceGenerationOptions]` e `[JsonSerializable]`.
  - Usar o atributo `[JsonConstructor]` para instruir explicitamente o compilador sobre qual construtor de record/classe imutável utilizar durante a desserialização.
  - Para expressões regulares estáticas, utilizar obrigatoriamente o atributo `[GeneratedRegex]` em métodos parciais.
- **Estruturas de Dados e Passagem por Referência**:
  - Utilizar `readonly struct` para criar tipos de valor imutáveis que não necessitam de alocações na Heap.
  - Ao passar structs grandes como argumentos de método para evitar a cópia de seus dados na Stack, utilizar o modificador de parâmetro `in` (passagem por referência somente leitura).
