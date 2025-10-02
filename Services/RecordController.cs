using Recorder.Interfaces;
using Recorder.Recorders;

namespace Recorder.Services;

public class RecordController : IRecordController
{

    TwitchRecorder _recorder { get; }

    public RecordController(TwitchRecorder recorder)
    {
        _recorder = recorder;
    }
}