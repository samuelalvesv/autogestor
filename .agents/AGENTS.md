# Regras do Agente — autogestor

## Idioma

- Todo código (classes, variáveis, métodos, comentários) deve ser escrito em **inglês**.
- Mensagens de commit, documentação de negócio e comunicação com o usuário devem ser em **português brasileiro**.

## Convenções de Código

- Usar `record` para DTOs e Value Objects imutáveis.
- Usar `sealed class` por padrão; remover `sealed` apenas se herança for intencional.
- Nunca instanciar dependências com `new`; sempre usar injeção de dependência.
- Nomes de interfaces começam com `I` (ex: `IOrderRepository`).
- **Programação Assíncrona**: Usar obrigatoriamente `async`/`await` de ponta a ponta para todas as operações de I/O e tarefas concorrentes.
- **Propagação de Cancelamento**: Todos os métodos assíncronos (Endpoints da API, Request Handlers e chamadas de I/O no banco/HTTP) devem obrigatoriamente aceitar e propagar um `CancellationToken` para evitar retenção de recursos no servidor.
- **Tratamento de Exceções Nativo**: O handler de exceções globais mencionado nos wrappers deve ser implementado utilizando a interface nativa `IExceptionHandler` (disponível a partir do .NET 8), evitando middlewares customizados pesados.
- **Performance de Alocação**: Em métodos assíncronos que possuem caminhos de execução síncronos frequentes (como checagem de cache ou validações em memória rápidos), preferir `ValueTask` ou `ValueTask<T>` ao invés de `Task` para reduzir alocações na Heap.
- **Ferramentas e Bibliotecas**: Adotar exclusivamente tecnologias open source consolidadas e amplamente validadas pela comunidade do .NET.
- **Desacoplamento e Reuso (Wrappers)**:
  - Implementar **wrappers** de controle (como o padrão `Result<T>` para fluxos de negócio ou handlers de exceções globais) para evitar a repetição de lógica de tratamento de erro, logs e try-catchs em múltiplos endpoints.
- **Qualidade de Código e Roslyn**: Forçar padrões rígidos de qualidade, estilo de escrita e formatação de código C# utilizando analisadores do Roslyn configurados via arquivo `.editorconfig` na raiz da solução.

## Testes e TDD (Test-Driven Development)

- Aplicar TDD para regras de negócio (`Domain`) e casos de uso (`Application`).
- Ciclo obrigatório: Escrever o teste (Red) ➜ Implementar o mínimo (Green) ➜ Refatorar (Refactor).
- Usar mocks (NSubstitute/Moq) para isolar as dependências do `Application`.
- Evitar TDD para UI (Blazor/Web) e configurações de infraestrutura.

## Estrutura de Pastas e Documentação

- **READMEs Locais**: Cada projeto/camada dentro de `src/` e `test/` possui seu próprio arquivo `README.md` contendo as regras de desenvolvimento específicas e a estrutura esperada do projeto. O agente deve consultar o `README.md` correspondente à pasta/projeto onde as alterações serão efetuadas antes de realizar qualquer modificação.
- Consultar `.agents/architecture.md` para o mapa completo de camadas e dependências.
- Consultar `src/Autogestor.Infrastructure/README.md` para regras específicas de persistência e Entity Framework Core.
- Consultar `src/Autogestor.ServiceDefaults/README.md` para regras específicas de observabilidade e resiliência com .NET Aspire.
- **Evitar duplicação**: Não repetir informações entre os arquivos `README.md` ou nos arquivos `.md` na pasta `.agents/`. A informação deve ser escrita em um único local centralizado e apenas referenciada nos outros locais.
- **Definições Assertivas**: Ao redigir documentações, nunca apresente opções ou caminhos alternativos. Defina sempre uma única abordagem padrão, escolhendo a solução mais moderna e aplicável ao projeto.
- **Evolução das Regras**: Os arquivos `.md` do projeto definem regras, mas há flexibilidade para sugerir alterações nelas durante o desenvolvimento, desde que a mudança traga benefícios para o projeto.


## Regras de Commit

- Nunca realize o commit diretamente, não prepare alterações no Git por conta própria e não envie a mensagem de commit sem solicitação do usuário.
- Quando o usuário solicitar "me dê a mensagem de commit" (ou similar), o agente deve verificar todos os arquivos preparados (staged) para commit e definir a mensagem de commit de acordo com essas mudanças preparadas.
- Formato: `tipo: descrição curta em português`
- Tipos: `feat`, `fix`, `refactor`, `docs`, `test`, `chore`
