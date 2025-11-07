namespace StreamMirrorer.Interfaces;

public interface IRecorder
{
    /// <summary>
    /// Name of channel to record.
    /// </summary>
    public string? RecorderName { get; }
    
    /// <summary>
    /// Is the recorder currently recording.
    /// </summary>
    public bool IsRecording { get; }
    
    /// <summary>
    /// Starts the recording process.
    /// </summary>
    /// <param name="name">Name of broadcast channel</param>
    public Task<bool> StartRecording(string name);
    
    /// <summary>
    /// Stops the recording process.
    /// </summary>
    public Task<bool> StopRecording();
}