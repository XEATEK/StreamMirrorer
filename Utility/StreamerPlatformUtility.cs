using StreamMirrorer.Enums;
using StreamMirrorer.Interfaces;
using StreamMirrorer.Recorders;

namespace StreamMirrorer.Utility;

public static class StreamerPlatformUtility 
{
    
    public static Type GetRecorderType(StreamerPlatforms platform)
    {
        return platform switch
        {
            StreamerPlatforms.Twitch => typeof(TwitchRecorder),
            _ => throw new NotSupportedException($"Platform {platform} is not supported.")
        };
    }
}