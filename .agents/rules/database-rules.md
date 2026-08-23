---
name: database-rules
description: Database scripts, PostgreSQL naming conventions (snake_case), capitalization, and database folder structure.
applyTo: "db/**/*.sql"
---

# Regras de Banco de Dados Nativo (db/)

Este diretório armazena scripts SQL (`.sql`) que rodam fora do fluxo padrão do EF Core (como views, procedures, triggers e cargas de dados).
As migrações de tabelas e chaves são gerenciadas via C# (EF Core Migrations).

## Estrutura de Pastas
- `db/procedures/`: Stored Procedures (nome com prefixo `sp_`, ex: `sp_processa_fechamento.sql`).
- `db/functions/`: UDFs/Funções (nome com prefixo `fn_`, ex: `fn_calcula_comissao.sql`).
- `db/views/`: Views (nome com prefixo `vw_`, ex: `vw_resumo_vendas.sql`).
- `db/triggers/`: Triggers (nome com prefixo `tr_`, ex: `tr_atualiza_estoque.sql`).
- `db/scripts/`: SQL scripts para popular tabelas (inserts).

## Convenções de Escrita SQL (PostgreSQL 18)
- **Nomes de Objetos**: Sempre em minúsculas e snake_case (ex: `user_account`, `created_at`). Evitar camelCase.
- **Palavras-chave SQL**: Escrever em MAIÚSCULAS (ex: `SELECT`, `FROM`, `WHERE`, `CREATE OR REPLACE FUNCTION`).
- **Prefixos Obrigatórios**:
  - Procedures: `sp_`
  - Functions: `fn_`
  - Views: `vw_`
  - Triggers: `tr_`
