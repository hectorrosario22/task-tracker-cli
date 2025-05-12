using Microsoft.Extensions.DependencyInjection;
using TaskTrackerCLI.Interfaces;
using TaskTrackerCLI.Services;

var serviceProvider = BuildServiceProvider();
var taskService = serviceProvider.GetRequiredService<ITaskService>();
var printService = serviceProvider.GetRequiredService<IPrintService>();

if (args.Length == 0)
{
    printService.PrintError("Please provide a command.");
    return;
}

string command = args[0].ToLower();
string[] commandArgs = [.. args.Skip(1)];

switch (command)
{
    case "add":
        await AddCommand(commandArgs);
        break;
    case "update":
        await UpdateCommand(commandArgs);
        break;
    case "delete":
        await DeleteCommand(commandArgs);
        break;
    case "mark-in-progress":
        await MarkTaskWithStatusCommand(commandArgs, "in-progress");
        break;
    case "mark-done":
        await MarkTaskWithStatusCommand(commandArgs, "done");
        break;
    case "list":
        await ListTasksCommand(commandArgs);
        break;
    default:
        printService.PrintError("Invalid command.");
        break;
}

async Task AddCommand(string[] commandArgs)
{
    if (commandArgs.Length != 1)
    {
        printService.PrintError("Please provide a task description.");
        return;
    }

    var result = await taskService.AddTask(commandArgs[0]);
    if (!result.IsSuccess)
    {
        printService.PrintError(result.ErrorMessage!);
        return;
    }

    printService.PrintSuccess($"Task added successfully (ID: {result.Value})");
}

async Task UpdateCommand(string[] commandArgs)
{
    if (commandArgs.Length != 2)
    {
        printService.PrintError("Please provide a task ID and a new description.");
        return;
    }

    if (!int.TryParse(commandArgs[0], out int id))
    {
        printService.PrintError("Invalid task ID.");
        return;
    }

    var result = await taskService.UpdateTask(id, commandArgs[1]);
    if (!result.IsSuccess)
    {
        printService.PrintError(result.ErrorMessage!);
        return;
    }

    printService.PrintSuccess($"Task updated successfully (ID: {id})");
}

async Task DeleteCommand(string[] commandArgs)
{
    if (commandArgs.Length != 1)
    {
        printService.PrintError("Please provide a task ID.");
        return;
    }

    if (!int.TryParse(commandArgs[0], out int id))
    {
        printService.PrintError("Invalid task ID.");
        return;
    }

    var result = await taskService.DeleteTask(id);
    if (!result.IsSuccess)
    {
        printService.PrintError(result.ErrorMessage!);
        return;
    }

    printService.PrintSuccess($"Task deleted successfully (ID: {id})");
}

async Task MarkTaskWithStatusCommand(string[] commandArgs, string status)
{
    if (commandArgs.Length != 1)
    {
        printService.PrintError("Please provide a task ID.");
        return;
    }

    if (!int.TryParse(commandArgs[0], out int id))
    {
        printService.PrintError("Invalid task ID.");
        return;
    }

    var result = await taskService.MarkTaskWithStatus(id, status);
    if (!result.IsSuccess)
    {
        printService.PrintError(result.ErrorMessage!);
        return;
    }

    printService.PrintSuccess($"Task marked as '{status}' successfully (ID: {id})");
}

async Task ListTasksCommand(string[] commandArgs)
{
    string? status = commandArgs.Length > 0 ? commandArgs[0] : null;
    if (commandArgs.Length > 0 && string.IsNullOrWhiteSpace(status))
    {
        printService.PrintError("Invalid status.");
        return;
    }

    var result = await taskService.GetTasks(status);
    if (!result.IsSuccess)
    {
        printService.PrintError(result.ErrorMessage!);
        return;
    }
    
    printService.PrintTasks(result.Value ?? []);
}

ServiceProvider BuildServiceProvider()
{
    var services = new ServiceCollection();
    services.AddScoped<ITaskService, TaskService>();
    services.AddScoped<IPrintService, PrintService>();

    return services.BuildServiceProvider();
}