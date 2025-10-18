namespace StreamMirrorer.Interfaces;

public interface ICommandLineInterface
{
    //If ever needed to run cmd.exe interactively
    //https://learn.microsoft.com/en-us/dotnet/api/System.Diagnostics.ProcessStartInfo.RedirectStandardInput?view=net-9.0
    
    // /// <summary>
    // /// Is the command line interface currently processing a command.
    // /// </summary>
    // bool Busy { get; }
    
    /// <summary>
    /// Starts processing a command.
    /// </summary>
    /// <param name="command"></param>
    /// <returns>Returns string on complete</returns>
    public Task<string> Execute(string command);
    
    // /// <summary>
    // /// Gets any unconsumed output from the current task while it is running.
    // /// </summary>
    // /// <remarks>Make sure your initial command returns json data</remarks>
    // /// <returns>raw json string output</returns>
    // public string GetUnconsumedOutput();
    // 
    // /// <summary>
    // /// stops the current running task.
    // /// </summary>
    // /// <returns>Returns string on complete</returns>
    // public Task<string> StopCurrentTask();
    // 
    // /// <summary>
    // /// Exits the command line interface and cleans up any resources.
    // /// </summary>
    // public void Exit();
}