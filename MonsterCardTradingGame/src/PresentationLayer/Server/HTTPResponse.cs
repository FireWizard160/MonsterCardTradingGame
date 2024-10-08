namespace MonsterCardTradingGame.Server
{
    public class HTTPResponse
    {
        public int StatusCode { get; set; }
        public string Body { get; set; }

        public HTTPResponse(int statusCode, string body)
        {
            StatusCode = statusCode;
            Body = body;
        }
    }
}