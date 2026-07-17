# Regras do Agente — autogestor

## Idioma

- Todo código (classes, variáveis, métodos, comentários) deve ser escrito em **inglês**.
- Mensagens de commit, documentação de negócio e comunicação com o usuário devem ser em **português brasileiro**.

## Regras Dinâmicas de Desenvolvimento (Injetadas por Glob)

Este projeto utiliza regras de ativação dinâmica pelo Antigravity baseadas no arquivo editado. A IA **não precisa ler** estes arquivos manualmente, eles são injetados automaticamente:
- **Convenções C#**: [.agents/rules/csharp-conventions.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/csharp-conventions.md) (Geral C# e Razor).
- **Contratos gRPC**: [.agents/rules/grpc-contracts.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/grpc-contracts.md) (gRPC Code-First).
- **Camada de Domínio**: [.agents/rules/domain-rules.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/domain-rules.md) (Entities, Value Objects).
- **Camada de Aplicação**: [.agents/rules/application-rules.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/application-rules.md) (MediatR, Use Cases).
- **Camada de Infraestrutura**: [.agents/rules/infrastructure-rules.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/infrastructure-rules.md) (EF Core, Repositórios).
- **Camada de Apresentação (Api)**: [.agents/rules/api-rules.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/api-rules.md) (gRPC Services).
- **Interface Gráfica (UI)**: [.agents/rules/ui-rules.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/ui-rules.md) (MudBlazor, RCL).
- **Host Web (Web)**: [.agents/rules/web-rules.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/web-rules.md) (WASM PWA).
- **ServiceDefaults**: [.agents/rules/service-defaults-rules.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/service-defaults-rules.md) (Resiliência e OTel).
- **Banco de Dados Nativo**: [.agents/rules/database-rules.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/database-rules.md) (db/).
- **Testes**: [.agents/rules/testing-rules.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/testing-rules.md) (Unit, Integration, Arch).

## Regras Sob Demanda (Model Decision / Invocação Manual)

Documentações com YAML Frontmatter carregadas dinamicamente pela IA apenas quando o contexto exige:
- **Arquitetura Geral**: [.agents/rules/architecture.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/architecture.md) (Estrutura de dependências e responsabilidades).
- **Autenticação & Multi-Tenancy**: [.agents/rules/identity-multitenancy.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/identity-multitenancy.md) (TenantId, acessos e branches).
- **Git Flow & Commits**: [.agents/rules/git-flow.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/git-flow.md) e [.agents/rules/git-commit.md](file:///Users/samuelalvesv/dev/autogestor/.agents/rules/git-commit.md).

## Nota sobre READMEs locais
> [!NOTE]
> Os arquivos `README.md` locais contidos nos subprojetos são destinados exclusivamente a desenvolvedores humanos. Para a IA, as regras dinâmicas em `.agents/rules/` têm precedência absoluta. A IA **deve ignorar** a leitura manual dos READMEs locais.
