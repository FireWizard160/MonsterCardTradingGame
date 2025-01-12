using MonsterCardTradingGame.Cards;
using MonsterCardTradingGame.src;
using Npgsql;

namespace MonsterCardTradingGame.Repositories
{
    public class PostgreSqlUserRepository
    {
        private readonly string _connectionString = "Host=localhost;Username=postgres;Password=1234;Database=mtcg";




        public bool AddUser(string username, string password)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "INSERT INTO users (username, password) VALUES (@username, @password) ON CONFLICT (username) DO NOTHING", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", password);
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }


        public bool ValidateUser(string username, string password)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "SELECT COUNT(*) FROM users WHERE username = @username AND password = @password", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", password); // Compare plaintext
                    return (long)cmd.ExecuteScalar() > 0;
                }
            }
        }


        public void AddSession(string username, string token)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "UPDATE users SET session_token = @token WHERE username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("token", token);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public string GetSessionToken(string username)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "SELECT session_token FROM users WHERE username = @username", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    return cmd.ExecuteScalar()?.ToString();
                }
            }
        }


        public User GetUserBySessionToken(string sessionToken)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                           "SELECT id, username, coins, elo, games_played, wins, losses FROM users WHERE session_token = @sessionToken", conn))
                {
                    cmd.Parameters.AddWithValue("sessionToken", sessionToken);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new User
                            {
                                id = reader.GetInt32(0),
                                username = reader.GetString(1),
                                coins = reader.GetInt32(2),
                                elo = reader.GetInt32(3),
                                gamesPlayed = reader.GetInt32(4),
                                wins = reader.GetInt32(5),
                                losses = reader.GetInt32(6)
                            };
                        }
                    }
                }
            }
            return null;
        }

        // Check if the new username is available
        public bool IsUsernameAvailable(string newUsername)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT COUNT(*) FROM users WHERE username = @newUsername", conn))
                {
                    cmd.Parameters.AddWithValue("newUsername", newUsername);
                    return (long)cmd.ExecuteScalar() == 0;  // Returns true if username is available
                }
            }
        }


        public void UpdateUsername(string sessionToken, string newUsername)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                           "UPDATE users SET username = @newUsername WHERE session_token = @sessionToken", conn))
                {
                    cmd.Parameters.AddWithValue("sessionToken", sessionToken);
                    cmd.Parameters.AddWithValue("newUsername", newUsername);
                    cmd.ExecuteNonQuery();
                }
            }
        }


        public List<User> GetUsersSortedByElo()
        {
            var users = new List<User>();

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                           "SELECT id, username, coins, elo, games_played, wins, losses FROM users ORDER BY elo DESC", conn))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new User
                            {
                                id = reader.GetInt32(0),
                                username = reader.GetString(1),
                                coins = reader.GetInt32(2),
                                elo = reader.GetInt32(3),
                                gamesPlayed = reader.GetInt32(4),
                                wins = reader.GetInt32(5),
                                losses = reader.GetInt32(6)
                            });
                        }
                    }
                }
            }

            return users;
        }

        public void DeductCoins(int userId, int amount)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                           "UPDATE users SET coins = coins - @amount WHERE id = @userId AND coins >= @amount", conn))
                {
                    cmd.Parameters.AddWithValue("amount", amount);
                    cmd.Parameters.AddWithValue("userId", userId);
                    cmd.ExecuteNonQuery();
                }
            }
        }






    }
}
