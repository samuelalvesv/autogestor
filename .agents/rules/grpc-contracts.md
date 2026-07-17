---
name: grpc-contracts
description: Apply gRPC Code-First, Protobuf serialization, and backward compatibility contract versioning guidelines.
applyTo: "src/Autogestor.Contracts/**/*.cs"
---

# gRPC Code-First & Protobuf

- As interfaces de contratos de serviço gRPC devem viver na camada `Autogestor.Contracts` e ser decoradas com o atributo `[ServiceContract]`.
- Os DTOs de Request/Response gRPC devem ser declarados como `record` ou classes imutáveis decorados com `[DataContract]`, e cada propriedade exposta deve conter o atributo `[DataMember(Order = N)]` com numeração de ordem explícita.
- **Versionamento de Contratos**: Para preservar a compatibilidade retroativa com clientes nativos em produção (como futuras versões do app MAUI instalados em dispositivos de usuários), **nunca** altere a numeração `Order` de propriedades existentes nos DTOs e **nunca** as delete. Adicione campos novos de forma incremental com numerações novas e do tipo anulável (`nullable`).
