# db/autogestor.db

Diretório destinado para armazenar scripts SQL (.sql) de migração e estruturação do banco de dados.

## Banco de Dados
- **Motor**: PostgreSQL 18
- **Finalidade**: Armazenamento e execução de scripts SQL.

## Convenções de Escrita SQL (PostgreSQL)
Para manter o padrão e evitar problemas de sensibilidade de caixa (case sensitivity) do PostgreSQL, siga as seguintes diretrizes:

1. **Nomes de Objetos (Tabelas, Colunas, Views, Índices, Schemas)**:
   - Escrever **sempre em minúsculas** e separando palavras por sublinhado (**snake_case**).
   - Exemplo correto: `user_account`, `created_at`, `order_item_id`.
   - Evitar `CamelCase` ou misturar maiúsculas/minúsculas para evitar a necessidade de usar aspas duplas (`""`) no SQL.

2. **Palavras-chave do SQL**:
   - Escrever em **MAIÚSCULAS** para diferenciar da lógica do negócio.
   - Exemplo: `SELECT`, `FROM`, `WHERE`, `INSERT INTO`, `CREATE TABLE`, `FOREIGN KEY`.

3. **Tipos de Dados Padrão**:
   - Chave Primária: `UUID` (para permitir a geração de IDs únicos offline).
   - Texto: `TEXT` (padrão moderno que evita limites arbitrários e simplifica o esquema).
   - Datas/Tempo: `TIMESTAMP WITH TIME ZONE` (timestamptz) por padrão.
   - Booleanos: `BOOLEAN`.

4. **Nome de Chaves Primárias e Estrangeiras**:
   - Chave Primária: Sempre exatamente `id`.
   - Chave Estrangeira: Sempre `nome_tabela_estrangeira_id`.

5. **Nomenclatura dos Arquivos `.sql`**:
   - Formato: `NNN_descricao_em_snake_case.sql` (prefixo numérico sequencial de 3 dígitos).
   - Exemplos: `001_create_user_account.sql`, `002_create_order.sql`, `003_add_index_order_status.sql`.
   - A numeração garante a ordem de execução correta das migrações.

