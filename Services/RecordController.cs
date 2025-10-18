using StreamMirrorer.Enums;
using StreamMirrorer.Interfaces;
using StreamMirrorer.Recorders;
using StreamMirrorer.Utility;

namespace StreamMirrorer.Services;

public class RecordController : IRecordController
{
    //services
    private readonly ILogger<IRecordController> _logger;
    private readonly RecorderFactory _recorderFactory;
    
    //Private variables
    private Dictionary<string, IRecorder> Recorders { get; set; }
    
    public RecordController(ILogger<RecordController> logger, RecorderFactory recorderFactory)
    {
        _logger = logger;
        _recorderFactory = recorderFactory;
        
        Recorders = new Dictionary<string, IRecorder>();
    }
    
    public async Task<bool> StartNewRecording(string streamerName, StreamerPlatforms streamerPlatform)
    {
        //Getting streamer link
        string linkFormatStr = StreamerPlatformUtility.GetPlatformLink(streamerPlatform);
        
        Dictionary<string, object> parms = new()
        {
            { "StreamerName", streamerName }
        };
        
        string formatedLink = StringFormatter.ReplaceNamedPlaceholders(linkFormatStr, parms);
        
        _logger.LogInformation("StreamerLink: {formatedLink}", formatedLink);
        
        IRecorder recorder = _recorderFactory.Create(streamerPlatform);

        const string outputPath = @"O:\Projects\StreamMirrorer\TempStreams";
        
        recorder.StartRecording(formatedLink, outputPath);
        
        Thread.Sleep(10000);
        
        recorder.StopRecording();
        
        return true;
    }

    public async Task<bool> StopActiveRecording(string streamerName, StreamerPlatforms streamerPlatform)
    {
        return true; 
    }
}