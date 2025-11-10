using StreamMirrorer.Enums;
using StreamMirrorer.Recorders;

namespace StreamMirrorer.Interfaces;

public interface IRecordController
{
    public Task<bool> StartNewRecording(string streamerName, StreamerPlatforms streamerPlatform);
    public Task<bool> StopActiveRecording(string streamerName, StreamerPlatforms streamerPlatform);
}