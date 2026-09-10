---
name: domain-rules
description: Rules for Domain layer entities, value objects, domain services, interfaces, and events.
applyTo: "src/Autogestor.Domain/**/*.cs"
---

# Regras do Domínio (Autogestor.Domain)

## Estrutura de Pastas
- `Entities/`: Entidades com identidade (`Id`).
- `ValueObjects/`: Objetos de valor imutáveis sem identidade.
- `Enums/`: Enumerações de domínio.
- `Interfaces/`: Contratos de repositórios (`IXxxRepository`).
- `Events/`: Eventos de domínio.

## Diretrizes e Restrições
- **Isolamento de Infraestrutura**: Zero pacotes NuGet externos e zero referências a outros projetos da Solution.
- **Herança de Classes Base**: As entidades de domínio devem herdar de `AuditableEntity` (para dados globais auditados) ou `TenantEntity` (para dados isolados por tenant auditados). Casos especiais que não requerem auditoria de usuário (como a própria entidade de usuário, logs de sistema ou dados estáticos globais) devem herdar diretamente de `Entity`.
- **Encapsulamento**: Em classes seladas, os construtores devem ser estritamente privados (`private`); em classes base abstratas destinadas a herança, devem ser protegidos (`protected`). A instanciação pública é realizada exclusivamente por métodos de fábrica expressivos (`Create`, `From`) para garantir que instâncias inválidas nunca sejam criadas.
- **Identity & Multi-Tenancy**: Seguir integralmente [.agents/rules/identity-multitenancy.md](identity-multitenancy.md).

## Ferramentas

- **Plugin `dotnet-test`**: subagente `test-quality-auditor`, skills `assertion-quality`, `test-gap-analysis`
- **Plugin `dotnet-diag`**: subagente `optimizing-dotnet-performance`, skill `analyzing-dotnet-performance`
- **Submódulo `ponytail`**: skills `ponytail`, `ponytail-review`
