using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using TaskTrackerCLI.Interfaces;

namespace TaskTrackerCLI.Services;

public class TaskService : ITaskService
{
    private readonly HashSet<string> _allowedStatuses = ["todo", "in-progress", "done"];
    private readonly string _filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.json");
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        AllowTrailingCommas = false,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public TaskService()
    {
        if (!File.Exists(_filepath))
        {
            File.WriteAllText(_filepath, "[]");
        }
    }

    public async Task<Result<int>> AddTask(string description)
    {
        var tasks = await LoadTasks();
        var newTask = new TrackerTask
        {
            Id = tasks.Count == 0 ? 1 : tasks.Max(d => d.Id) + 1,
            Description = description,
            CreatedAt = DateTime.Now,
        };

        var validationResult = ValidateTask(newTask);
        if (!validationResult.IsSuccess)
        {
            return Result<int>.Failure(validationResult.ErrorMessage!);
        }

        tasks.Add(newTask);
        await SaveTasks(tasks);
        return Result<int>.Success(newTask.Id);
    }

    public async Task<Result> UpdateTask(int id, string description)
    {
        var tasks = await LoadTasks();
        var findResult = FindTask(tasks, id);
        if (!findResult.HasValue)
        {
            return Result.Failure($"Task with ID {id} not found.");
        }

        findResult.Value.Task.ChangeDescription(description);
        var validationResult = ValidateTask(findResult.Value.Task);
        if (!validationResult.IsSuccess)
        {
            return Result.Failure(validationResult.ErrorMessage!);
        }

        tasks[findResult.Value.Index] = findResult.Value.Task;
        await SaveTasks(tasks);
        return Result.Success();
    }

    public async Task<Result> DeleteTask(int id)
    {
        var tasks = await LoadTasks();
        var findResult = FindTask(tasks, id);
        if (!findResult.HasValue)
        {
            return Result.Failure($"Task with ID {id} not found.");
        }

        tasks.RemoveAt(findResult.Value.Index);
        await SaveTasks(tasks);
        return Result.Success();
    }

    public async Task<Result> MarkTaskWithStatus(int id, string status)
    {
        if (!_allowedStatuses.Contains(status))
        {
            return Result.Failure($"Invalid status '{status}'. Allowed values are: {string.Join(", ", _allowedStatuses)}");
        }

        var tasks = await LoadTasks();
        var findResult = FindTask(tasks, id);
        if (!findResult.HasValue)
        {
            return Result.Failure($"Task with ID {id} not found.");
        }

        findResult.Value.Task.ChangeStatus(status);
        var validationResult = ValidateTask(findResult.Value.Task);
        if (!validationResult.IsSuccess)
        {
            return Result.Failure(validationResult.ErrorMessage!);
        }
        
        tasks[findResult.Value.Index] = findResult.Value.Task;
        
        await SaveTasks(tasks);
        return Result.Success();
    }

    public async Task<Result<List<TrackerTask>>> GetTasks(string? status)
    {
        if (!string.IsNullOrEmpty(status) && !_allowedStatuses.Contains(status))
        {
            return Result<List<TrackerTask>>.Failure($"Invalid status '{status}'. Allowed values are: {string.Join(", ", _allowedStatuses)}");
        }

        var tasks = await LoadTasks();
        var filteredTasks = tasks
            .Where(t => string.IsNullOrEmpty(status) || t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToList();

        return Result<List<TrackerTask>>.Success(filteredTasks);
    }

    private async Task<List<TrackerTask>> LoadTasks()
    {
        using FileStream stream = File.Open(
            _filepath, FileMode.Open,
            FileAccess.Read, FileShare.Read
        );
        var tasks = await JsonSerializer.DeserializeAsync<List<TrackerTask>>(stream, _jsonOptions);
        return tasks ?? [];
    }

    private async Task SaveTasks(List<TrackerTask> tasks)
    {
        using FileStream stream = File.Open(
            _filepath, FileMode.Create,
            FileAccess.Write, FileShare.None
        );
        await JsonSerializer.SerializeAsync(stream, tasks, _jsonOptions);
    }

    private static (int Index, TrackerTask Task)? FindTask(List<TrackerTask> tasks, int id)
    {
        var index = tasks.FindIndex(t => t.Id == id);
        return index >= 0 ? (index, tasks[index]) : null;
    }

    private static Result ValidateTask(TrackerTask task)
    {
        List<ValidationResult> validationResults = [];
        var isValid = Validator.TryValidateObject(
            task,
            new ValidationContext(task),
            validationResults,
            validateAllProperties: true
        );
        return isValid ? Result.Success() : Result.Failure(
            string.Join("| ", validationResults.Select(v => v.ErrorMessage))
        );
    }
}