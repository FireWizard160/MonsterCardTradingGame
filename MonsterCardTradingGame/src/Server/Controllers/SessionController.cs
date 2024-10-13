using MonsterCardTradingGame.Server;
using MonsterCardTradingGame.Services;
using Newtonsoft.Json;

namespace MonsterCardTradingGame.Controllers
{
    public class SessionController
    {
        public static HTTPResponse HandleLogin(HTTPRequest request)
        {
            var loginData = JsonConvert.DeserializeObject<LoginData>(request.Body);

            // Authenticate user
            var token = SessionService.Login(loginData.username, loginData.password);


            if (token != null)
            {
                return new HTTPResponse(200, JsonConvert.SerializeObject(new { token }));
            }
            else
            {
                return new HTTPResponse(401, "{\"error\": \"Invalid credentials\"}");
            }
        }
    }

    public class LoginData
    {
        public string username { get; set; }
        public string password { get; set; }

    }
}