using System;
using System.Net;
using MonsterCardTradingGame.Repositories;
using MonsterCardTradingGame.Server;
using MonsterCardTradingGame.src.Screen;

namespace MonsterCardTradingGame


{
    internal class Program
    {
        public static void Main(string[] args)
        {
            DatabaseTest dbTest = new DatabaseTest();
            bool canConnect = dbTest.TestConnection();


            if (canConnect)
            {
                Console.WriteLine("Database connection test passed.");
            }
            else
            {
                Console.WriteLine("Database connection test failed.");
            }
            dbTest.OutputUsersTable();

            HTTPServer server = new HTTPServer(IPAddress.Any, 8080);
            server.Start();
            StartScreen.Print();



        }
    }
}