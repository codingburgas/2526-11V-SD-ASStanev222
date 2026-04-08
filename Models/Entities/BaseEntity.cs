using System;

namespace ProjectLMS.Models.Entities;

/// <summary>
/// Base entity class with common properties for all entities.
/// Provides a consistent Id and CreatedAt timestamp for audit trail.
/// </summary>
public abstract class BaseEntity
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
