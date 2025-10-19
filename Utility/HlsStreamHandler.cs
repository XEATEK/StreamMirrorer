using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using StreamMirrorer.Recorders;

namespace StreamMirrorer.Utility;

public class HlsStreamHandler
{
    private readonly ILogger<HlsStreamHandler> _logger;
    private readonly IConfiguration _configuration;

    private Process? _ffmpegProcess;
    private CancellationTokenSource? _cancellationTokenSource;

    public HlsStreamHandler(ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        _logger = loggerFactory.CreateLogger<HlsStreamHandler>();
        _configuration = configuration;
    }

    /// <summary>
    /// Starts processing an HLS stream by piping its transport stream output to a handler.
    /// </summary>
    /// <param name="hlsPlaylistUrl">The URL of the M3U8 playlist for the HLS stream.</param>
    /// <param name="onSegmentReceived">An action called with chunks of the transport stream. The memory is only valid for the duration of the call.</param>
    public async Task StartProcessing(
        string hlsPlaylistUrl,
        Action<ReadOnlyMemory<byte>> onSegmentReceived
    )
    {
        string ffmpegPath = _configuration["FFMpegPath"] ??
                            throw new NullReferenceException("FFMpegPath not set in appsettings.json");


        string arguments = $"-i \"{hlsPlaylistUrl}\" -c copy -f mpegts pipe:1";

        var processStartInfo = new ProcessStartInfo(ffmpegPath)
        {
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        _ffmpegProcess = new Process { StartInfo = processStartInfo };
        _ffmpegProcess.ErrorDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data)) _logger.LogInformation("[FFmpeg]: {Data}", args.Data);
        };

        try
        {
            _ffmpegProcess.Start();
            _ffmpegProcess.BeginErrorReadLine();
            _logger.LogInformation("FFmpeg process started with PID: {Id}", _ffmpegProcess.Id);

            _cancellationTokenSource = new CancellationTokenSource();

            // Pass the updated handler to the reading method
            await ReadOutputStream(
                _ffmpegProcess.StandardOutput.BaseStream,
                onSegmentReceived,
                _cancellationTokenSource.Token
            );
        }
        catch (Exception ex)
        {
            _logger.LogError("Failed to start FFmpeg process: {Message}", ex.Message);
            StopProcessing();
        }
        finally
        {
            try
            {
                if (_ffmpegProcess?.HasExited == false)
                {
                    _ffmpegProcess.WaitForExit();
                }
            }
            catch (InvalidOperationException)
            {
                // There is no process attached to _ffmpegProcess so we don't have to kill it and can just return.
            }

            _logger.LogInformation("FFmpeg process has exited.");
        }
    }

    /// <summary>
    /// Stops the FFmpeg process gracefully.
    /// </summary>
    public void StopProcessing()
    {
        try
        {
            if (_ffmpegProcess == null || _ffmpegProcess.HasExited)
            {
                return;
            }
        }
        catch (InvalidOperationException)
        {
            // There is no process attached to _ffmpegProcess so we don't have to kill it and can just return.
            _logger.LogInformation("No process was attached to _ffmpegProcess so exiting.");
            _ffmpegProcess?.Dispose();
            return;
        }

        try
        {
            _logger.LogInformation("Stopping FFmpeg process...");
            _cancellationTokenSource?.Cancel();
            _ffmpegProcess.Kill();
            _ffmpegProcess = null;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while stopping the process: {Message}", ex.Message);
        }
    }

    /// <summary>
    /// Asynchronously reads the stream and invokes the handler with data chunks.
    /// </summary>
    private async Task ReadOutputStream(Stream stream, Action<ReadOnlyMemory<byte>> handler, CancellationToken token)
    {
        Memory<byte> buffer = new byte[1048576]; //1MiB

        try
        {
            while (!token.IsCancellationRequested)
            {
                int bytesRead = await stream.ReadAsync(buffer, token);

                if (bytesRead == 0)
                {
                    break; // End of stream
                }

                handler(buffer.Slice(0, bytesRead));
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Stream reading was canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while reading the stream: {Message}", ex.Message);
        }
    }
}