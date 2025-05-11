# Task Tracker CLI

A simple console application built in C# to manage tasks stored in a JSON file. This project is described [here](https://roadmap.sh/projects/task-tracker).

## Features

- Add new tasks with a description
- Update task descriptions
- Delete tasks by ID
- Mark tasks with statuses: `todo`, `in-progress`, `done`
- List all tasks or filter by status
- Tasks are persisted in a `tasks.json` file

## Requirements

- [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) or higher

## Getting Started

**Clone the repository**

```bash
git clone https://github.com/YOUR-USERNAME/task-tracker-cli.git
cd task-tracker-cli
```

**Build the project**

```bash
dotnet build
```

**Run commands**

```bash
# Create a new task
dotnet run -- add "Buy groceries"

# List all tasks
dotnet run -- list

# List filtered tasks by status todo (allowed values: todo, in-progress, done)
dotnet run -- list todo

# Update the description of the task with ID 1
dotnet run -- update 1 "Buy groceries and cook dinner"

# Mark task with ID 1 as In Progress
dotnet run -- mark-in-progress 1

# Mark task with ID 1 as Done
dotnet run -- mark-done 1

# Delete a task with ID 1
dotnet run -- delete 1
```

### Usage

```bash
dotnet run -- <command> [arguments]
```

### Available Commands

| Command            | Arguments               | Description                          |
|--------------------|--------------------------|--------------------------------------|
| `add`              | `"description"`          | Adds a new task                      |
| `update`           | `<id> "new description"` | Updates the task description         |
| `delete`           | `<id>`                   | Deletes a task by ID                 |
| `mark-in-progress` | `<id>`                   | Marks the task as `in-progress`      |
| `mark-done`        | `<id>`                   | Marks the task as `done`             |
| `list`             | `[status]`               | Lists tasks, optionally by status    |