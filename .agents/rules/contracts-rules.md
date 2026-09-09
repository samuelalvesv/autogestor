---
name: contracts-rules
description: Data transfer objects (DTOs), request/response contracts, gRPC service interfaces, Protobuf serialization, and contract versioning guidelines.
applyTo: "src/Autogestor.Contract/**/*.cs"
---

# Regras de Contratos (Autogestor.Contract)

## Estrutura de Pastas
- `Requests/`:
  - `[Feature]/`: DTOs de requisição fortemente tipados específicos por funcionalidade (ex: `Categories/CreateCategoryRequest.cs`).
  - `Request.cs`, `PagedRequest.cs`: Contratos base de requisição.
- `Responses/`:
  - `[Feature]/`: DTOs de resposta fortemente tipados específicos por funcionalidade.
  - `Response.cs`, `PagedResponse.cs`: Contratos base e genéricos de resposta.
  - `EntityResponse.cs`, `AuditableEntityResponse.cs`: Contratos base de entidades com identidade e auditoria.
- `Services/`:
  - `I[Feature]Service.cs`: Interfaces de serviços Code-First (`[ServiceContract]`) (ex: `ICategoryService.cs`).
- `ContractDefaults.cs`: Constantes de paginação e limites de transporte.

## Diretrizes e Restrições
- **Isolamento Total**: Zero dependências de projetos internos (`Domain`, `Application`, `Infrastructure`, etc.) e zero dependências de banco de dados ou frameworks pesados.
- **DTOs Imutáveis com `required`**: Todos os DTOs de Request e Response devem ser declarados como `sealed record` (ou `abstract record` para classes base) com propriedades `{ get; init; }` marcadas obrigatoriamente como `required`.
- **Coleções em Respostas**: Coleções em DTOs (como `PagedResponse<T>.Data`) devem utilizar obrigatoriamente `IReadOnlyList<T>?` em vez de `IEnumerable<T>?`, garantindo contagem O(1) indexada e evitando múltiplas enumerações na UI (Blazor) e no transporte gRPC.
- **Sem Construtores Customizados**: É proibido o uso de construtores parametrizados em DTOs de contratos. Todas as instanciações devem utilizar inicializadores de objeto nomeados (`{ Prop = valor }`), forçando a declaração explícita de todos os campos (mesmo quando o valor for explicitamente `null`).
- **gRPC Code-First & Protobuf**:
  - As interfaces de contratos de serviço gRPC devem residir na camada `Autogestor.Contract` e ser decoradas com o atributo `[ServiceContract]`.
  - Todos os DTOs de Request e Response (incluindo classes base como `Request`, `Response<T>`) devem ser declarados obrigatoriamente como `sealed record` (ou `abstract record` para classes base) decorados com `[DataContract]`, e cada propriedade serializada com `[DataMember(Order = N)]` com numeração de ordem explícita.
  - **Numeração de Ordens em Herança**: Em classes derivadas (ex: `PagedRequest : Request` ou `CategoryResponse : AuditableEntityResponse : EntityResponse`), as ordens dos membros da classe filha devem iniciar sequencialmente após as ordens da classe base.
  - **Evolução Livre de Contratos (Ambiente Pré-Produção)**: Como o projeto está em desenvolvimento ativo e ainda **não está em ambiente de produção**, há total liberdade para alterar, reordenar, renomear ou remover propriedades, DTOs e interfaces de contrato sempre que a modelagem exigir. Não se aplicam restrições de compatibilidade retroativa (breaking changes são totalmente permitidas nesta fase); as numerações de `Order` do Protobuf devem apenas manter consistência interna e sequencialidade entre a base e as classes derivadas.
  - **Tipagem Forte**: É proibido expor propriedades de tipos genéricos (`object`) ou coleções não tipadas. Todos os contratos devem ser fortemente tipados com primitivos, enums ou outros DTOs de contrato.
- **Compartilhamento**: Esta biblioteca é consumida tanto pelo frontend (RCL/WASM) quanto pelo backend (Api/Application), mantendo as definições de transporte únicas e consistentes em toda a solução.

## Ferramentas

- **Plugin `dotnet-diag`**:
  - Subagente `optimizing-dotnet-performance`: Avaliação de impacto de alocação de memória em DTOs, coleções e overhead de serialização Protobuf.
  - Skill `analyzing-dotnet-performance`: Detecção de anti-patterns de alocação de memória em serialização e manipulação de coleções nos contratos.
- **Plugin `dotnet-test`**:
  - Subagente `test-quality-auditor`: Auditoria dos testes que comprovam integridade de serialização e compatibilidade de contratos.
  - Skills `assertion-quality`, `test-gap-analysis`: Validação de profundidade de testes e análise de mutações em contratos ponta a ponta.
- **Plugin `dotnet-upgrade`**:
  - Skill `dotnet-aot-compat`: Garantia de compatibilidade de DTOs e serializadores com compilação Native AOT e trimming.
- **Submódulo `ponytail`**:
  - Skill `ponytail`: Modelagem minimalista de DTOs, evitando propriedades redundantes ou hierarquias especulativas (YAGNI).
