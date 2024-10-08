using System;
using System.IO;
using System.Linq;
using System.Threading.Channels;

namespace MonsterCardTradingGame.Server
{
    public class HTTPRequest
    {
        public string Method { get; set; }
        public string Path { get; set; }
        public string Body { get; set; }

        public static HTTPRequest Parse(string request)
        {
            string[] lines = request.Split(new[] { "\r\n" }, StringSplitOptions.None);
            string[] requestLine = lines[0].Split(' ');
            string method = requestLine[0];
            string path = requestLine[1].Split('?')[0];

            int bodyIndex = 0;
            for (int i = 1; i < lines.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(lines[i]))
                {
                    bodyIndex = i + 1;
                    break;
                }
            }


            string body = bodyIndex < lines.Length ? string.Join("\r\n", lines.Skip(bodyIndex)) : string.Empty;


            return new HTTPRequest
            {
                Method = method,
                Path = path,
                Body = body
            };
        }


    }
}