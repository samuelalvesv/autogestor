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
- **Tratamento de Data/Hora (UTC)**: É obrigatório instanciar e manipular valores de data e hora sempre em formato UTC na aplicação (ex: utilizando `DateTime.UtcNow`). Para regras de mapeamento de persistência, consultar o README da infraestrutura.
- **Ferramentas e Bibliotecas**: Adotar exclusivamente tecnologias open source consolidadas e amplamente validadas pela comunidade do .NET.
- **Desacoplamento e Reuso (Wrappers)**:
  - Implementar **wrappers** de controle (como o padrão `Result<T>` para fluxos de negócio ou handlers de exceções globais) para evitar a repetição de lógica de tratamento de erro, logs e try-catchs em múltiplos endpoints.
- **Validação Estática em Tempo de Compilação**: Preferir sempre que possível a validação estática de código, detectando erros em tempo de compilação ao invés de em tempo de execução. Isso inclui: uso de tipos fortes ao invés de `string`/`object` genéricos, atributos de análise estática (`[NotNullWhen]`, `[MemberNotNull]`, `[StringSyntax]`), `const` e `readonly` para imutabilidade verificável pelo compilador, nullable reference types habilitados (`<Nullable>enable</Nullable>`), e warnings tratados como erros (`<TreatWarningsAsErrors>true</TreatWarningsAsErrors>`) para impedir que avisos de análise sejam ignorados.
- **Qualidade de Código e Roslyn**: Forçar padrões rígidos de qualidade, estilo de escrita e formatação de código C# utilizando analisadores do Roslyn configurados via arquivo `.editorconfig` na raiz da solução.
- **gRPC Code-First & Protobuf**:
  - As interfaces de contratos de serviço gRPC devem viver na camada `Autogestor.Contracts` e ser decoradas com o atributo `[ServiceContract]`.
  - Os DTOs de Request/Response gRPC devem ser declarados como `record` ou classes imutáveis decorados com `[DataContract]`, e cada propriedade exposta deve conter o atributo `[DataMember(Order = N)]` com numeração de ordem explícita.
  - **Versionamento de Contratos**: Para preservar a compatibilidade retroativa com clientes nativos em produção (como futuras versões do app MAUI instalados em dispositivos de usuários), **nunca** altere a numeração `Order` de propriedades existentes nos DTOs e **nunca** as delete. Adicione campos novos de forma incremental com numerações novas e do tipo anulável (`nullable`).

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

## Testes e TDD (Test-Driven Development)

- Aplicar TDD para regras de negócio (`Domain`) e casos de uso (`Application`).
- Ciclo obrigatório: Escrever o teste (Red) ➜ Implementar o mínimo (Green) ➜ Refatorar (Refactor).
- Usar mocks (NSubstitute/Moq) para isolar as dependências do `Application`.
- Evitar TDD para UI (Blazor/Web) e configurações de infraestrutura.

## Estrutura de Pastas e Documentação

- **READMEs Locais**: Cada projeto dentro de `src/` e `test/` possui seu próprio arquivo `README.md` contendo as regras de desenvolvimento específicas e a estrutura esperada do projeto. O agente deve consultar o `README.md` correspondente à pasta/projeto onde as alterações serão efetuadas antes de realizar qualquer modificação.
- **Índice de Documentação e Regras**:
  * **Global & Processos**:
    * [AGENTS.md](file:///Users/samuelalvesv/dev/autogestor/.agents/AGENTS.md): Convenções de código globais, diretrizes de performance e regras de commit.
    * [architecture.md](file:///Users/samuelalvesv/dev/autogestor/.agents/architecture.md): Mapa completo de camadas, fluxo de dependências e organização arquitetural.
    * [identity-multitenancy.md](file:///Users/samuelalvesv/dev/autogestor/.agents/identity-multitenancy.md): Estratégia de autenticação (ASP.NET Identity), autorização de filiais e isolamento de tenants.
  * **Projetos da Solução (`src/`)**:
    * [Domain](file:///Users/samuelalvesv/dev/autogestor/src/Autogestor.Domain/README.md): Convenções de entidades de domínio, value objects e interfaces de repositórios.
    * [Application](file:///Users/samuelalvesv/dev/autogestor/src/Autogestor.Application/README.md): Regras de casos de uso (Commands/Queries), validações (FluentValidation) e orquestração.
    * [Infrastructure](file:///Users/samuelalvesv/dev/autogestor/src/Autogestor.Infrastructure/README.md): Regras de persistência (EF Core, Dapper), banco de dados (PostgreSQL) e repositórios.
    * [Api](file:///Users/samuelalvesv/dev/autogestor/src/Autogestor.Api/README.md): Apresentação HTTP, endpoints gRPC/gRPC-Web, middlewares de exceção e injeção de dependência raiz.
    * [UI](file:///Users/samuelalvesv/dev/autogestor/src/Autogestor.UI/README.md): Biblioteca de componentes compartilhados (Razor Class Library), temas (MudBlazor) e diretrizes visuais.
    * [Web](file:///Users/samuelalvesv/dev/autogestor/src/Autogestor.Web/README.md): Bootstrap do Blazor WebAssembly, suporte a PWA e compilação AOT.
    * [ServiceDefaults](file:///Users/samuelalvesv/dev/autogestor/src/Autogestor.ServiceDefaults/README.md): Regras de observabilidade, telemetria (OpenTelemetry) e resiliência com .NET Aspire.
  * **Banco de Dados & Scripts**:
    * [db](file:///Users/samuelalvesv/dev/autogestor/db/README.md): Convenções de SQL nativo no PostgreSQL (procedures, functions, triggers, views, scripts de carga).
  * **Testes Automatizados (`test/`)**:
    * [UnitTests](file:///Users/samuelalvesv/dev/autogestor/test/Autogestor.UnitTests/README.md): Diretrizes de testes de unidade e TDD para domínio e aplicação.
    * [IntegrationTests](file:///Users/samuelalvesv/dev/autogestor/test/Autogestor.IntegrationTests/README.md): Diretrizes de testes de integração e cenários de banco.
    * [ArchitectureTests](file:///Users/samuelalvesv/dev/autogestor/test/Autogestor.ArchitectureTests/README.md): Regras de validação de dependências de arquitetura e convenções automáticas.
- **Evitar duplicação**: Não repetir informações entre os arquivos `README.md` ou nos arquivos `.md` na pasta `.agents/`. A informação deve ser escrita em um único local centralizado e apenas referenciada nos outros locais.
- **Definições Assertivas**: Ao redigir documentações, nunca apresente opções ou caminhos alternativos. Defina sempre uma única abordagem padrão, escolhendo a solução mais moderna e aplicável ao projeto.
- **Evolução das Regras**: Os arquivos `.md` do projeto definem regras, mas há flexibilidade para sugerir alterações nelas durante o desenvolvimento, desde que a mudança traga benefícios para o projeto.


## Regras de Commit

- Nunca realize o commit diretamente, não prepare alterações no Git por conta própria e não envie a mensagem de commit sem solicitação do usuário.
- Quando o usuário solicitar "me dê a mensagem de commit" (ou similar), o agente deve verificar todos os arquivos preparados (staged) para commit e definir a mensagem de commit de acordo com essas mudanças preparadas.
- Formato: `tipo: descrição curta em português`
- Tipos: `feat`, `fix`, `refactor`, `docs`, `test`, `chore`
