using MonsterCardTradingGame.Cards;
using MonsterCardTradingGame.src;
using Npgsql;

namespace MonsterCardTradingGame.Repositories
{
    public class PostgreSqlBattleRepository
    {
        private readonly string _connectionString = "Host=localhost;Username=postgres;Password=1234;Database=mtcg";

        public void SaveBattleLogToDatabase(int player1Id, int player2Id, int? winnerId, List<string> battleLog)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                           @"INSERT INTO battles (player1_id, player2_id, winner_id, battle_log, created_at) 
                     VALUES (@player1_id, @player2_id, @winner_id, @battle_log, @created_at)",
                           conn))
                {
                    cmd.Parameters.AddWithValue("player1_id", player1Id);
                    cmd.Parameters.AddWithValue("player2_id", player2Id);
                    cmd.Parameters.AddWithValue("winner_id", winnerId.HasValue ? (object)winnerId.Value : DBNull.Value); // Handle null for draws
                    cmd.Parameters.AddWithValue("battle_log", string.Join("\n", battleLog));
                    cmd.Parameters.AddWithValue("created_at", DateTime.UtcNow); // Use the current UTC timestamp
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public List<Card> GetDeckByUserId(int userId)
        {
            var deck = new List<Card>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                           "SELECT id, damage, element_type, card_type, monster_type FROM cards WHERE owner_id = @user_id AND is_in_deck = TRUE",
                           conn))
                {
                    cmd.Parameters.AddWithValue("user_id", userId);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string cardType = reader.GetString(3); // "Monster" or "Spell"
                            if (cardType == "monster")
                            {
                                // Create a MonsterCard and set the monster type
                                var monsterCard = new MonsterCard
                                {
                                    id = reader.GetGuid(0),
                                    damage = reader.GetInt32(1),
                                    _element = Enum.Parse<Element>(reader.GetString(2)),
                                    cardType = "monster",
                                    MonsterType = reader.GetString(4) // Read the monster type as a string
                                };


                                deck.Add(monsterCard);
                            }
                            else if (cardType == "spell")
                            {
                                // Create a SpellCard
                                var spellCard = new SpellCard
                                {
                                    id = reader.GetGuid(0),
                                    damage = reader.GetInt32(1),
                                    _element = Enum.Parse<Element>(reader.GetString(2)),
                                    cardType = "spell"
                                };
                                deck.Add(spellCard);
                            }
                        }
                    }
                }
            }

            return deck;
        }



        public int? GetRandomOpponentId(int excludeUserId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                           @"
                   SELECT d.user_id 
                   FROM decks d
                   JOIN deck_cards dc ON d.id = dc.deck_id
                   WHERE d.user_id != @exclude_user
                   GROUP BY d.user_id
                   HAVING COUNT(dc.card_id) = 4
                   ORDER BY RANDOM()
                   LIMIT 1",
                           conn))
                {
                    cmd.Parameters.AddWithValue("exclude_user", excludeUserId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            int userId = reader.GetInt32(0);
                            Console.WriteLine("Query Output: " + userId);
                            return userId; // Return the user_id of the selected opponent
                        }
                    }
                }
            }
            Console.WriteLine("Query Output: No opponent found.");
            return null; // Return null if no opponent is found
        }

        public void UpdateElo(int userId, int eloChange)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                           "UPDATE users SET elo = elo + @eloChange WHERE id = @userId",
                           conn))
                {
                    cmd.Parameters.AddWithValue("eloChange", eloChange);
                    cmd.Parameters.AddWithValue("userId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void LevelUpCards(List<Card> deck)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                foreach (var card in deck)
                {
                    card.damage += 5; // Update the card's damage in memory
                    using (var cmd = new NpgsqlCommand("UPDATE cards SET damage = damage + 5 WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("id", card.id);
                        cmd.ExecuteNonQuery(); // Persist the change in the database
                    }
                }
            }
        }

        public void UpdateUserStatistics(int userId, bool isWinner)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                string sql = isWinner
                    ? "UPDATE users SET wins = wins + 1, games_played = games_played + 1 WHERE id = @userId"
                    : "UPDATE users SET losses = losses + 1, games_played = games_played + 1 WHERE id = @userId";

                using (var cmd = new NpgsqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("userId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }



}
