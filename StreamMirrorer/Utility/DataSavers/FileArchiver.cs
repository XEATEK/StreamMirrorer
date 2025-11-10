using StreamMirrorer.Interfaces;

namespace StreamMirrorer.Utility.DataSavers;

public class FileArchiver : IStreamArchiver
{
    private readonly ILogger<FileArchiver> _logger;
    
    private readonly FileStream _fileStream;

    public FileArchiver(ILoggerFactory loggerFactory, string streamerName)
    {
        _logger = loggerFactory.CreateLogger<FileArchiver>();
        
        //Set directory string
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string directoryPath = $"O:\\Projects\\StreamMirrorer\\TempStreams\\{streamerName}\\{timestamp}";
        
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
            _logger.LogTrace("Archived {Length} bytes.", data.Length);
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
        GC.SuppressFinalize(this);
    }
}