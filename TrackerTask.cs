namespace TaskTrackerCLI;

public record TrackerTask(string Description)
{
    public int Id { get; set; }
    public string Status { get; set; } = "todo";
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}