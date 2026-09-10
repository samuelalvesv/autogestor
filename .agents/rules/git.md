---
name: git-rules
description: Invariantes de governança e segurança para controle de versão, integridade de staging e restrições estritas de comandos Git.
trigger: always_on
---

# Governança de Controle de Versão (Git)

Diretrizes e salvaguardas permanentes para interação com o controle de versão Git no repositório do Autogestor.

---

## 1. Restrições Estritas e Comandos Proibidos

A inteligência artificial **NUNCA** deve executar comandos com efeitos colaterais que modifiquem o histórico, preparem arquivos, alterem branches ou enviem código para repositórios remotos.

É expressamente proibida a execução direta ou indireta dos seguintes comandos:

- `git add` e `git add *`
- `git branch` e `git branch *`
- `git commit` e `git commit *`
- `git push` e `git push *`
- `git reset` e `git reset *`
- `git rm` e `git rm *`
- `rtk git add` e `rtk git add *`
- `rtk git branch` e `rtk git branch *`
- `rtk git commit` e `rtk git commit *`
- `rtk git push` e `rtk git push *`
- `rtk git reset` e `rtk git reset *`
- `rtk git rm` e `rtk git rm *`

> [!CAUTION]
> **Integridade de Versionamento**: Qualquer alteração na staging area (`git add`, `git reset`), criação ou troca de branches, commits e publicações (`push`) são prerrogativas exclusivas do desenvolvedor humano e devem ser realizadas manualmente por ele.

---

## 2. Comandos Permitidos (Somente Leitura e Inspeção)

O agente está autorizado a executar exclusivamente comandos de inspeção e leitura, sempre com o prefixo `rtk`:

- `rtk git status` e `rtk git status *` (verificação do estado da árvore de trabalho e staging)
- `rtk git diff` e `rtk git diff *` (leitura das diferenças não preparadas ou em staging)
- `rtk git log` e `rtk git log *` (inspeção do histórico recente de commits)

---

## 3. Elaboração de Mensagens de Commit

Quando o usuário solicitar a sugestão, elaboração ou definição de uma mensagem de commit (ex: *"me dê a mensagem de commit"*, *"qual o commit para essas alterações?"* ou similar):

- O agente **NUNCA** deve formular a resposta de forma ad-hoc ou improvisada.
- O agente deve invocar e seguir integralmente a skill operacional **`commit`**:
  - Localização: [.agents/skills/commit/SKILL.md](../skills/commit/SKILL.md)
  - Comando: `/commit`
