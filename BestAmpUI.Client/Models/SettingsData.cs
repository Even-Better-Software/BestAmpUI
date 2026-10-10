namespace BestAmpUI.Client.Models;

public class SettingsData
{
    public string HostApi { get; set; } = "";
    public string DeviceIn { get; set; } = "";
    public int ChannelIn { get; set; } = 0;
    public string DeviceOut { get; set; } = "";
    public int ChannelOut { get; set; } = 0;
    public int SampleRate { get; set; } = 0;
    public int FramesPerBuffer { get; set; } = 0;
}
