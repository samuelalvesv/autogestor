---
name: contracts-rules
description: Data transfer objects (DTOs), request/response contracts, gRPC service interfaces, and contract isolation guidelines.
applyTo: "src/Autogestor.Contracts/**/*.cs"
---

# Regras de Contratos (Autogestor.Contracts)

## Estrutura de Pastas
- `[Feature]/`:
  - `Requests/`: DTOs de requisição fortemente tipados (`[DataContract]`).
  - `Responses/`: DTOs de resposta fortemente tipados (`[DataContract]`).
  - `Services/`: Interfaces de serviços gRPC (`[ServiceContract]`).
- `Common/`: DTOs compartilhados globais (ex: paginação, respostas padrão, DTOs utilitários).

## Diretrizes e Restrições
- **Isolamento Total**: Zero dependências de projetos internos (`Domain`, `Application`, `Infrastructure`, etc.) e zero dependências de banco de dados ou frameworks pesados.
- **DTOs Imutáveis**: Todos os DTOs de Request e Response devem ser declarados obrigatoriamente como `sealed record` imutáveis.
- **gRPC Code-First**:
  - As interfaces de contratos de serviço devem ser decoradas com `[ServiceContract]`.
  - As regras de serialização Protobuf e versionamento retrocompatível devem seguir [.agents/rules/grpc-contracts.md](grpc-contracts.md).
- **Compartilhamento**: Esta biblioteca é consumida tanto pelo frontend (RCL/WASM) quanto pelo backend (Api/Application), mantendo as definições de transporte únicas e consistentes em toda a solução.
