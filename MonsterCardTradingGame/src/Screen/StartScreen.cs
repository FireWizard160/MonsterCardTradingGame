using System;

namespace MonsterCardTradingGame.src.Screen
{
    public static class StartScreen
    {




        public static void Print()
        {
            Console.Clear();
            Console.WriteLine("Welcome to the Monster Card Trading Game");
            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            MainMenuScreen.Print();




        }


    }
}