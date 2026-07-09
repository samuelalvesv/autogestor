# Identity & Multi-Tenancy — autogestor

## Estratégia

Autenticação e autorização baseadas em **ASP.NET Identity + isolamento multi-tenant**, utilizando banco de dados compartilhado com filtros automáticos por `TenantId`. Cada tenant opera em completo isolamento — nenhum dado, usuário ou operação cruza a fronteira entre tenants.

## Hierarquia de Acesso

```text
Tenant (empresa que contratou o SaaS)
├── Branch (loja/filial)  — 0, 1 ou N por tenant
│   ├── Dados operacionais (produtos, vendas, estoque...)
│   └── Funcionários vinculados via UserBranchAccess
└── User (funcionário)    — 0, 1 ou N por tenant
    └── Acesso a 0, 1 ou N branches (controlado pelo admin)
```text

### Cardinalidades

| Relação | Cardinalidade |
| --- | --- |
| Tenant → Branch | `1:N` (um tenant possui zero ou muitas lojas) |
| Tenant → User | `1:N` (um tenant possui zero ou muitos funcionários) |
| User ↔ Branch | `N:N` (um funcionário acessa zero ou muitas lojas; uma loja tem zero ou muitos funcionários) |

## Modelo de Entidades

> **Regra de Domain-Driven Design (DDD)**: Evitar modelos anêmicos. As propriedades das entidades devem possuir setters privados (`private set`), e qualquer alteração de estado (ex: ativar/desativar um Tenant ou alterar o nome de uma Branch) deve ser exposta obrigatoriamente por **métodos de domínio expressivos** (ex: `Activate()`, `Deactivate()`, `Rename(string newName)`) contendo as validações necessárias para proteger as invariantes de negócio.

### Tenant

Representa a empresa que contratou o SaaS. É a raiz do isolamento.

```csharp
public sealed class Tenant
{
    public Guid Id { get; init; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; init; }
}
```text

### Branch (Loja/Filial)

Unidade operacional dentro do tenant.

```csharp
public sealed class Branch
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
    public string Name { get; private set; }
    public bool IsActive { get; private set; }
}
```text

### ApplicationUser

Extensão do `IdentityUser` com vínculo ao tenant.

```csharp
public sealed class ApplicationUser : IdentityUser<Guid>
{
    public Guid TenantId { get; init; }
    public string FullName { get; private set; }
    public bool IsActive { get; private set; }
}
```text

### UserBranchAccess (tabela associativa N:N)

Controla quais lojas cada funcionário pode acessar e com qual perfil.

```csharp
public sealed class UserBranchAccess
{
    public Guid UserId { get; init; }
    public Guid BranchId { get; init; }
    public BranchRole Role { get; init; }
}
```text

### BranchRole (enum de perfis)

```csharp
public enum BranchRole
{
    Admin,
    Seller,
    Administrative,
    Logistics
}
```text

## Dois Níveis de Isolamento

### Nível 1 — Isolamento entre Tenants (automático)

Garantido por **Global Query Filters** do EF Core. Toda entidade que herda de `TenantEntity` é filtrada automaticamente pelo `TenantId` do usuário autenticado. Nenhuma query manual é necessária.

```csharp
// Entidade base para todas as entidades com escopo de tenant
public abstract class TenantEntity
{
    public Guid Id { get; init; }
    public Guid TenantId { get; init; }
}

// DbContext — filtro global aplicado automaticamente
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder);

    builder.Entity<Branch>()
        .HasQueryFilter(b => b.TenantId == _currentTenantId);

    builder.Entity<Product>()
        .HasQueryFilter(p => p.TenantId == _currentTenantId);

    // Aplicar para TODA entidade que herda de TenantEntity
}
```text

> **Regra inviolável**: nenhuma query no sistema opera sem o filtro de `TenantId`. Mesmo queries administrativas internas respeitam o escopo do tenant autenticado.

### Nível 2 — Controle de Acesso por Branch (explícito)

Verificado no **Application layer** via `IBranchAuthorizationService`. Antes de executar qualquer operação sobre uma branch, o use case confirma que o usuário possui registro em `UserBranchAccess` para aquela branch.

