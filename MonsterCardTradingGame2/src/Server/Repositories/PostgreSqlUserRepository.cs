using Npgsql;

namespace MonsterCardTradingGame.Repositories
{
    public class PostgreSqlUserRepository
    {
        private readonly string _connectionString = "Host=localhost;Username=postgres;Password=1234;Database=mtcg";

        // Add a user with plaintext password
        public bool AddUser(string username, string password)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                    "INSERT INTO users (username, password) VALUES (@username, @password) ON CONFLICT (username) DO NOTHING", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", password); // Store as plaintext
                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // Validate user with plaintext password comparison
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

        // Update session token in the users table
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

        // Retrieve session token from the users table
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
    }
}
