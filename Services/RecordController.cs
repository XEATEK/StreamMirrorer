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
        streamerName = streamerName.ToLower();
        
        IRecorder recorder = _recorderFactory.Create(streamerPlatform);

        if (Recorders.ContainsKey(streamerName))
        {
            return false;
        }
        
        recorder.StartRecording(streamerName);
        
        Recorders.Add(streamerName, recorder);
        
        return true;
    }

    public async Task<bool> StopActiveRecording(string streamerName, StreamerPlatforms streamerPlatform)
    {
        streamerName = streamerName.ToLower();
        IRecorder? recorder;
        
        Recorders.TryGetValue(streamerName, out recorder);
        
        if (recorder == null)
        {
            return false;
        }
        
        Recorders.Remove(streamerName);
        return await recorder.StopRecording();
    }
}