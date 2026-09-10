---
name: service-defaults-rules
description: .NET Aspire ServiceDefaults, OpenTelemetry tracing and metrics, HTTP resilience policies, and global service health checks.
applyTo: "src/Autogestor.ServiceDefaults/**/*.cs"
---

# Regras do ServiceDefaults (Autogestor.ServiceDefaults)

## Diretrizes e Restrições
- **Sem Lógica de Negócios**: Nenhuma lógica de domínio ou regra de negócio deve residir neste projeto.
- **Padrão de Extensão**: Deve expor o método de extensão `AddServiceDefaults(this IHostApplicationBuilder)` para configurar observabilidade, resiliência e health checks de forma uniforme em todos os serviços.
- **Observabilidade**: Exporte métricas, logs e traces via OpenTelemetry (OTLP).
- **Resiliência HTTP**: Configurar timeouts, retries e circuit breakers utilizando as políticas do `Microsoft.Extensions.Http.Resilience`.

## Ferramentas

- **Plugin `dotnet-aspnetcore`**: skills `configuring-opentelemetry-dotnet`, `dotnet-webapi`
- **Plugin `dotnet-diag`**: subagente `optimizing-dotnet-performance`, skill `analyzing-dotnet-performance`
- **Submódulo `ponytail`**: skill `ponytail`
