using NUnit.Framework;
using MonsterCardTradingGame.Server;
using System;

namespace MonsterCardTradingGame.Tests
{
    [TestFixture]
    public class HTTPServerTests
    {
        [Test]
        public void TestRequestParsing_ValidRequest()
        {
            string rawRequest = "POST /login HTTP/1.1\r\nHost: localhost\r\nContent-Type: application/json\r\n\r\n{\"username\": \"user\", \"password\": \"pass\"}";

            HTTPRequest request = HTTPRequest.Parse(rawRequest);

            Assert.That(request.Method, Is.EqualTo("POST"));
            Assert.That(request.Path, Is.EqualTo("/login"));
            Assert.That(request.Body, Is.EqualTo("{\"username\": \"user\", \"password\": \"pass\"}"));
        }

        [Test]
        public void TestRequestParsing_InvalidRequest()
        {
            string rawRequest = "GET /nonexistentpath HTTP/1.1\r\nHost: localhost\r\n\r\n";

            HTTPRequest request = HTTPRequest.Parse(rawRequest);

            Assert.That(request.Method, Is.EqualTo("GET"));
            Assert.That(request.Path, Is.EqualTo("/nonexistentpath"));
            Assert.That(request.Body, Is.Empty);
        }

        [Test]
        public void TestServerHandlesLoginRoute()
        {
            string rawRequest = "POST /login HTTP/1.1\r\nHost: localhost\r\nContent-Type: application/json\r\n\r\n{\"username\": \"test5\", \"password\": \"test\"}";

            HTTPServer server = new HTTPServer(System.Net.IPAddress.Loopback, 8080);

            HTTPRequest request = HTTPRequest.Parse(rawRequest);
            HTTPResponse response = server.RouteRequest(request);

            Assert.That(response.StatusCode, Is.EqualTo(200));

            // Verify the response contains a token
            Assert.That(response.Body, Does.Contain("\"token\":"));

            // Optionally, you can parse the response body to verify it is valid JSON with a token field
            var responseBody = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(response.Body);
            Assert.That((string)responseBody.token, Is.Not.Null.And.Not.Empty);
        }


        [Test]
        public void TestServerHandlesInvalidRoute()
        {
            string rawRequest = "GET /nonexistentroute HTTP/1.1\r\nHost: localhost\r\n\r\n";

            HTTPServer server = new HTTPServer(System.Net.IPAddress.Loopback, 8080);

            HTTPRequest request = HTTPRequest.Parse(rawRequest);
            HTTPResponse response = server.RouteRequest(request);

            Assert.That(response.StatusCode, Is.EqualTo(404));
            Assert.That(response.Body, Does.Contain("Not Found"));
        }



        [Test]
        public void TestInvalidMethodHandling()
        {
            string rawRequest = "PUT /nonexistentroute HTTP/1.1\r\nHost: localhost\r\n\r\n";

            HTTPServer server = new HTTPServer(System.Net.IPAddress.Loopback, 8080);

            HTTPRequest request = HTTPRequest.Parse(rawRequest);
            HTTPResponse response = server.RouteRequest(request);

            Assert.That(response.StatusCode, Is.EqualTo(404));
            Assert.That(response.Body, Does.Contain("Not Found"));
        }

        [Test]
        public void TestServerHandlesScoreboardRoute()
        {
            string rawRequest = "GET /scoreboard HTTP/1.1\r\nHost: localhost\r\n";

            HTTPServer server = new HTTPServer(System.Net.IPAddress.Loopback, 8080);

            HTTPRequest request = HTTPRequest.Parse(rawRequest);
            HTTPResponse response = server.RouteRequest(request);

            Assert.That(response.StatusCode, Is.EqualTo(200));

            // Validate response is a JSON array
            var scoreboard = Newtonsoft.Json.JsonConvert.DeserializeObject<List<dynamic>>(response.Body);
            Assert.That(scoreboard, Is.Not.Null.And.Not.Empty);

            // Validate each entry in the scoreboard
            foreach (var entry in scoreboard)
            {
                Assert.That((int)entry.Wins, Is.GreaterThanOrEqualTo(0), "Wins should be non-negative.");
                Assert.That((int)entry.Losses, Is.GreaterThanOrEqualTo(0), "Losses should be non-negative.");
            }
        }

    }
}
