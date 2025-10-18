using StreamMirrorer.Enums;
using StreamMirrorer.Interfaces;
using StreamMirrorer.Recorders;
using StreamMirrorer.Utility;

namespace StreamMirrorer.Services;

public interface IRecorderFactory
{
    IRecorder Create(StreamerPlatforms streamerPlatform);
}

public class RecorderFactory : IRecorderFactory
{
    private readonly IServiceProvider _serviceProvider;

    public RecorderFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IRecorder Create(StreamerPlatforms streamerPlatform)
    {
        Type recorderType = StreamerPlatformUtility.GetRecorderType(streamerPlatform);
        
        IRecorder recorder = (IRecorder)_serviceProvider.GetRequiredService(recorderType);

        return recorder;
    }
}