using MonsterCardTradingGame.Cards;
using Newtonsoft.Json;
using MonsterCardTradingGame.Repositories;
using MonsterCardTradingGame.Server;

namespace MonsterCardTradingGame.Controllers
{
    public class BattleController
    {
        public static HTTPResponse HandleBattle(HTTPRequest request)
        {
            var body = JsonConvert.DeserializeObject<Dictionary<string, string>>(request.Body);
            if (body == null || !body.ContainsKey("Session-Token"))
            {
                return new HTTPResponse(400, "{\"error\": \"Session token is required\"}");
            }

            string sessionToken = body["Session-Token"];
            var userRepository = new PostgreSqlUserRepository();
            var user = userRepository.GetUserBySessionToken(sessionToken);

            if (user == null)
            {
                return new HTTPResponse(401, "{\"error\": \"Invalid session token\"}");
            }

            var battleRepository = new PostgreSqlBattleRepository();
            var userDeck = battleRepository.GetDeckByUserId(user.id);

            // Console.WriteLine(JsonConvert.SerializeObject(userDeck));

            if (userDeck.Count != 4)
            {
                return new HTTPResponse(400, "{\"error\": \"Your deck must contain exactly 4 cards.\"}");
            }

            var opponentId = battleRepository.GetRandomOpponentId(user.id);
            if (!opponentId.HasValue)
            {
                return new HTTPResponse(400, "{\"error\": \"No opponents available to battle.\"}");
            }

            var opponentDeck = battleRepository.GetDeckByUserId(opponentId.Value);
            if (opponentDeck.Count != 4)
            {
                Console.WriteLine(opponentDeck.Count);
                return new HTTPResponse(500, "{\"error\": \"Opponent's deck is invalid.\"}");
            }

            var battle = new Battle();
            return battle.StartBattle(userDeck, opponentDeck, user.id, opponentId.Value);
        }
    }
}