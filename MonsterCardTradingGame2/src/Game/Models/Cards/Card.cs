using MonsterCardTradingGame.src;

namespace MonsterCardTradingGame.Cards
{
    public abstract class Card
    {
        public Guid id { get; set; }
        public int damage { get; set; }
        public Element _element { get; set; }
        public string cardType { get; set; }
        public int? ownerId { get; set; }
        public bool isInDeck { get; set; }


        public abstract void SetCardType();  // For setting the card type (Monster or Spell)
    }

}