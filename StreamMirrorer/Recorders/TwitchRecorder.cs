using Recorder.Interfaces;
using System.IO;

namespace Recorder.Recorders;

public class TwitchRecorder : IRecorder
{
    //Services
    
    //Properties
    public string? RecorderName { get; private set; }
    public string? OutputPath { get; private set; }

    public bool IsRecording { get; private set; } = false;

    private Task? _recordingTask;

    public TwitchRecorder()
    {
        
    }
    
    private void Setup(string name, string outputPath)
    {
        // Implementation for setting up Twitch recorder
        Console.WriteLine($"Setting up Twitch recorder for channel: {name}");
        
        RecorderName = name;
        OutputPath = outputPath;
        
    }

    public void StartRecording(string name, string outputPath)
    {
        Setup(name, outputPath);
        Console.WriteLine($"Starting Twitch recording from {RecorderName} to {OutputPath}");
    }

    public void StopRecording()
    {
        Console.WriteLine("Stopping Twitch recording");
    }
    
}