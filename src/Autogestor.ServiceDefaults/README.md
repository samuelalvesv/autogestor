# Autogestor.ServiceDefaults

Biblioteca compartilhada para configuração centralizada de resiliência, observabilidade e saúde dos serviços orquestrados pelo .NET Aspire.

## Regras Específicas

- Nenhuma lógica de negócios ou regras de domínio deve ser colocada neste projeto.
- Expor um método de extensão padrão `AddServiceDefaults(this IHostApplicationBuilder)` para configurar todos os serviços de maneira uniforme.

## Diretrizes de Resiliência & Observabilidade

- **Alta Performance e Escala**: Projetar a lógica de comunicação entre serviços visando tolerância a falhas e capacidade de escala em ambientes distribuídos, aproveitando a resiliência nativa do .NET Aspire.
- **Resiliência HTTP**: Configurar retries automáticos, circuit breakers e timeouts para comunicação HTTP entre serviços utilizando a integração nativa com o `Microsoft.Extensions.Http.Resilience`.
- **Observabilidade**: Todas as métricas, logs e traces devem ser exportados via OpenTelemetry através dos métodos de extensão configurados aqui.
