using System.Text;

namespace BestAmpUI.Client.Models;

public class DefaultHostApiResponse : Response
{
    private const int OPCODE = 4;
    public int HostApiId { get; private set; } = 0;
    public string HostApiName { get; private set; } = "";

    public DefaultHostApiResponse(byte[] msg)
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

        HostApiId = BitConverter.ToInt32(AsHostOrderBytes(msg, start, sizeof(int)));
        start += sizeof(int);

        StringBuilder builder = new();
        while (msg[start] != '\0')
        {
            builder.Append(msg[start++]);
        }
        HostApiName = builder.ToString();
    }
}
