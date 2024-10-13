using Npgsql;
using System;

namespace MonsterCardTradingGame.Repositories
{
    public class UserData
    {
        private string _connectionString = "Host=localhost;Username=myuser;Password=mypassword;Database=mtcgdb";

        // Adds a new user to the database (register) and stores a token.
        public bool AddUser(string username, string password)
        {
            var hashedPassword = HashPassword(password);
            var token = GenerateToken();  // Generate a token for the user

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                try
                {
                    using (var cmd = new NpgsqlCommand("INSERT INTO users (username, password, token) VALUES (@username, @password, @token)", conn))
                    {
                        cmd.Parameters.AddWithValue("username", username);
                        cmd.Parameters.AddWithValue("password", hashedPassword);
                        cmd.Parameters.AddWithValue("token", token);  // Store the generated token
                        cmd.ExecuteNonQuery();
                    }
                    return true;
                }
                catch (NpgsqlException)
                {
                    // Handle unique constraint violation (if username already exists)
                    Console.WriteLine("Failed to add user");
                    return false;
                }
            }
        }

        // Validates a user by checking if the username and hashed password match.
        public bool ValidateUser(string username, string password)
        {
            var hashedPassword = HashPassword(password);

            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();

                using (var cmd = new NpgsqlCommand("SELECT COUNT(1) FROM users WHERE username = @username AND password = @password", conn))
                {
                    cmd.Parameters.AddWithValue("username", username);
                    cmd.Parameters.AddWithValue("password", hashedPassword);
                    return (long)cmd.ExecuteScalar() == 1;
                }
            }
        }

        // Hash password using SHA-256
        private string HashPassword(string password)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(bytes).Replace("-", "").ToLower();
            }
        }

        // Generate a unique token
        private string GenerateToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
