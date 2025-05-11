using System.Text.Json;

namespace TaskTrackerCLI;

public class TaskService
{
    readonly HashSet<string> _allowedStatuses = ["todo", "in-progress", "done"];
    readonly string _filepath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tasks.json");
    readonly JsonSerializerOptions _jsonOptions = new()
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
        var newTask = new TrackerTask(description)
        {
            Id = tasks.Count == 0 ? 1 : tasks.Max(d => d.Id) + 1,
            CreatedAt = DateTime.Now,
        };
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

        var task = findResult.Value.Task with
        {
            Description = description,
            UpdatedAt = DateTime.Now
        };

        tasks[findResult.Value.Index] = task;

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

        var task = findResult.Value.Task with
        {
            Status = status,
            UpdatedAt = DateTime.Now
        };

        tasks[findResult.Value.Index] = task;
        await SaveTasks(tasks);
        return Result.Success();
    }

    public async Task<Result<string>> GetTasksAsJson(string? status)
    {
        if (!string.IsNullOrEmpty(status) && !_allowedStatuses.Contains(status))
        {
            return Result<string>.Failure($"Invalid status '{status}'. Allowed values are: {string.Join(", ", _allowedStatuses)}");
        }

        var tasks = await LoadTasks();
        var filteredTasks = tasks
            .Where(t => string.IsNullOrEmpty(status) || t.Status == status)
            .OrderByDescending(t => t.CreatedAt)
            .ToList();

        var tasksAsJson = JsonSerializer.Serialize(filteredTasks, _jsonOptions);
        return Result<string>.Success(tasksAsJson);
    }

    async Task<List<TrackerTask>> LoadTasks()
    {
        using FileStream stream = File.Open(
            _filepath, FileMode.Open,
            FileAccess.Read, FileShare.Read
        );
        var tasks = await JsonSerializer.DeserializeAsync<List<TrackerTask>>(stream, _jsonOptions);
        return tasks ?? [];
    }

    async Task SaveTasks(List<TrackerTask> tasks)
    {
        using FileStream stream = File.Open(
            _filepath, FileMode.Create,
            FileAccess.Write, FileShare.None
        );
        await JsonSerializer.SerializeAsync(stream, tasks, _jsonOptions);
    }

    (int Index, TrackerTask Task)? FindTask(List<TrackerTask> tasks, int id)
    {
        var index = tasks.FindIndex(t => t.Id == id);
        return index >= 0 ? (index, tasks[index]) : null;
    }
}