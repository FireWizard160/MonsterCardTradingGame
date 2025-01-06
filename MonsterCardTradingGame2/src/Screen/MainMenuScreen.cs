using System;
using System.Threading;

namespace MonsterCardTradingGame.src.Screen
{
    public static class MainMenuScreen

    {

    public static void Print()
    {
        Console.Clear();

        Console.WriteLine("\nWrite login to log in to your existting Account");
        Console.WriteLine("\nWrite register to register a new account");


        handleMainMenuInput(readMainMenuInput());

    }

    static string readMainMenuInput()
    {
        return Console.ReadLine();

    }

    static void handleMainMenuInput(string userinput)
    {

        if (userinput.Equals("login"))
        {
            LoginScreen.printLoginScreen();
            return;
        }

        if (userinput.Equals("register"))
        {
            RegisterScreen.printRegisterScreen();
            return;
        }

        Console.WriteLine("Invalid input, please try again");
        Thread.Sleep(1000);
        Print();

    }


    }


}