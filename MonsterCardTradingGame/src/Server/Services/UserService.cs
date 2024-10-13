using System;
using MonsterCardTradingGame.Repositories;

namespace MonsterCardTradingGame.Services
{
    public class UserService
    {
        private static UserData _userData = new UserData();
        public static bool Authenticate(string username, string password)
        {
            return _userData.ValidateUser(username, password);
            //return InMemoryUserRepository.ValidateUser(username, password);
        }

        public static string GenerateToken(string username)
        {
            // Generate and store a session token
            string token = Guid.NewGuid().ToString();
            InMemoryUserRepository.AddSession(username, token);
            return token;
        }

        public static bool RegisterUser(string username, string password)
        {
            return _userData.AddUser(username, password);
            // return InMemoryUserRepository.AddUser(username, password);
        }

        public static bool ValidateUser(string username, string password)
        {
            return _userData.ValidateUser(username, password);
            //return InMemoryUserRepository.ValidateUser(username, password);
        }
    }
}