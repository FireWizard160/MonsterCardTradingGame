using Npgsql;
using System;

namespace MonsterCardTradingGame.Repositories
{
    public class DatabaseTest
    {
        private string _connectionString = "Host=localhost;Username=myuser;Password=mypassword;Database=mtcgdb";

        public bool TestConnection()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();  // Try to open the connection
                    Console.WriteLine("Connection to the database succeeded!");



                    return true;  // If successful, return true
                }
                catch (Exception ex)  // Catch any errors that occur during the connection attempt
                {
                    Console.WriteLine($"Failed to connect to the database: {ex.Message}");
                    return false;  // If failed, return false
                }
                finally
                {
                    conn.Close();  // Ensure the connection is closed after testing
                }
            }
        }

        public void OutputUsersTable()
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                try
                {
                    conn.Open();  // Open the connection to the database

                    // SQL query to get all data from the users table
                    string query = "SELECT username, password, token FROM users";
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())  // Execute the query and get the result
                        {
                            Console.WriteLine("Users table contents:");
                            Console.WriteLine("-----------------------------");

                            // Iterate through the result set and output each row
                            while (reader.Read())
                            {
                                string username = reader.GetString(0);
                                string password = reader.GetString(1);
                                string token = reader.IsDBNull(2) ? "NULL" : reader.GetString(2);  // Handle NULL tokens

                                Console.WriteLine($"Username: {username}, Password: {password}, Token: {token}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching data from users table: {ex.Message}");
                }
                finally
                {
                    conn.Close();  // Ensure the connection is closed
                }
            }
        }
    }
}