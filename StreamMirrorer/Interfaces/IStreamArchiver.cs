namespace StreamMirrorer.Interfaces;

public interface IStreamArchiver : IAsyncDisposable
{
    public void HandleDataChunk(ReadOnlyMemory<byte> data);
}