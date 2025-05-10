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
    default:
        Console.WriteLine("Invalid command.");
        break;
}

async Task AddCommand(string[] commandArgs)
{
    if (commandArgs.Length != 1)
    {
        return;
    }

    // TODO: Add to JSON file
    TrackerTask newTask = new(commandArgs[0]);
    var id = await taskService.AddTask(newTask);
    Console.WriteLine($"Task added successfully (ID: {id})");
}