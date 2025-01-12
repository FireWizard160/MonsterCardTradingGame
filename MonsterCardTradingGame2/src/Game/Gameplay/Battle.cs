using MonsterCardTradingGame.Repositories;
using MonsterCardTradingGame.Server;
using MonsterCardTradingGame.src;
using Newtonsoft.Json;

namespace MonsterCardTradingGame.Cards
{
    public class Battle
    {
        public List<string> BattleLog { get; private set; } = new List<string>();

        public HTTPResponse StartBattle(List<Card> userDeck, List<Card> opponentDeck, int userId, int opponentId)
        {
            Random random = new Random();
            int maxRounds = 100;

            for (int round = 0; round < maxRounds && userDeck.Count > 0 && opponentDeck.Count > 0; round++)
            {
                // Select random cards from each deck
                Card userCard = userDeck[random.Next(userDeck.Count)];
                Card opponentCard = opponentDeck[random.Next(opponentDeck.Count)];

                string userRuleLog, opponentRuleLog;

                int userDamage = CalculateDamage(userCard, opponentCard, out userRuleLog);
                int opponentDamage = CalculateDamage(opponentCard, userCard, out opponentRuleLog);

                if (userDamage > opponentDamage)
                {
                    BattleLog.Add(
                        $"Round {round + 1}: Your {DescribeCard(userCard)} defeated opponent's {DescribeCard(opponentCard)}. {userRuleLog}");
                    opponentDeck.Remove(opponentCard); // Opponent's card is removed
                }
                else if (opponentDamage > userDamage)
                {
                    BattleLog.Add(
                        $"Round {round + 1}: Opponent's {DescribeCard(opponentCard)} defeated your {DescribeCard(userCard)}. {opponentRuleLog}");
                    userDeck.Remove(userCard); // User's card is removed
                }
                else
                {
                    BattleLog.Add(
                        $"Round {round + 1}: It's a draw between your {DescribeCard(userCard)} and opponent's {DescribeCard(opponentCard)}. {userRuleLog} {opponentRuleLog}");
                }
            }

            string result;
            int userEloChange = 0;
            int opponentEloChange = 0;
            int? winnerId = null;

            if (userDeck.Count > opponentDeck.Count)
            {
                result = "User wins the battle.";
                winnerId = userId;
                userEloChange = 10;
                opponentEloChange = -5;
            }
            else if (opponentDeck.Count > userDeck.Count)
            {
                result = "Opponent wins the battle.";
                winnerId = opponentId;
                userEloChange = -5;
                opponentEloChange = 10;
            }
            else
            {
                result = "The battle ends in a draw.";
            }

            // Save battle log, update Elo ratings, and handle card leveling
            var battleRepository = new PostgreSqlBattleRepository();
            battleRepository.SaveBattleLogToDatabase(userId, opponentId, winnerId, BattleLog);
            battleRepository.UpdateElo(userId, userEloChange);
            battleRepository.UpdateElo(opponentId, opponentEloChange);

            if (winnerId == userId)
            {
                battleRepository.LevelUpCards(userDeck); // Level up the winner's cards
            }
            else if (winnerId == opponentId)
            {
                battleRepository.LevelUpCards(opponentDeck); // Level up the winner's cards
            }

            // Return response
            return new HTTPResponse(200, JsonConvert.SerializeObject(new { BattleLog, Result = result }));
        }


        public int CalculateDamage(Card attacker, Card defender, out string ruleLog)
        {
            ruleLog = "";

            // Implement special rules
            if (attacker is MonsterCard attackerMonster && defender is MonsterCard defenderMonster)
            {
                if (attackerMonster.MonsterType == "Goblin" && defenderMonster.MonsterType == "Dragon")
                {
                    ruleLog = "Goblin is too afraid of Dragon to attack.";
                    return 0; // Goblin can't attack Dragon
                }

                if (attackerMonster.MonsterType == "Wizard" && defenderMonster.MonsterType == "Ork")
                {
                    ruleLog = "Wizard controls Ork, preventing it from dealing damage.";
                    return 0; // Wizard neutralizes Ork
                }

                if (attackerMonster.MonsterType == "FireElf" && defenderMonster.MonsterType == "Dragon")
                {
                    ruleLog = "FireElf evades Dragon's attack.";
                    return 0; // FireElf evades Dragon
                }
            }

            if (defender is MonsterCard defenderKnight && defenderKnight.MonsterType == "Knight" &&
                attacker is SpellCard attackerSpell && attackerSpell._element == Element.water)
            {
                ruleLog = "Knight drowns instantly from WaterSpell.";
                return int.MaxValue; // Instant defeat for Knight
            }

            if (defender is MonsterCard defenderKraken && attacker is SpellCard)
            {
                ruleLog = "Kraken is immune to spells.";
                return 0; // Kraken is immune to spells
            }

            // Default damage calculation with effectiveness
            double effectiveness = GetEffectiveness(attacker._element, defender._element);
            ruleLog += effectiveness > 1.0 ? " It's super effective!" :
                effectiveness < 1.0 ? " It's not very effective." : "";
            return (int)(attacker.damage * effectiveness);
        }

        private double GetEffectiveness(Element attackerElement, Element defenderElement)
        {
            if (attackerElement == Element.water && defenderElement == Element.fire) return 2.0;
            if (attackerElement == Element.fire && defenderElement == Element.normal) return 2.0;
            if (attackerElement == Element.normal && defenderElement == Element.water) return 2.0;
            if (attackerElement == Element.fire && defenderElement == Element.water) return 0.5;
            return 1.0;
        }

        private string DescribeCard(Card card)
        {
            string typeDescription = card is MonsterCard monster ? $"{monster.MonsterType}" : "Spell";
            return $"{typeDescription} ({card._element} - {card.damage} damage)";
        }
    }
}