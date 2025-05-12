namespace TaskTrackerCLI.Interfaces;

public interface IPrintService
{
    void PrintTasks(List<TrackerTask> tasks);
    void PrintError(string errorMessage);
    void PrintSuccess(string message);
}