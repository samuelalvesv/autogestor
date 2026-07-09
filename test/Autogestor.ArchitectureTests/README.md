# Autogestor.ArchitectureTests

Testes de arquitetura automatizados para validar programaticamente o mapa de dependências e regras de design da Clean Architecture.

## Responsabilidades

- Validar que a estrutura de camadas e as regras de ouro definidas em `architecture.md` não estão sendo violadas na compilação.

## Regras Específicas

- **Validação de Dependências**:
  - A camada `Autogestor.Domain` não pode referenciar nenhum outro projeto da Solution nem bibliotecas de terceiros de infraestrutura.
  - A camada `Autogestor.Application` deve referenciar apenas `Domain` (proibido referenciar `Infrastructure`, `Api`, `Web`).
  - Classes e tipos expostos em `Domain` e `Application` não devem possuir referências ou dependências com pacotes NuGet específicos de banco ou rede (como `Microsoft.EntityFrameworkCore`).
- **Padrões de Nomenclatura e Design**:
  - Garantir que todas as interfaces comecem com `I` (ex: `IXxxRepository`).
  - Garantir que classes dentro de Domain e Application sejam marcadas como `sealed` por padrão, a menos que herança seja explicitamente intencional.
- **Tecnologia**: Usar a biblioteca **`NetArchTest.eNet`** para definir as asserções de arquitetura de forma fluida.
