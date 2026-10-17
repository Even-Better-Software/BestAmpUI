using System.Text;

namespace BestAmpUI.Client.Models;

public class HostApisResponse : Response
{
    private const int OPCODE = 3;

    public Dictionary<int, string> HostApis = [];

    public HostApisResponse(byte[] msg)
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
        int index = 0;
        while (start < msg.Length)
        {
            int hostApiId = BitConverter.ToInt32(AsHostOrderBytes(msg, start, sizeof(int)));
            start += sizeof(int);

            builder.Clear();
            while (msg[start] != '\0')
            {
                builder.Append(msg[start++]);
            }
            start++;
            string hostApiName = builder.ToString();
            
            HostApis.Add(hostApiId, hostApiName);

            index++;
        }
       
    }
}
