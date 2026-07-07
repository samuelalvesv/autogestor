# db (Scripts de Banco de Dados)

Diretório destinado para armazenar scripts SQL (.sql) que rodam fora do fluxo padrão do Entity Framework Core (como lógica nativa no PostgreSQL, consultas complexas via Dapper, views, procedures e carga inicial de dados).

> **Atenção**: As migrações do banco de dados (esquema principal, tabelas, chaves) são gerenciadas automaticamente via C# (EF Core Migrations). Este diretório serve apenas para objetos de banco que não são mapeados nativamente pelo ORM.

## Banco de Dados

- **Motor**: PostgreSQL 18

## Estrutura de Pastas Esperada

Sempre que uma nova estrutura SQL for necessária, ela deve ser organizada nas seguintes pastas:

- `db/procedures/`: Stored Procedures (arquivos começam com o prefixo `sp_`, ex: `sp_processa_fechamento.sql`).
- `db/functions/`: Funções de Banco / UDFs (arquivos começam com o prefixo `fn_`, ex: `fn_calcula_comissao.sql`).
- `db/views/`: Views e Views Materializadas (arquivos começam com o prefixo `vw_`, ex: `vw_resumo_vendas.sql`).
- `db/triggers/`: Triggers de banco (arquivos começam com o prefixo `tr_`, ex: `tr_atualiza_estoque.sql`).
- `db/scripts/`: Scripts SQL para popular tabelas com dados padrões ou estáticos (inserts de tabelas de categorias, configurações iniciais, estados/cidades, etc.).

## Convenções de Escrita SQL (PostgreSQL)

Para manter o padrão e evitar problemas de sensibilidade de caixa (case sensitivity) do PostgreSQL, siga as seguintes diretrizes:

1. **Nomes de Objetos (Tabelas, Colunas, Views, Procedures)**:
   - Escrever **sempre em minúsculas** e separando palavras por sublinhado (**snake_case**).
   - Exemplo correto: `user_account`, `created_at`, `sp_atualizar_saldo`.
   - Evitar `CamelCase` ou misturar maiúsculas/minúsculas para evitar a necessidade de usar aspas duplas (`""`) no SQL.

2. **Palavras-chave do SQL**:
   - Escrever em **MAIÚSCULAS** para diferenciar da lógica do negócio.
   - Exemplo: `SELECT`, `FROM`, `WHERE`, `INSERT INTO`, `CREATE OR REPLACE FUNCTION`, `RETURNS TRIGGER`.

3. **Prefixos Obrigatórios de Nomenclatura**:
   - Procedures: `sp_`
   - Functions: `fn_`
   - Views: `vw_`
   - Triggers: `tr_`
