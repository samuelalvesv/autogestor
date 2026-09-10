namespace Autogestor.Domain.Entities;

public sealed class Category : TenantEntity
{
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;

    private Category() { }

    private Category(string title, string description)
    {
        Title = title;
        Description = description;
    }

    public static Category Create(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("O título da categoria não pode ser vazio.", nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("A descrição da categoria não pode ser vazia.", nameof(description));

        return new Category(title, description);
    }
}