```csharp
// Interface no Application layer
public interface IBranchAuthorizationService
{
    Task<bool> HasAccessAsync(Guid userId, Guid branchId, CancellationToken ct);
    Task<bool> HasRoleAsync(Guid userId, Guid branchId, BranchRole role, CancellationToken ct);
    Task<IReadOnlyList<Guid>> GetAccessibleBranchIdsAsync(Guid userId, CancellationToken ct);
}
```text

## Fluxo de Autenticação e Autorização

```text
[Funcionário] → POST /api/auth/login { email, password }
      ↓
ASP.NET Identity valida credenciais
      ↓
JWT gerado com claims:
{
  "sub": "user-guid",
  "tenantId": "tenant-guid",
  "role": "Collaborator"
}
      ↓
[Funcionário] → GET /api/products?branchId=xxx
      ↓
Nível 1: Global Query Filter garante que só dados do tenant aparecem
Nível 2: IBranchAuthorizationService verifica acesso à branch xxx
      ↓
✅ Autorizado → retorna dados
❌ Sem acesso à branch → 403 Forbidden
```text

## Fluxo do Admin Gerenciando Acessos

### Criar uma nova loja

```text
[Admin] → POST /api/branches { name: "Filial Shopping" }
        → Branch criada com TenantId do admin autenticado
```text

### Criar um funcionário e conceder acesso

```text
[Admin] → POST /api/users {
           fullName: "Maria Silva",
           email: "maria@...",
           branchAccesses: [
             { branchId: "filial-centro-id", role: "Seller" },
             { branchId: "filial-shopping-id", role: "Administrative" }
           ]
         }
        → Usuário criado no mesmo TenantId do admin
        → Registros de UserBranchAccess criados
```text

### Alterar acessos de um funcionário

```text
[Admin] → PUT /api/users/{userId}/branch-accesses {
           grant:  [{ branchId: "filial-online-id", role: "Logistics" }],
           revoke: [{ branchId: "filial-centro-id" }]
         }
        → Adiciona/remove registros de UserBranchAccess
```text

## Exemplo Prático de Isolamento

```text
Tenant "Moda Brasil" (tenant-a)
├── Filial Centro
│   ├── João (Admin)        → vê Filial Centro ✅
│   ├── Maria (Vendedora)   → vê Filial Centro ✅
│   └── Carlos (Logístico)  → vê Filial Centro ✅
├── Filial Shopping
│   ├── João (Admin)        → vê Filial Shopping ✅
│   └── Ana (Vendedora)     → vê Filial Shopping ✅
└── Filial Online
    ├── João (Admin)        → vê Filial Online ✅
    └── Pedro (Operador)    → vê Filial Online ✅

Maria NÃO acessa Filial Shopping    ❌ (sem UserBranchAccess)
Ana NÃO acessa Filial Centro        ❌ (sem UserBranchAccess)
João acessa TODAS                    ✅ (admin concedeu acesso)

Tenant "Tech Store" (tenant-b)
├── Loja Única
│   └── Lucas (Admin)

Lucas NÃO vê nada da "Moda Brasil"  ❌ (Global Query Filter por TenantId)
João NÃO vê nada da "Tech Store"    ❌ (Global Query Filter por TenantId)
```text

## Distribuição nas Camadas

| Camada | Artefatos |
| --- | --- |
| **Domain** | `Tenant`, `Branch`, `UserBranchAccess`, `BranchRole`, `TenantEntity` (base), `ITenantProvider`, `IBranchAccessRepository` |
| **Application** | `IBranchAuthorizationService`, use cases (`CreateBranchCommand`, `GrantBranchAccessCommand`, `RevokeBranchAccessCommand`), DTOs |
| **Infrastructure** | `ApplicationUser : IdentityUser<Guid>`, `TenantProvider` (lê `TenantId` do JWT), Global Query Filters no `DbContext`, implementação de `IBranchAuthorizationService` |
| **Api** | Configuração do ASP.NET Identity, JWT Bearer, middleware de resolução de tenant, Authorization Policies |

## Tecnologias Utilizadas

| Componente | Tecnologia |
| --- | --- |
| Autenticação | ASP.NET Identity (.NET 10) |
| Tokens | JWT Bearer |
| ORM | EF Core com Global Query Filters |
| Isolamento de Tenant | Claim `TenantId` + filtro automático no `DbContext` |
| Autorização por Branch | `UserBranchAccess` + `IBranchAuthorizationService` |
