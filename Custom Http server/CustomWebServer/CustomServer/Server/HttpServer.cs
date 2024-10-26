using System.Net;
using System.Net.Sockets;
using System.Text;

namespace CustomServer.Server;

public class HttpServer
{
    private readonly string _ipAddress;
    private readonly int _port;
    public HttpServer(string ipAddress, int port)
    {
        _ipAddress = ipAddress;
        _port = port;
    }

    public void Start()
    {
        var listenerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        listenerSocket.Bind(new IPEndPoint(IPAddress.Parse(_ipAddress), _port));
        listenerSocket.Listen(10);

        Console.WriteLine($"Server started at http://{_ipAddress}:{_port}");

        while (true)
        {
            var clientSocket = listenerSocket.Accept();
            Task.Run(() => HandleClient(clientSocket));
        }
    }

    private void HandleClient(Socket clientSocket)
    {
        using (clientSocket)
        {
            // Read the request
            var buffer = new byte[1024];
            int bytesRead = clientSocket.Receive(buffer);
            var requestText = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received request:\n{requestText}");

            // Process the request and generate a response
            var response = GenerateHttpResponse("Processed successfully");

            // Send the response back to the client
            var responseBytes = Encoding.UTF8.GetBytes(response);
            clientSocket.Send(responseBytes);
        }
    }

    private static string GenerateHttpResponse(string content, int statusCode = 200)
    {
        return $"HTTP/1.1 {statusCode} {(statusCode == 200 ? "OK" : "Not Found")}\r\n" +
               "Content-Type: text/plain\r\n" +
               $"Content-Length: {content.Length}\r\n" +
               "\r\n" +
               content;
    }

}
