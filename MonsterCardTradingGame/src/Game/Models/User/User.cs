using System;
using System.Collections.Generic;
using MonsterCardTradingGame.Cards;

namespace MonsterCardTradingGame.src
{
    public class User
    {
        private string username { get; set; }
        private string password { get; set; }
        private int coins { get; set; } = 20;
        private int token { get; set; }
        private int elo { get; set; } = 100;



        Card[] deck = new Card[3];
        List<Card> stack;
    }
}