using StreamMirrorer.Interfaces;
using System.IO;
using StreamMirrorer.Utility;

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

    private string? StreamLink { get; set; }
    
    public TwitchRecorder(ILogger<TwitchRecorder> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }
    
    private void Setup(string username, string outputPath)
    {
        // Implementation for setting up Twitch recorder
        _logger.LogInformation("Setting up Twitch recorder for channel: {username}", username);
        
        //Getting streamer link
        string linkFormatStr = _configuration["LinkFormats:Twitch"] ?? throw new NullReferenceException(message:"No Twitch link format specified.");
        
        Dictionary<string, object> linkParms = new()
        {
            { "StreamerName", username }
        };
        
        StreamLink = StringFormatter.ReplaceNamedPlaceholders(linkFormatStr, linkParms);
        
        _logger.LogInformation("StreamerLink: {streamLink}", StreamLink);
        
        RecorderName = username;
        OutputPath = outputPath;
        
    }

    private async Task<string> GetStreamLink()
    {
        CommandLineInterface cli = new CommandLineInterface();
        
        string streamLinkCommand = _configuration["StreamLinkCommand"] ?? "streamlink {TwitchParameters} {StreamerLink} best --json";
        
        Dictionary<string, object> linkParms = new()
        {
            { "StreamerLink", StreamLink ?? throw new ArgumentNullException(nameof(StreamLink), "StreamLink cannot be null.") },
            { "TwitchParameters", _configuration["StreamLink:TwitchParameters"] ?? "" }
        };
        
        _logger.LogInformation("Parameters: {linkParms}", linkParms);
        
        string command = StringFormatter.ReplaceNamedPlaceholders(streamLinkCommand, linkParms);
        
        _logger.LogInformation("Command to get stream url: {command}", command);
        
        string result = await cli.Execute(command);

        _logger.LogInformation("Result: {result}", result);
        
        return result;
    }

    public async Task<bool> StartRecording(string name, string outputPath)
    {
        Setup(name, outputPath);
        _logger.LogInformation("Starting Twitch recording from {name} to {outputPath}", name, outputPath);
        
        string streamLink = await GetStreamLink();

        return true;
    }

    public async Task<bool> StopRecording()
    {
        _logger.LogInformation("Stopping Twitch recording");
        
        return true;
    }
    
}