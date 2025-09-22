namespace Recorder.Interfaces;

public interface IRecorder
{
    /// <summary>
    /// Name of channel to record.
    /// </summary>
    public string? RecorderName { get; }
    
    /// <summary>
    /// Folder path to save recordings.
    /// </summary>
    public string? OutputPath { get; }
    
    /// <summary>
    /// Is the recorder currently recording.
    /// </summary>
    public bool IsRecording { get; }
    
    /// <summary>
    /// Starts the recording process.
    /// </summary>
    /// <param name="name">Name of broadcast channel</param>
    /// <param name="outputPath">System folder path to temporarily store data</param>
    public void StartRecording(string name, string outputPath);
    
    /// <summary>
    /// Stops the recording process.
    /// </summary>
    public void StopRecording();
}