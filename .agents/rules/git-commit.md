---
name: git-commit
description: Use when the user requests to formulate a commit message or when reviewing staged files to prepare a git commit.
---

# Regras de Commit

- Nunca realize o commit diretamente, não prepare alterações no Git por conta própria e não envie a mensagem de commit sem solicitação do usuário.
- Quando o usuário solicitar "me dê a mensagem de commit" (ou similar), o agente deve verificar todos os arquivos preparados (staged) para commit e definir a mensagem de commit de acordo com essas mudanças preparadas.
- Formato: `tipo: descrição curta em português`
- Tipos: `feat`, `fix`, `refactor`, `docs`, `test`, `chore`
