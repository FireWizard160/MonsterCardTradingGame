using System;
using System.Collections.Generic;
using MonsterCardTradingGame.Cards;

namespace MonsterCardTradingGame.src
{
    public class User
    {
        internal int id { get; set; }
        internal string username { get; set; }
        private string password { get; set; }
        internal int coins { get; set; } = 20;
        private int token { get; set; }
        internal int losses { get; set; } = 0;
        internal int wins { get; set; } = 0;
        internal int elo { get; set; } = 100;
        internal int gamesPlayed { get; set; } = 0;



        Card[] deck = new Card[3];
        private List<Card> cardCollection;


    }
}