using StreamMirrorer.Enums;
using StreamMirrorer.Interfaces;
using StreamMirrorer.Recorders;

namespace StreamMirrorer.Utility;

public static class StreamerPlatformUtility 
{
    public static string GetPlatformLink(StreamerPlatforms platform)
    {
        switch (platform)
        {
            case StreamerPlatforms.Twitch:
                return "https://www.twitch.tv/{StreamerName}";
            default:
                throw new ArgumentOutOfRangeException(paramName: nameof(platform), message: "No valid platform specified, No link available.");
        }
    }
    
    public static Type GetRecorderType(StreamerPlatforms platform)
    {
        return platform switch
        {
            StreamerPlatforms.Twitch => typeof(TwitchRecorder),
            _ => throw new NotSupportedException($"Platform {platform} is not supported.")
        };
    }
}