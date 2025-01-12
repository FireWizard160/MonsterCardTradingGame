using MonsterCardTradingGame.src;

namespace MonsterCardTradingGame.Cards
{
    public class MonsterCard : Card
    {
        public string MonsterType { get; set; }

        public override void SetCardType()
        {
            cardType = "monster";
        }
    }

}