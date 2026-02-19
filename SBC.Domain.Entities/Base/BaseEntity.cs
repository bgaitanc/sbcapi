using System.ComponentModel.DataAnnotations.Schema;

namespace SBC.Domain.Entities.Base;

/// <summary>
/// Represents the base entity class that serves as the foundation for
/// other entities within the application domain. Provides common properties
/// such as Id, CreatedAt, CreatedBy, UpdatedAt, and UpdatedBy
/// to standardize entity structures.
/// </summary>
public class BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// This property is used as the primary key to uniquely distinguish each entity instance.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was created.
    /// This property is used for tracking the creation timestamp of the entity.
    /// </summary>
    [Column(TypeName = "datetime2")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the user or system responsible for creating the entity.
    /// This property tracks the origin of the entity's creation for auditing purposes.
    /// </summary>
    //TODO: createdBy and updatedBy should be Guid?
    public string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the entity was last updated.
    /// This property is used to track and record the most recent modification
    /// to the entity, providing a history of changes over time.
    /// </summary>
    [Column(TypeName = "datetime2")]
    public DateTime? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}