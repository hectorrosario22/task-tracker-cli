using TaskTrackerCLI.Interfaces;

namespace TaskTrackerCLI.Services;

public class PrintService : IPrintService
{
    public void PrintTasks(List<TrackerTask> tasks)
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("No tasks found.");
            return;
        }

        string[] headers = ["ID", "Description", "Status", "Created At", "Updated At"];

        int widthID = Math.Max(headers[0].Length, tasks.Max(t => t.Id.ToString().Length));
        int widthDescription = Math.Max(headers[1].Length, tasks.Max(t => t.Description.Length));
        int widthStatus = Math.Max(headers[2].Length, tasks.Max(t => t.Status.ToString().Length));
        int widthCreatedAt = Math.Max(headers[3].Length, tasks.Max(t => t.CreatedAt.ToString().Length));
        int widthUpdatedAt = Math.Max(headers[4].Length, tasks.Max(t => t.UpdatedAt.HasValue ? t.UpdatedAt.Value.ToString().Length : 0));

        string format = $"{{0,-{widthID}}} | {{1,-{widthDescription}}} | {{2,-{widthStatus}}} | {{3,-{widthCreatedAt}}} | {{4,-{widthUpdatedAt}}}";
        Console.WriteLine(format, headers[0], headers[1], headers[2], headers[3], headers[4]);

        foreach (var task in tasks)
        {
            Console.WriteLine(
                format,
                task.Id,
                task.Description,
                task.Status,
                task.CreatedAt,
                task.UpdatedAt.HasValue ? task.UpdatedAt.Value.ToString() : "N/A"
            );
        }
    }

    public void PrintError(string errorMessage)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(errorMessage);
        Console.ResetColor();
    }

    public void PrintSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}