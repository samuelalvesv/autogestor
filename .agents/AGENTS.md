# Regras do Agente — autogestor

## Idioma
- Todo código (classes, variáveis, métodos, comentários) deve ser escrito em **inglês**.
- Mensagens de commit, documentação de negócio e comunicação com o usuário devem ser em **português brasileiro**.

## Convenções de Código
- Usar `record` para DTOs e Value Objects imutáveis.
- Usar `sealed class` por padrão; remover `sealed` apenas se herança for intencional.
- Nunca instanciar dependências com `new`; sempre usar injeção de dependência.
- Nomes de interfaces começam com `I` (ex: `IOrderRepository`).
- **Programação Assíncrona**: Usar obrigatoriamente `async`/`await` de ponta a ponta para todas as operações de I/O e tarefas concorrentes.
- **Ferramentas e Bibliotecas**: Adotar exclusivamente tecnologias open source consolidadas e amplamente validadas pela comunidade do .NET.
- **Design Distribuído**: Projetar toda a lógica do sistema visando alta performance e capacidade de escala em ambientes distribuídos, aproveitando a resiliência e a descoberta de serviços nativas do .NET Aspire.
- **Desacoplamento e Reuso (Wrappers)**:
  - Implementar **wrappers** de controle (como o padrão `Result<T>` para fluxos de negócio ou handlers de exceções globais) para evitar a repetição de lógica de tratamento de erro, logs e try-catchs em múltiplos endpoints.

## Testes e TDD (Test-Driven Development)
- Aplicar TDD para regras de negócio (`core`) e casos de uso (`app`).
- Ciclo obrigatório: Escrever o teste (Red) ➜ Implementar o mínimo (Green) ➜ Refatorar (Refactor).
- Usar mocks (NSubstitute/Moq) para isolar as dependências do `app`.
- Evitar TDD para UI (Blazor/web) e configurações de infraestrutura.

## Estrutura de Pastas e Documentação
- Consultar `.agents/architecture.md` para o mapa completo de camadas e dependências.
- **Evitar duplicação**: Não repetir informações entre os arquivos `README.md` ou nos arquivos `.md` na pasta `.agents/`. A informação deve ser escrita em um único local centralizado e apenas referenciada nos outros locais.
- **Definições Assertivas**: Ao redigir documentações, nunca apresente opções ou caminhos alternativos. Defina sempre uma única abordagem padrão, escolhendo a solução mais moderna e aplicável ao projeto.


## Regras de Commit
- Nunca realize o commit diretamente também não prepare a alteração. Apenas envie a mensagem de commit para aprovação.
- Formato: `tipo: descrição curta em português`
- Tipos: `feat`, `fix`, `refactor`, `docs`, `test`, `chore`
