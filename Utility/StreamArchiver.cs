namespace StreamMirrorer.Utility;

public class StreamArchiver : IAsyncDisposable
{
    private readonly ILogger<StreamArchiver> _logger;
    
    private readonly FileStream _fileStream;

    public StreamArchiver(ILoggerFactory loggerFactory, string directoryPath)
    {
        _logger = loggerFactory.CreateLogger<StreamArchiver>();
        
        if (!string.IsNullOrEmpty(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        
        string filePath = Path.Combine(directoryPath, "stream.ts");
        
        _fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Read);
        
        _logger.LogInformation("Archive file opened at: {directoryPath}", directoryPath);
    }

    /// <summary>
    /// This is the method we will use as our handler. It writes the received data to the file.
    /// </summary>
    public void HandleDataChunk(ReadOnlyMemory<byte> data)
    {
        if (_fileStream.CanWrite)
        {
            // Write the received chunk of data directly to the file.
            _fileStream.Write(data.Span);
            _logger.LogInformation("Archived {Length} bytes.", data.Length);
        }
    }

    /// <summary>
    /// Implements IAsyncDisposable to ensure the file stream is properly closed and flushed.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        _logger.LogInformation("Archiver is being disposed. Flushing and closing file stream...");
        await _fileStream.DisposeAsync();
        _logger.LogInformation("Archive file closed.");
    }
}