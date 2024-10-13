using System;
using System.Net;
using MonsterCardTradingGame.Server;
using MonsterCardTradingGame.src.Screen;

namespace MonsterCardTradingGame


{
    internal class Program
    {
        public static void Main(string[] args)
        {
            HTTPServer server = new HTTPServer(IPAddress.Any, 8080);
            server.Start();
            StartScreen.Print();



        }
    }
}