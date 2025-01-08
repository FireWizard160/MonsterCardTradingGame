using Npgsql;

namespace MonsterCardTradingGame.Repositories
{
    public class UserData
    {
        string _connectionString = "Host=localhost;Username=postgres;Password=1234;Database=mtcg";


        public void AddUser(string username, string password)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("INSERT INTO users (username, password) VALUES (@username, @password)", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", password);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public bool ValidateUser(string username, string password)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT COUNT(1) FROM users WHERE username = @username AND password = @password", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", password);
                    return (long)cmd.ExecuteScalar() == 1;
                }
            }
        }
    }
}