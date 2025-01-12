using NUnit.Framework;
using System.Collections.Generic;
using MonsterCardTradingGame.Cards;
using MonsterCardTradingGame.Repositories;
using MonsterCardTradingGame.src;
using Moq;
using Npgsql;

namespace MonsterCardTradingGame.Tests
{
    [TestFixture]
    public class BattleTests
    {
        private Battle _battle;


        [SetUp]
        public void SetUp()
        {
            _battle = new Battle();
        }

        [Test]
        public void TestEffectiveness_FireAgainstWater_IsNotVeryEffective()
        {
            var fireCard = new MonsterCard { damage = 60, _element = Element.fire };
            var waterCard = new MonsterCard { damage = 80, _element = Element.water };

            string ruleLog;
            var damage = _battle.CalculateDamage(fireCard, waterCard, out ruleLog);

            // Assert that damage is halved when Fire attacks Water (low effectiveness)
            Assert.That(damage, Is.EqualTo(30), "The damage should be halved due to Fire's low effectiveness against Water.");
        }

        [Test]
        public void TestEffectiveness_WaterAgainstFire_IsSuperEffective()
        {
            var waterCard = new MonsterCard { damage = 60, _element = Element.water };
            var fireCard = new MonsterCard { damage = 80, _element = Element.fire };

            string ruleLog;
            var damage = _battle.CalculateDamage(waterCard, fireCard, out ruleLog);

            // Assert that water against fire is super effective (damage is doubled)
            Assert.That(damage, Is.EqualTo(120), "Water against Fire should be super effective.");
            Assert.That(ruleLog, Does.Contain("It's super effective!"), "Rule log should indicate that the attack is super effective.");
        }

        [Test]
        public void TestBattleLog_SavesLogToDatabase()
        {
            var userDeck = new List<Card>
            {
                new MonsterCard { damage = 50, _element = Element.fire }
            };

            var opponentDeck = new List<Card>
            {
                new MonsterCard { damage = 40, _element = Element.water }
            };


            var response = _battle.StartBattle(userDeck, opponentDeck, 1, 2);

            // Assert that the battle log contains the word 'Your' to indicate the user's card
            Assert.That(response.Body, Does.Contain("{\"BattleLog\":[\"Round 1: Opponent's  (water - 40 damage) defeated your  (fire - 50 damage).  It's super effective!\"],\"Result\":\"Opponent wins the battle.\"}"), "Battle log should contain 'Your' to indicate the user's card");
        }

        [Test]
        public void TestBattleEndsInDraw_WhenDecksAreEqual()
        {
            var userDeck = new List<Card>
            {
                new MonsterCard { damage = 50, _element = Element.fire },

            };

            var opponentDeck = new List<Card>
            {
                new MonsterCard { damage = 50, _element = Element.fire },

            };

            var response = _battle.StartBattle(userDeck, opponentDeck, 1, 2);

            // Assert that the result contains "The battle ends in a draw"
            Assert.That(response.Body, Does.Contain("The battle ends in a draw"), "The battle should end in a draw if the decks are equal.");
        }

        [Test]
        public void TestEloChangesAfterBattle()
        {
            var userDeck = new List<Card>
            {
                new MonsterCard { damage = 50, _element = Element.fire }
            };

            var opponentDeck = new List<Card>
            {
                new MonsterCard { damage = 40, _element = Element.water }
            };

            // Assume userId is 1 and opponentId is 2
            var response = _battle.StartBattle(userDeck, opponentDeck, 1, 2);

            // Verify that the Elo change is in accordance with the results

            Assert.That(response.Body, Does.Contain("{\"BattleLog\":[\"Round 1: Opponent's  (water - 40 damage) defeated your  (fire - 50 damage).  It's super effective!\"],\"Result\":\"Opponent wins the battle.\"}"), "The battle should indicate that the user wins.");
        }



    }
}
