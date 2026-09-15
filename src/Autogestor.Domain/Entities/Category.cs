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
        Validate(title: title, description: description);
        return new Category(title: title, description: description);
    }

    public void Update(string title, string description)
    {
        Validate(title: title, description: description);

        Title = title;
        Description = description;
    }

    private static void Validate(string title, string description)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException(message: "O título da categoria não pode ser vazio.", paramName: nameof(title));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException(message: "A descrição da categoria não pode ser vazia.", paramName: nameof(description));
    }
}
