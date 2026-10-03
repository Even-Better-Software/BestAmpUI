using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;

const int KEEP_ALIVE_INTERVAL = 30;

// this should be the address where the client is reachable
const string ALLOWED_ORIGIN = "http://127.0.0.1:7082";

// this should be the address where the bridge is reachable
const string LISTENING_ADDRESS = "http:/127.0.0.1:7081";

// this should be the port where TCP communication happens
const int TCP_PORT = 7080;

const int BUFFER_SIZE = 65536;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
WebApplication app = builder.Build();

WebSocketOptions sockOptions = new()
{
    KeepAliveInterval = TimeSpan.FromSeconds(KEEP_ALIVE_INTERVAL),
    AllowedOrigins = {ALLOWED_ORIGIN},
};
app.UseWebSockets(sockOptions);

app.Map("/backend", async context =>
{
    if (!context.WebSockets.IsWebSocketRequest)
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
        return;
    }

    using WebSocket sock = await context.WebSockets.AcceptWebSocketAsync();

    using TcpClient tcp = new() {NoDelay = true};
    await tcp.ConnectAsync(IPAddress.Loopback, TCP_PORT, context.RequestAborted);
    using NetworkStream tcpStream = tcp.GetStream();

    using CancellationTokenSource cancelTokenSrc = 
            CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);
    Task browserToTcp = ForwardBrowserToTcpAsync(sock, tcpStream, cancelTokenSrc.Token);
    Task tcpToBrowser = ForwardTcpToBrowserAsync(tcpStream, sock, cancelTokenSrc.Token);
    
    try
    {
        await Task.WhenAny(browserToTcp, tcpToBrowser);
    }
    catch (OperationCanceledException)
    {
        // this happens on a disconnect
        Console.Out.WriteLine("Connection disconnected normally.");
    }
    finally
    {
        cancelTokenSrc.Cancel();
        try
        {
            await sock.CloseAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        }
        catch (Exception err)
        {
            // tried to close connection twice?
            Console.Error.WriteLine(err);
        }
    }
});

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapFallbackToFile("index.html");
app.Run(LISTENING_ADDRESS);

app.MapGet("/", () => "Hello World!");

app.Run();

/**
 * Listens for TCP traffic. Forwards it to the WebSocket connection.
 */
static async Task ForwardTcpToBrowserAsync(
    NetworkStream tcpStream, 
    WebSocket sock, 
    CancellationToken cancelToken)
{
    byte[] buffer = new byte[BUFFER_SIZE];
    while (sock.State == WebSocketState.Open)
    {
        WebSocketReceiveResult result = await sock.ReceiveAsync(
            buffer, cancelToken
        );

        if (result.MessageType == WebSocketMessageType.Close)
        {
            return;
        }

        if (result.MessageType != WebSocketMessageType.Binary)
        {
            // ignore messages not with binary format
            continue;
        }

        await tcpStream.WriteAsync(buffer.AsMemory(0, result.Count), cancelToken);
    }
}

/**
 * Listens for WebSocket traffic. Forwards it to the TCP connection.
 */
static async Task ForwardBrowserToTcpAsync(
    WebSocket sock, 
    NetworkStream tcpStream, 
    CancellationToken cancelToken)
{
    byte[] buffer = new byte[BUFFER_SIZE];
    while (sock.State == WebSocketState.Open)
    {
        int count = await tcpStream.ReadAsync(buffer, cancelToken);

        if (count == 0)
        {
            // tcp packet with 0 length --> close connection
            return;
        }

        await sock.SendAsync(
            buffer.AsMemory(0, count), 
            WebSocketMessageType.Binary,
            endOfMessage: true,
            cancelToken
        );
    }
}
