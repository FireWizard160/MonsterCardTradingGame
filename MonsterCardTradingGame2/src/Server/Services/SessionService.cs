using MonsterCardTradingGame.Repositories;

public static class SessionService
{
    private static readonly PostgreSqlUserRepository _userRepo = new();

    // Login and generate a session token
    public static string Login(string username, string password)
    {
        if (!_userRepo.ValidateUser(username, password))
            return null;

        string token = Guid.NewGuid().ToString();
        _userRepo.AddSession(username, token);
        return token;
    }

    // Get an existing session token
    public static string GetSession(string username)
    {
        return _userRepo.GetSessionToken(username);
    }
}