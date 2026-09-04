---
name: antigravity-rtk-rules
description: Token-optimized CLI proxy rules for executing shell commands via rtk.
trigger: always_on
---

# RTK - Rust Token Killer (Google Antigravity)

**Usage**: Token-optimized CLI proxy for shell commands.

## Rule

Always prefix shell commands with `rtk` to minimize token consumption.

Examples:

```bash
rtk git status
rtk cargo test
rtk ls src/
rtk grep "pattern" src/
rtk find "*.rs" .
rtk docker ps
rtk gh pr list
```

## Meta Commands

```bash
rtk gain              # Show token savings
rtk gain --history    # Command history with savings
rtk discover          # Find missed RTK opportunities
rtk proxy <cmd>       # Run raw (no filtering, for debugging)
```

## Why

RTK filters and compresses command output before it reaches the LLM context, cutting up to 90% of the bash output on common operations. Always use `rtk <cmd>` instead of raw commands.

## Permissões de Execução Segura

As permissões para execução de comandos de terminal com o prefixo `rtk` e utilitários do ecossistema .NET e Git são centralizadas e versionadas no arquivo `.agents/settings.json` na raiz do repositório:
- Apenas subcomandos explícitos de leitura, inspeção e compilação são permitidos automaticamente (ex: `rtk git status`, `rtk git log`, `rtk dotnet test`).
- Comandos com efeitos colaterais, mutações de controle de versão ou exclusão (`git commit`, `git add`, `git push`, `rm -rf`) são explicitamente bloqueados na lista de `deny`.
