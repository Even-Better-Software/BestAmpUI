using System.Text;

namespace BestAmpUI.Client.Models;

public class DeviceIdsResponse : Response
{
    public const int OPCODE = 1005;

    public Dictionary<int, DeviceInfo> Devices = [];

    public DeviceIdsResponse(byte[] msg)
    {
        int start = 0;

        int OpCode = BitConverter.ToInt32(AsHostOrderBytes(msg, start, sizeof(int)));
        start += sizeof(int);

        if (OpCode != OPCODE)
        {
            Console.WriteLine($"Error: Received msg w/ opcode {OpCode} but expected {OPCODE}");
            return;
        }

        int contentLength = BitConverter.ToInt32(AsHostOrderBytes(msg, start, sizeof(int)));
        start += sizeof(int);

        if (contentLength != (msg.Length - start))
        {
            Console.WriteLine($"Error: Mismatching content length.");
        }

        StringBuilder builder = new();
        while (start < msg.Length)
        {
            if (start + 1 >= msg.Length && msg[start] == '\0')
            {
                Console.WriteLine("Extra \\0");
                break; // end of message
            }

            DeviceInfo device = new();

            int deviceId = BitConverter.ToInt32(AsHostOrderBytes(msg, start, sizeof(int)));
            start += sizeof(int);

            builder.Clear();
            while (msg[start] != '\0')
            {
                builder.Append((char)msg[start++]);
            }
            start++;
            device.Name = builder.ToString();
            
            device.MaxInChannel = BitConverter.ToInt32(AsHostOrderBytes(msg, start, sizeof(int)));
            start += sizeof(int);

            device.MaxOutChannel = BitConverter.ToInt32(AsHostOrderBytes(msg, start, sizeof(int)));
            start += sizeof(int);

            device.SampleRate = BitConverter.ToInt32(AsHostOrderBytes(msg, start, sizeof(int)));
            start += sizeof(int);

            Devices[deviceId] = device;
        }
       
    }
}
