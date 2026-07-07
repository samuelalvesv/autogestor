# Autogestor.Domain

Camada mais interna da arquitetura. Ver [architecture.md](../../.agents/architecture.md) para o mapa completo.

## Estrutura de Pastas Esperada

```text
Autogestor.Domain/
├── Entities/          # Entidades com identidade (Id)
├── ValueObjects/      # Objetos imutáveis sem identidade
├── Enums/             # Enumerações de domínio
├── Interfaces/        # Contratos de repositórios (IXxxRepository)
└── Events/            # Eventos de domínio
```text

## Regras Específicas

- Zero pacotes NuGet externos.
- Zero referências a outros projetos.
- Entidades devem ter construtores privados e métodos de fábrica (`Create`, `From`).
