namespace BestAmpUI.Client.Models;

public abstract class Response
{
    protected static Span<byte> AsHostOrderBytes(byte[] bytes, int spanStart, int spanLength)
    {
        Span<byte> span = bytes.AsSpan().Slice(spanStart, spanLength);
        if (BitConverter.IsLittleEndian)
        {
            span.Reverse();
        }
        
        return span;
    }

    public static int OpcodeFromMessage(byte[] msg)
    {
        byte[] bytes = msg[0 .. sizeof(int)];
        if (BitConverter.IsLittleEndian)
        {
            bytes = [.. bytes.Reverse()];
        }

        return BitConverter.ToInt32(bytes);
    }
}
