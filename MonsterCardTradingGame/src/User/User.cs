using System;
using System.Collections.Generic;

namespace MonsterCardTradingGame.src
{
    public class User
    {
        private string username { get; set; }
        private string password { get; set; }
        private int coins { get; set; } = 20;
        private int token { get; set; }



        Card[] deck = new Card[3];

        List<Card> stack;
    }
}