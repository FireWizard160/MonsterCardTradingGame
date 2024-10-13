using System;
using System.Collections.Generic;
using MonsterCardTradingGame.Repositories;

namespace MonsterCardTradingGame.Services
{
    public class SessionService
    {
        private static Dictionary<string, string> _sessions = new Dictionary<string, string>();
        private static UserData _userData = new UserData();

        // Logs in a user by validating the credentials and generating a token
        public static string Login(string username, string password)
        {
            // Validate user credentials
            if (UserService.ValidateUser(username, password))
            {
                // Generate a session token
                string token = GenerateToken(username);

                // Store the session token in memory and update in the database
                _sessions[username] = token;


                return token;
            }
            return null;
        }

        // Generates a unique session token
        private static string GenerateToken(string username)
        {
            return $"{username}-{Guid.NewGuid()}";
        }
    }
}