using MonsterCardTradingGame.Cards;

namespace MonsterCardTradingGame.src
{
    public class SpellCard : Card
    {

        int damage = 0;
        private char elementType;


        public SpellCard(Element element, int damage, char elementType) : base(element, damage)
        {
            this.damage = damage;
            this.elementType = elementType;
        }
    }
}