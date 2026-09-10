#!/bin/bash

# Navega para a raiz do repositório a partir da localização do script
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
cd "$REPO_ROOT" || exit 1

# 1. Filtra se houve alterações em arquivos relevantes para build (.cs, .razor, .csproj, .props, .targets, .sln, .slnx)
RELEVANT_CHANGES=$(git status --porcelain | grep -E '\.(cs|razor|csproj|props|targets|slnx)$')

if [ -z "$RELEVANT_CHANGES" ]; then
  # Nenhuma alteração em código ou infraestrutura de build, permite finalização imediata
  echo '{"decision": "allow"}'
  exit 0
fi

# 2. Executa a formatação automática de código nas alterações feitas
dotnet format

# 3. Executa o build da solução para verificar integridade
BUILD_OUTPUT=$(dotnet build 2>&1)
EXIT_CODE=$?

if [ $EXIT_CODE -eq 0 ]; then
  # Compilação bem-sucedida, permite a finalização
  echo '{"decision": "allow"}'
else
  # Compilação falhou, filtra mensagens de erro
  ERRORS=$(echo "$BUILD_OUTPUT" | grep -E "error CS|error MSB|Build FAILED")

  if [ -z "$ERRORS" ]; then
    # Se não achar erros específicos, pega as últimas 20 linhas do log
    ERRORS=$(echo "$BUILD_OUTPUT" | tail -n 20)
  fi

  # Escapa barras invertidas, aspas duplas e formata quebras de linha para JSON válido
  ESCAPED_ERRORS=$(echo "$ERRORS" | sed 's/\\/\\\\/g' | sed 's/"/\\"/g' | awk '{printf "%s\\n", $0}')

  echo "{\"decision\": \"continue\", \"reason\": \"A compilação do projeto (.NET build) falhou. Por favor, corrija os seguintes erros de compilação antes de concluir:\\n\\n$ESCAPED_ERRORS\"}"
fi
