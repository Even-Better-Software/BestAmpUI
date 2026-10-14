namespace BestAmpUI.Client.Models;

public class SettingsData
{
    private const int OPCODE = 1;
    public int HostApi { get; set; } = 0;
    public int DeviceIn { get; set; } = 0;
    public int ChannelIn { get; set; } = 0;
    public int DeviceOut { get; set; } = 0;
    public int ChannelOut { get; set; } = 0;
    public int SampleRate { get; set; } = 0;
    public int FramesPerBuffer { get; set; } = 0;
    public int SampleFormat { get; set; } = 0;

     public byte[] BuildMessage()
    {
        List<byte> msg = [];

        AppendAsNetworkOrder(msg, BitConverter.GetBytes(OPCODE));
        AppendAsNetworkOrder(msg, BitConverter.GetBytes(GetContentLength()));
        AppendAsNetworkOrder(msg, BitConverter.GetBytes(HostApi));
        AppendAsNetworkOrder(msg, BitConverter.GetBytes(DeviceIn));
        AppendAsNetworkOrder(msg, BitConverter.GetBytes(DeviceOut));
        AppendAsNetworkOrder(msg, BitConverter.GetBytes(ChannelIn));
        AppendAsNetworkOrder(msg, BitConverter.GetBytes(ChannelOut));
        AppendAsNetworkOrder(msg, BitConverter.GetBytes(FramesPerBuffer));
        AppendAsNetworkOrder(msg, BitConverter.GetBytes(SampleRate));
        // no sampleformat yet

        return [.. msg];
    }

    private int GetContentLength()
    {
        int length = 0;

// TODO
        return length;
    }

    private static void AppendAsNetworkOrder(List<byte> msg, byte[] bytes)
    {
        msg.AddRange(BitConverter.IsLittleEndian ? bytes.Reverse() : bytes);
    }
}
