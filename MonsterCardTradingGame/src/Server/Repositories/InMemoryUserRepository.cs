using System.Collections.Generic;

namespace MonsterCardTradingGame.Repositories
{
    public class InMemoryUserRepository
    {
        private static Dictionary<string, string> users = new Dictionary<string, string>();
        private static Dictionary<string, string> sessions = new Dictionary<string, string>();

        public static bool AddUser(string username, string password)
        {
            if (users.ContainsKey(username))
                return false;

            users.Add(username, password);
            return true;
        }

        public static bool ValidateUser(string username, string password)
        {
            return users.ContainsKey(username) && users[username] == password;
        }

        public static void AddSession(string username, string token)
        {
            sessions[username] = token;
        }

        public static string GetSessionToken(string username)
        {
            return sessions.ContainsKey(username) ? sessions[username] : null;
        }
    }
}