using MonsterCardTradingGame.src;

namespace MonsterCardTradingGame.Cards
{
    public abstract class Card
    {
        public Element _element { get; private set; }
        public int damage { get; private set; }

        protected Card(Element element, int damage)
        {
            this._element = element;
            this.damage = damage;
        }
    }
}