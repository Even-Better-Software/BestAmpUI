namespace BestAmpUI.Client.Models;

public class DialKnobsData
{
    private const int OPCODE = 2;
    private const int CONTENT_LENGTH = 5 * sizeof(float);
    public int Voice { get; set; } = 0;
    public int Gain { get; set; } = 0;
    public int Volume { get; set; } = 0;
    public int Bass { get; set; } = 0;
    public int Treble { get; set; } = 0;

    public byte[] BuildMessage()
    {
        List<byte> msg = [];

        AppendAsNetworkOrder(msg, BitConverter.GetBytes(OPCODE));
        AppendAsNetworkOrder(msg, BitConverter.GetBytes(CONTENT_LENGTH));
        AppendAsNetworkOrder(msg, BitConverter.GetBytes(Gain));
        AppendAsNetworkOrder(msg, BitConverter.GetBytes(Bass));
        AppendAsNetworkOrder(msg, BitConverter.GetBytes(Treble));
        AppendAsNetworkOrder(msg, BitConverter.GetBytes(Volume));
        AppendAsNetworkOrder(msg, BitConverter.GetBytes(Voice));

        return [.. msg];
    }

    private static void AppendAsNetworkOrder(List<byte> msg, byte[] bytes)
    {
        msg.AddRange(BitConverter.IsLittleEndian ? bytes.Reverse() : bytes);
    }
}
