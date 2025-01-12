using NUnit.Framework;
using MonsterCardTradingGame.Controllers;
using MonsterCardTradingGame.Server;
using Newtonsoft.Json;

namespace MonsterCardTradingGame.Tests
{
    [TestFixture]
    public class ControllerTests
    {
        [Test]
        public void HandleBattle_ShouldReturn400_WhenSessionTokenIsMissing()
        {
            // Arrange: Simulate a missing Session-Token in the request body
            var request = new HTTPRequest { Body = "{}" };  // Empty JSON body, no Session-Token

            // Act: Call the HandleBattle method, which should return an error response
            var response = BattleController.HandleBattle(request);

            // Assert: Check if the error message contains "Session token is required"
            Assert.That(response.Body.Trim(), Does.Contain("Session token is required"));
        }

        [Test]
        public void HandleBattle_ShouldReturn401_WhenSessionTokenIsInvalid()
        {
            // Arrange: Provide a request with an invalid session token
            var request = new HTTPRequest { Body = "{\"Session-Token\": \"invalid-token\"}" };

            // Act: Call the HandleBattle method, which should return an error response
            var response = BattleController.HandleBattle(request);

            // Assert: Check if the error message contains "Invalid session token"
            Assert.That(response.Body.Trim(), Does.Contain("Invalid session token"));
        }

    }
}
