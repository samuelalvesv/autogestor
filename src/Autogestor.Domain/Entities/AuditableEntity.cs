using Autogestor.Domain.Entities;

namespace Autogestor.Domain.Models;

public abstract class AuditableEntity : Entity
{
    public bool Active { get; protected set; }
    public Guid CreatedBy { get; protected set; }
    public DateTime CreatedAt { get; protected set; }
    public Guid UpdatedBy { get; protected set; }
    public DateTime UpdatedAt { get; protected set; }

    public void Activate()
    {
        Active = true;
    }

    public void Deactivate()
    {
        Active = false;
    }
}
