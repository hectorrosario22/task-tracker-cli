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

    public async Task<int> AddTask(TrackerTask task)
    {
        using FileStream stream = File.Open(
            _filepath, FileMode.Open,
            FileAccess.ReadWrite, FileShare.None
        );
        var tasks = await JsonSerializer.DeserializeAsync<List<TrackerTask>>(stream, _jsonOptions) ?? [];

        task.Id = tasks.Count == 0 ? 1 : tasks.Max(d => d.Id) + 1;
        task.CreatedAt = DateTime.Now;
        tasks.Add(task);

        stream.SetLength(0);
        stream.Position = 0;

        await JsonSerializer.SerializeAsync(stream, tasks, _jsonOptions);
        return task.Id;
    }
}