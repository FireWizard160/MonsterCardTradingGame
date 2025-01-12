using MonsterCardTradingGame.Cards;
using Npgsql;

namespace MonsterCardTradingGame.Repositories;

public class PostgreSqlDeckRepository
{
    private readonly string _connectionString = "Host=localhost;Username=postgres;Password=1234;Database=mtcg";

    public int CreateDeck(int userId)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("INSERT INTO decks (user_id) VALUES (@user_id) RETURNING id", conn))
            {
                cmd.Parameters.AddWithValue("user_id", userId);
                return (int)cmd.ExecuteScalar();
            }
        }
    }

    public void AddCardToDeck(int deckId, Guid cardId)
    {
        using (var conn = new NpgsqlConnection(_connectionString))
        {
            conn.Open();
            using (var cmd = new NpgsqlCommand("INSERT INTO deck_cards (deck_id, card_id) VALUES (@deck_id, @card_id)", conn))
            {
                cmd.Parameters.AddWithValue("deck_id", deckId);
                cmd.Parameters.AddWithValue("card_id", cardId);
                cmd.ExecuteNonQuery();
            }
        }
    }



}
