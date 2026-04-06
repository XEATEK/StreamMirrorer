using System.Globalization;
using StreamMirrorer.Interfaces;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using StreamMirrorer.Utility;
using StreamMirrorer.Utility.DataSavers;

namespace StreamMirrorer.Recorders;

public class TwitchRecorder : IRecorder
{
    //Services
    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<TwitchRecorder> _logger;
    private readonly IConfiguration _configuration;

    //Properties
    public string? RecorderName { get; private set; }

    public bool IsRecording { get; private set; } = false;

    private CancellationTokenSource _cts;

    private string? StreamLink { get; set; }

    public TwitchRecorder(ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        _logger = loggerFactory.CreateLogger<TwitchRecorder>();
        _loggerFactory = loggerFactory;
        _configuration = configuration;
        _cts = new CancellationTokenSource();
    }

    private void Setup(string username)
    {
        // Implementation for setting up Twitch recorder
        _logger.LogInformation("Setting up Twitch recorder for channel: {username}", username);

        //Getting streamer link
        string linkFormatStr = _configuration["LinkFormats:Twitch"] ??
                               throw new NullReferenceException(message: "No Twitch link format specified.");

        Dictionary<string, object> linkParms = new()
        {
            { "StreamerName", username }
        };

        StreamLink = StringFormatter.ReplaceNamedPlaceholders(linkFormatStr, linkParms);

        _logger.LogInformation("StreamerLink: {streamLink}", StreamLink);

        RecorderName = username;

        _logger.LogInformation("creating cancellation token");
        _cts = new CancellationTokenSource();
    }

    public void StartRecording(string channelName)
    {
        _ = Task.Factory.StartNew(() => RecordingProcess(channelName));
    }

    private async Task RecordingProcess(string channelName)
    {
        try
        {
            Setup(channelName);
            _logger.LogInformation("Starting Twitch recording from {channelName}", channelName);

            string streamLink = await GetStreamLink();

            //Create stream handler that stores the received stream
            await using IStreamArchiver archiver = new FileArchiver(_loggerFactory, _configuration, channelName);

            HlsStreamHandler handler = new(_loggerFactory, _configuration);

            // Start processing. This will run until StopProcessing is called or the stream ends.
            var processingTask = handler.StartProcessing(streamLink, archiver.HandleDataChunk);
            
            await Task.Delay(Timeout.Infinite, _cts.Token)
                .ConfigureAwait(ConfigureAwaitOptions.SuppressThrowing);

            handler.StopProcessing();

            // Wait for the main processing task to complete its cleanup
            await processingTask;

            _logger.LogInformation("Recording finished.");
        }
        catch (Exception e)
        {
            _logger.LogCritical(message: "Error occured while starting recording: {e}", e);
            throw;
        }
    }

    public async Task<bool> StopRecording()
    {
        _logger.LogInformation("Stopping Twitch recording");
        await _cts.CancelAsync();
        return true;
    }

    private async Task<string> GetStreamLink()
    {
        CommandLineInterface cli = new CommandLineInterface();

        string streamLinkCommand = _configuration["StreamLinkCommand"] ??
                                   "streamlink {TwitchParameters} {StreamerLink} best --json";

        Dictionary<string, object> linkParms = new()
        {
            {
                "StreamerLink",
                StreamLink ?? throw new ArgumentNullException(nameof(StreamLink), "StreamLink cannot be null.")
            },
            { "TwitchParameters", _configuration["StreamLink:TwitchParameters"] ?? "" }
        };

        _logger.LogInformation("Parameters: {linkParms}", linkParms);

        string command = StringFormatter.ReplaceNamedPlaceholders(streamLinkCommand, linkParms);

        _logger.LogInformation("Command to get stream url: {command}", command);

        string result = await cli.Execute(command);

        _logger.LogInformation("Result: {result}", result);

        JsonNode? jsonData = JsonNode.Parse(result);

        if (jsonData != null &&
            jsonData.AsObject().TryGetPropertyValue("url", out var urlNode) &&
            urlNode != null)
        {
            return urlNode.ToString();
        }

        throw new NullReferenceException("Could not find 'url' in the expected JSON array structure.");
    }
}