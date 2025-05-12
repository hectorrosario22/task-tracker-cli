using System.ComponentModel.DataAnnotations;

namespace TaskTrackerCLI.Models;

public class TrackerTask : IValidatableObject
{
    public int Id { get; init; }
    public DateTime CreatedAt { get; set; }

    private string _description = string.Empty;
    public required string Description { get => _description; init => _description = value; }
    
    private string _status = "todo";
    public string Status { get => _status; init => _status = value; }

    private DateTime? _updatedAt;
    public DateTime? UpdatedAt { get => _updatedAt; init => _updatedAt = value; }

    public void ChangeDescription(string description)
    {
        _description = description;
        MarkAsUpdated();
    }

    public void ChangeStatus(string status)
    {
        _status = status;
        MarkAsUpdated();
    }

    private void MarkAsUpdated()
    {
        _updatedAt = DateTime.Now;
    }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Description))
        {
            yield return new ValidationResult("Description cannot be empty.", [nameof(Description)]);
        }

        if (Description.Length > 100)
        {
            yield return new ValidationResult("Description cannot exceed 100 characters.", [nameof(Description)]);
        }
    }
}