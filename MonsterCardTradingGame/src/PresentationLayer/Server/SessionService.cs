using System;
using System.Collections.Generic;
using System.Data;

namespace MonsterCardTradingGame.Services
{
    public class SessionService
    {
        private static Dictionary<string, string> _sessions = new Dictionary<string, string>();

        public static string Login(string username, string password)
        {
            // Validate user credentials (use UserService)
            if (UserService.ValidateUser(username, password))
            {
                string token = GenerateToken(username);
                _sessions[username] = token;
                return token;
            }
            return null;
        }

        private static string GenerateToken(string username)
        {
            return $"{username}-{Guid.NewGuid()}";
        }
    }
}