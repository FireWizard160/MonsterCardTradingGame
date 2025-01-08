using MonsterCardTradingGame.Repositories;

namespace MonsterCardTradingGame.Services
{
    public static class UserService
    {
        private static readonly PostgreSqlUserRepository _userRepo = new();

        // Register a new user
        public static bool RegisterUser(string username, string password)
        {
            return _userRepo.AddUser(username, password);
        }
    }
}