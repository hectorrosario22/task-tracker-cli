using System.Text.Json;

namespace TaskTrackerCLI;

public class TaskService
{
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

    public async Task<int> AddTask(string description)
    {
        using FileStream stream = File.Open(
            _filepath, FileMode.Open,
            FileAccess.ReadWrite, FileShare.None
        );
        var tasks = await JsonSerializer.DeserializeAsync<List<TrackerTask>>(stream, _jsonOptions) ?? [];

        TrackerTask task = new(description)
        {
            Id = tasks.Count == 0 ? 1 : tasks.Max(d => d.Id) + 1,
            CreatedAt = DateTime.Now,
        };
        tasks.Add(task);

        stream.SetLength(0);
        stream.Position = 0;

        await JsonSerializer.SerializeAsync(stream, tasks, _jsonOptions);
        return task.Id;
    }

    public async Task<(bool Success, string? ErrorMessage)> UpdateTask(int id, string description)
    {
        using FileStream stream = File.Open(
            _filepath, FileMode.Open,
            FileAccess.ReadWrite, FileShare.None
        );
        var tasks = await JsonSerializer.DeserializeAsync<List<TrackerTask>>(stream, _jsonOptions) ?? [];

        var index = tasks.FindIndex(t => t.Id == id);
        if (index == -1)
        {
            return (false, $"Task with ID {id} not found.");
        }

        var task = tasks.First(t => t.Id == id);
        task = task with
        {
            Description = description,
            UpdatedAt = DateTime.Now
        };

        tasks[index] = task;

        stream.SetLength(0);
        stream.Position = 0;

        await JsonSerializer.SerializeAsync(stream, tasks, _jsonOptions);
        return (true, null);
    }

    public async Task<(bool Success, string? ErrorMessage)> DeleteTask(int id)
    {
        using FileStream stream = File.Open(
            _filepath, FileMode.Open,
            FileAccess.ReadWrite, FileShare.None
        );
        var tasks = await JsonSerializer.DeserializeAsync<List<TrackerTask>>(stream, _jsonOptions) ?? [];

        var index = tasks.FindIndex(t => t.Id == id);
        if (index == -1)
        {
            return (false, $"Task with ID {id} not found.");
        }

        tasks.RemoveAt(index);
        stream.SetLength(0);
        stream.Position = 0;

        await JsonSerializer.SerializeAsync(stream, tasks, _jsonOptions);
        return (true, null);
    }
}