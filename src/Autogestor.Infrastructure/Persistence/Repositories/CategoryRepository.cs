using Autogestor.Domain.Entities;
using Autogestor.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Autogestor.Infrastructure.Persistence.Repositories;

public sealed class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await context.Categories.AddAsync(
            entity: category,
            cancellationToken: cancellationToken);
    }

    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(
                predicate: c => c.Id == id,
                cancellationToken: cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Categories
            .AsNoTracking()
            .ToListAsync(cancellationToken: cancellationToken);
    }
}
