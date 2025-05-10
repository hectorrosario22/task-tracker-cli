using TaskTrackerCLI;

if (args.Length == 0)
{
    Console.WriteLine("Please provide a command.");
    return;
}

string command = args[0].ToLower();
string[] commandArgs = [.. args.Skip(1)];
TaskService taskService = new();

switch (command)
{
    case "add":
        await AddCommand(commandArgs);
        break;
    case "update":
        await UpdateCommand(commandArgs);
        break;
    default:
        Console.WriteLine("Invalid command.");
        break;
}

async Task AddCommand(string[] commandArgs)
{
    if (commandArgs.Length != 1)
    {
        Console.WriteLine("Please provide a task description.");
        return;
    }

    if (string.IsNullOrWhiteSpace(commandArgs[0]))
    {
        Console.WriteLine("Task description cannot be empty.");
        return;
    }

    var id = await taskService.AddTask(commandArgs[0]);
    Console.WriteLine($"Task added successfully (ID: {id})");
}

async Task UpdateCommand(string[] commandArgs)
{
    if (commandArgs.Length != 2)
    {
        Console.WriteLine("Please provide a task ID and a new description.");
        return;
    }

    if (!int.TryParse(commandArgs[0], out int id))
    {
        Console.WriteLine("Invalid task ID.");
        return;
    }

    if (string.IsNullOrWhiteSpace(commandArgs[1]))
    {
        Console.WriteLine("Task description cannot be empty.");
        return;
    }

    var (Success, ErrorMessage) = await taskService.UpdateTask(id, commandArgs[1]);
    if (!Success)
    {
        Console.WriteLine(ErrorMessage);
    }
}