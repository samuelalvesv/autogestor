---
name: contracts-rules
description: Data transfer objects (DTOs), request/response contracts, gRPC service interfaces, and contract isolation guidelines.
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
- **Coleções em Respostas**: Coleções em DTOs (como `PagedResponse<T>.Data`) devem utilizar obrigatoriamente `IReadOnlyList<T>?` em vez de `IEnumerable<T>?`, garantindo contagem O(1) indexada e evitando múltiplas enumerações na UI (Blazor) e gRPC.
- **Sem Construtores Customizados**: É proibido o uso de construtores parametrizados em DTOs de contratos. Todas as instanciações devem utilizar inicializadores de objeto nomeados (`{ Prop = valor }`), forçando a declaração explícita de todos os campos (mesmo quando o valor for explicitamente `null`).
- **gRPC Code-First**:
  - As interfaces de contratos de serviço devem ser decoradas com `[ServiceContract]`.
  - Todos os DTOs de Request e Response (incluindo classes base como `Request`, `Response<T>`) devem ser decorados com `[DataContract]`, e cada propriedade serializada com `[DataMember(Order = N)]`.
  - **Numeração de Ordens em Herança**: Em classes derivadas (ex: `PagedRequest : Request` ou `CategoryResponse : AuditableEntityResponse : EntityResponse`), as ordens dos membros da classe filha devem iniciar sequencialmente após as ordens da classe base.
  - As regras de serialização Protobuf e versionamento retrocompatível devem seguir [grpc-contracts.md](grpc-contracts.md).
- **Compartilhamento**: Esta biblioteca é consumida tanto pelo frontend (RCL/WASM) quanto pelo backend (Api/Application), mantendo as definições de transporte únicas e consistentes em toda a solução.
