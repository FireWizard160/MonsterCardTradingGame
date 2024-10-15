using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using MonsterCardTradingGame.Controllers;

namespace MonsterCardTradingGame.Server
{
    public class HTTPServer
    {
        private TcpListener _listener;

        public HTTPServer(IPAddress ipAddress, int port)
        {
            _listener = new TcpListener(ipAddress, port);
        }

        public void Start()
        {
            _listener.Start();
            Console.WriteLine("HTTP Server is running...");

            while (true)
            {
                var client = _listener.AcceptTcpClient();
                HandleClient(client);
            }
        }

        private void HandleClient(TcpClient client)
        {
            using (var stream = client.GetStream())
            {
                StreamReader reader = new StreamReader(stream);
                StreamWriter writer = new StreamWriter(stream);


                //string requestLine = reader.ReadLine();
                byte[] buffer = new byte[client.ReceiveBufferSize];
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                if (bytesRead == 0)
                    return;

                string requestLine = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                Console.WriteLine("Received request: " + requestLine);


                if (!string.IsNullOrEmpty(requestLine))
                {


                    HTTPRequest request = HTTPRequest.Parse(requestLine);
                    HTTPResponse response = RouteRequest(request);


                    SendResponse(writer, response);
                }
                else
                {
                    Console.WriteLine("Received empty request");
                }

                client.Close();
            }
        }

        private HTTPResponse RouteRequest(HTTPRequest request)
        {
            if (request.Method == "POST" && request.Path == "/login")
            {
                Console.WriteLine("Received request: " + request.Path);
                return SessionController.HandleLogin(request);
            }
            else if (request.Method == "POST" && request.Path == "/register")
            {
                return UserController.HandleRegistration(request);

            }
            else
            {
                return new HTTPResponse(404, "{\"error\": \"Not Found\"}");
            }
        }

        private void SendResponse(StreamWriter writer, HTTPResponse response)
        {
            writer.WriteLine($"HTTP/1.1 {response.StatusCode} OK");
            writer.WriteLine("Content-Type: application/json");
            writer.WriteLine($"Content-Length: {response.Body.Length}");
            writer.WriteLine();
            writer.WriteLine(response.Body);
            writer.Flush();
        }
    }
}
