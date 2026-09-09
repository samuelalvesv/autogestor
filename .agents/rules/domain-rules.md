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
- **Encapsulamento**: As entidades devem possuir construtores privados/protegidos e métodos de fábrica públicos (`Create`, `From`) para garantir que instâncias inválidas nunca sejam criadas.
- **Isolamento de Tenants**: Entidades que possuem escopo de tenant devem herdar de `TenantEntity` (consultar regra de `identity-multitenancy`).

## Ferramentas

- **Plugin `dotnet-test`**:
  - Subagente `test-quality-auditor`: Auditoria de cobertura e profundidade dos testes de invariantes das entidades e Value Objects.
  - Skills `assertion-quality`, `test-gap-analysis`: Elaboração de asserções ricas em testes unitários e identificação de mutações e casos de borda.
- **Plugin `dotnet-diag`**:
  - Subagente `optimizing-dotnet-performance`: Avaliação de impacto de alocação de memória e execução em métodos de fábrica e operações no hot path.
  - Skill `analyzing-dotnet-performance`: Garantia de alocação eficiente, imutabilidade e ausência de anti-patterns de memória no domínio.
- **Submódulo `ponytail`**:
  - Skills `ponytail`, `ponytail-review`: Modelagem limpa, rica e anti-complexidade, mantendo entidades focadas estritamente nas regras de negócio necessárias (YAGNI).
