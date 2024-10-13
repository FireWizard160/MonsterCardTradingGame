using System;
using MonsterCardTradingGame.Cards;
using MonsterCardTradingGame.src;

namespace MonsterCardTradingGame.Cards
{
    public class Battle
    {
        // Define effectiveness rules
        public double GetEffectiveness(Element attackerElement, Element defenderElement)
        {
            if (attackerElement == Element.Water && defenderElement == Element.Fire)
                return 2.0; // Water -> Fire
            else if (attackerElement == Element.Fire && defenderElement == Element.Normal)
                return 2.0; // Fire -> Normal
            else if (attackerElement == Element.Normal && defenderElement == Element.Water)
                return 2.0; // Normal -> Water
            else if (attackerElement == Element.Fire && defenderElement == Element.Water)
                return 0.5; // Fire -> Water
            // Add more cases if needed...
            return 1.0; // No special effectiveness
        }

        public int CalculateDamage(Card attacker, Card defender)
        {
            // Check for special cases based on specific monster types
            if (attacker is Goblin && defender is Dragon)
            {
                Console.WriteLine("Goblins are too afraid to attack Dragons!");
                return 0; // Goblin won't attack Dragon
            }

            else if (attacker is Wizard && defender is Ork)
            {
                Console.WriteLine("Wizards control Orks, no damage!");
                return 0; // Wizard controls Ork
            }

            else if (attacker is SpellCard && defender is Knight && attacker._element == Element.Water)
            {
                Console.WriteLine("Knights drown instantly from WaterSpells!");
                return 100000000; // Knight drowns instantly from WaterSpell
            }

            else if (attacker is SpellCard && defender is Kraken)
            {
                Console.WriteLine("The Kraken is immune to spells!");
                return 0; // Kraken is immune to spells
            }

            else if (attacker is Dragon && defender is FireElf)
            {
                Console.WriteLine("FireElves can evade Dragon attacks!");
                return 0; // FireElf evades Dragon attacks
            }

            // If no special case, calculate normal damage
            double effectiveness = GetEffectiveness(attacker._element, defender._element);
            return (int)(attacker.damage * effectiveness);
        }
    }
}
