---
name: git-flow
description: Git branching model, tower git-flow-next helper, release management guidelines, semantic versioning, and commit types. Use this rule when asked about the git strategy, creating features/releases/hotfixes, or checking commit rules.
---

# Git Flow — autogestor

## Ferramenta

**git-flow-next** — reimplementação moderna do modelo Git Flow, escrita em Go, mantida pelo time do Tower. Substitui as versões descontinuadas (`nvie/gitflow` e `gitflow-avh`).

- Site: <https://git-flow.sh>
- Repositório: <https://github.com/gittower/git-flow-next>

## Instalação

```bash
brew install git-flow-next
```

## Inicialização do Repositório

Executar uma única vez na raiz do projeto:

```bash
git flow init
```

Configuração padrão adotada:

| Configuração | Valor |
| --- | --- |
| Branch de produção | `main` |
| Branch de desenvolvimento | `develop` |
| Prefixo de feature | `feature/` |
| Prefixo de release | `release/` |
| Prefixo de hotfix | `hotfix/` |
| Prefixo de tag | `v` |

## Estrutura de Branches

```text
main              ← código em produção, sempre estável
  │
  ├── feature/*   ← desenvolvimento de funcionalidades (nasce de develop)
  │
  ├── release/*   ← preparação para produção (nasce de develop)
  │
  └── hotfix/*    ← correção urgente em produção (nasce de main)
```

## Workflows

### 1. Desenvolver uma Funcionalidade (Feature)

Usado para toda nova funcionalidade, refatoração ou melhoria.

```bash
# Criar a branch feature a partir de develop
git flow feature start tenant-isolation

# Trabalhar normalmente com commits
git add .
git commit -m "feat: adicionar entidade Tenant com TenantId"
git commit -m "feat: implementar Global Query Filter por tenant"

# Publicar no remoto (para backup ou code review)
git flow feature publish tenant-isolation

# Finalizar: merge em develop e remoção da branch
git flow feature finish tenant-isolation
```

**Regras:**
- Nomear com descrição curta em kebab-case: `tenant-isolation`, `branch-access`
- Manter o escopo pequeno — uma feature por funcionalidade
- Fazer commits frequentes seguindo o padrão do projeto (`tipo: descrição`)

### 2. Preparar uma Release

Usado quando `develop` está pronto para ir para produção.

```bash
# Criar branch de release a partir de develop
git flow release start 1.0.0

# Apenas correções de bugs e ajustes finais nesta branch
git commit -m "fix: corrigir validação de TenantId nulo"
git commit -m "docs: atualizar changelog para v1.0.0"

# Finalizar: merge em main + develop, cria tag v1.0.0
git flow release finish 1.0.0
```

**Regras:**
- Seguir Semantic Versioning: `MAJOR.MINOR.PATCH`
- Na branch de release, apenas bug fixes e ajustes de documentação — nunca features novas

### 3. Corrigir Bug Urgente em Produção (Hotfix)

Usado quando há um bug crítico em `main` que não pode esperar a próxima release.

```bash
# Criar hotfix a partir de main
git flow hotfix start 1.0.1

# Corrigir o problema
git commit -m "fix: corrigir query filter ignorando TenantId em relatórios"

# Finalizar: merge em main + develop, cria tag v1.0.1
git flow hotfix finish 1.0.1
```

**Regras:**
- Usar apenas para correções urgentes que afetam produção
- Incrementar o `PATCH` da versão atual

## Convenções de Commit

Padrão já definido no [AGENTS.md](file:///Users/samuelalvesv/dev/autogestor/.agents/AGENTS.md):

```text
tipo: descrição curta em português
```

| Tipo | Quando usar |
| --- | --- |
| `feat` | Nova funcionalidade |
| `fix` | Correção de bug |
| `refactor` | Refatoração sem alterar comportamento |
| `docs` | Alteração em documentação |
| `test` | Adição ou alteração de testes |
| `chore` | Tarefas de manutenção (configs, dependências) |

## Regras do Projeto

1. **Nunca commitar diretamente em `main` ou `develop`** — sempre usar feature, release ou hotfix
2. **Uma feature = um escopo** — não misturar funcionalidades na mesma branch
3. **Commits atômicos** — cada commit deve compilar e passar nos testes
4. **Mensagens em português** — seguindo o padrão `tipo: descrição`
5. **Código em inglês** — classes, variáveis, métodos e comentários
