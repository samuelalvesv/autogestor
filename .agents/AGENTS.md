# Regras do Agente — autogestor

## Idioma

- Todo código (classes, variáveis, métodos, comentários) deve ser escrito em **inglês**.
- Mensagens de commit, documentação de negócio e comunicação com o usuário devem ser em **português brasileiro**.

## Regras Dinâmicas (Ativadas por Escopo)

Este projeto utiliza regras de ativação dinâmica baseadas em arquivos e tarefas para economizar tokens de contexto:
- **C# & Performance**: Ativada para arquivos `*.cs` e `*.razor`. Veja [.agents/rules/csharp-conventions.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/csharp-conventions.md).
- **gRPC & Contratos**: Ativada para a camada de contratos. Veja [.agents/rules/grpc-contracts.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/grpc-contracts.md).
- **Testes & TDD**: Ativada para diretório de testes. Veja [.agents/rules/tdd-testing.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/tdd-testing.md).
- **Git Commits**: Ativada sob demanda. Veja [.agents/rules/git-commit.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/git-commit.md).

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
