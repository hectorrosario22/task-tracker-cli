namespace TaskTrackerCLI.Interfaces;

public interface ITaskService
{
    Task<Result<int>> AddTask(string description);
    Task<Result> UpdateTask(int id, string description);
    Task<Result> DeleteTask(int id);
    Task<Result> MarkTaskWithStatus(int id, string status);
    Task<Result<List<TrackerTask>>> GetTasks(string? status);
}