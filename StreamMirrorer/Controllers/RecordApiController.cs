using Microsoft.AspNetCore.Mvc;
using StreamMirrorer.Enums;
using StreamMirrorer.Interfaces;

namespace StreamMirrorer.Controllers;

[ApiController]
[Route("api/[controller]")]
[IgnoreAntiforgeryToken]
public class RecordApiController : ControllerBase
{
    private readonly IRecordController _recordController;

    public RecordApiController(IRecordController recordController)
    {
        _recordController = recordController;
    }

    [HttpGet("start")]
    public async Task<IActionResult> StartRecording([FromQuery] string streamerName, [FromQuery] StreamerPlatforms platform)
    {
        if (string.IsNullOrWhiteSpace(streamerName))
        {
            return BadRequest("Streamer name is required.");
        }

        bool result = await _recordController.StartNewRecording(streamerName, platform);
        if (result)
        {
            return Ok(new { message = $"Started recording for {streamerName} on {platform}" });
        }

        return Conflict(new { message = $"Recording already in progress for {streamerName}" });
    }

    [HttpGet("stop")]
    public async Task<IActionResult> StopRecording([FromQuery] string streamerName, [FromQuery] StreamerPlatforms platform)
    {
        if (string.IsNullOrWhiteSpace(streamerName))
        {
            return BadRequest("Streamer name is required.");
        }

        bool result = await _recordController.StopActiveRecording(streamerName, platform);
        if (result)
        {
            return Ok(new { message = $"Stopped recording for {streamerName} on {platform}" });
        }

        return NotFound(new { message = $"No active recording found for {streamerName}" });
    }
}
