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
- **Encapsulamento**: As entidades devem possuir construtores privados/protegidos e métodos de fábrica públicos (`Create`, `From`) para garantir que instâncias inválidas nunca sejam criadas.
- **Isolamento de Tenants**: Entidades que possuem escopo de tenant devem herdar de `TenantEntity` (consultar regra de `identity-multitenancy`).
