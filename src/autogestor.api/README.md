# autogestor.api (Presentation)

Ponto de entrada HTTP do sistema. Ver [architecture.md](../../.agents/architecture.md) para o mapa completo.

## Estrutura de Pastas Esperada
```
autogestor.api/
├── Endpoints/
│   └── [Feature]Endpoints.cs   # Agrupamento de endpoints por funcionalidade
├── Middlewares/                 # Middlewares customizados (ex: ExceptionHandler)
├── Extensions/                 # Métodos de extensão para configuração
└── Program.cs                  # Composição raiz (DI, middlewares, endpoints)
```

## Regras Específicas
- Usar Minimal APIs (`app.MapGet`, `app.MapPost`, etc.).
- Endpoints apenas recebem a requisição, chamam o caso de uso no `app` e retornam o resultado.
- Nenhuma lógica de negócio deve existir aqui.
- O `Program.cs` é o único local que conhece todas as camadas para montar a injeção de dependência.
