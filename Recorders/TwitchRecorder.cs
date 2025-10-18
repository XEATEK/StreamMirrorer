using StreamMirrorer.Interfaces;
using System.IO;

namespace StreamMirrorer.Recorders;

public class TwitchRecorder : IRecorder
{
    //Services
    private readonly ILogger<TwitchRecorder> _logger;
    private readonly IConfiguration _configuration;
    
    //Properties
    public string? RecorderName { get; private set; }
    public string? OutputPath { get; private set; }

    public bool IsRecording { get; private set; } = false;

    private Task? _recordingTask;

    public TwitchRecorder(ILogger<TwitchRecorder> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    
    private void Setup(string streamLink, string outputPath)
    {
        // Implementation for setting up Twitch recorder
        Console.WriteLine($"Setting up Twitch recorder for channel: {streamLink}");
        
        RecorderName = streamLink;
        OutputPath = outputPath;
        
    }

    public void StartRecording(string streamLink, string outputPath)
    {
        Setup(streamLink, outputPath);
        Console.WriteLine($"Starting Twitch recording from {RecorderName} to {OutputPath}");
    }

    public void StopRecording()
    {
        Console.WriteLine("Stopping Twitch recording");
    }
    
}