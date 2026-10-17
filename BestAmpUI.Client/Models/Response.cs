namespace BestAmpUI.Client.Models;

public abstract class Response
{
    protected static Span<byte> AsHostOrderBytes(byte[] bytes, int spanStart, int spanLength)
    {
        Span<byte> span = bytes.AsSpan()[spanStart .. spanLength];
        if (BitConverter.IsLittleEndian)
        {
            span.Reverse();
        }
        
        return span;
    }
}
