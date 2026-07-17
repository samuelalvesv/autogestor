namespace Autogestor.Domain.Entities;

public sealed class Category : AuditableEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public Guid UserId { get; private set; }

    private Category() { }

    private Category(string name, string description, Guid userId)
    {
        Name = name;
        Description = description;
        UserId = userId;
    }

    public static Category Create(string name, string description, Guid userId)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Category name cannot be empty.", nameof(name));
        }

        return string.IsNullOrWhiteSpace(description)
            ? throw new ArgumentException("Category description cannot be empty.", nameof(description))
            : userId == Guid.Empty
            ? throw new ArgumentException("User ID must be a valid GUID.", nameof(userId))
            : new Category(name, description, userId);
    }
}
