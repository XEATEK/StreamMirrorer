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

    [HttpPost("start")]
    public async Task<IActionResult> StartRecording([FromBody] string streamerName, [FromBody] StreamerPlatforms platform)
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

    [HttpPost("stop")]
    public async Task<IActionResult> StopRecording([FromBody] string streamerName, [FromBody] StreamerPlatforms platform)
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
