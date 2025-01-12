using System;
using System.Collections.Generic;
using MonsterCardTradingGame.Cards;
using MonsterCardTradingGame.Repositories;
using MonsterCardTradingGame.Server; // For JSON parsing
using MonsterCardTradingGame.Services;
using MonsterCardTradingGame.src;
using Newtonsoft.Json;
using JsonException = System.Text.Json.JsonException;
using JsonSerializer = System.Text.Json.JsonSerializer;


namespace MonsterCardTradingGame.Controllers
{
    public class UserController
    {
        public static HTTPResponse HandleRegistration(HTTPRequest request)
        {
            try
            {
                // Parse the body JSON string into a dictionary
                var body = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Body);

                if (body == null || !body.ContainsKey("username") || !body.ContainsKey("password"))
                {
                    return new HTTPResponse(400, "{\"error\": \"Username and password are required\"}");
                }

                string username = body["username"];
                string password = body["password"];

                if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                {
                    return new HTTPResponse(400, "{\"error\": \"Username and password cannot be empty\"}");
                }

                bool isRegistered = UserService.RegisterUser(username, password);

                if (isRegistered)
                {
                    return new HTTPResponse(201, "{\"message\": \"User registered successfully\"}");
                }
                else
                {
                    return new HTTPResponse(409, "{\"error\": \"User already exists\"}");
                }
            }
            catch (JsonException)
            {
                return new HTTPResponse(400, "{\"error\": \"Invalid JSON format\"}");
            }
        }

        public static HTTPResponse HandleViewProfile(HTTPRequest request)
        {
            // Parse the body for session token
            var body = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Body);
            if (body == null || !body.ContainsKey("Session-Token"))
            {
                return new HTTPResponse(400, "{\"error\": \"Session token is required\"}");
            }

            string sessionToken = body["Session-Token"];

            // Initialize repository and get the user by session token
            var userRepository = new PostgreSqlUserRepository();
            var user = userRepository.GetUserBySessionToken(sessionToken);

            if (user == null)
            {
                return new HTTPResponse(401, "{\"error\": \"Invalid session token\"}");
            }

            // Return the user's profile data
            var userProfile = new
            {
                Username = user.username,
                Coins = user.coins,
                Elo = user.elo,
                GamesPlayed = user.gamesPlayed,
                Wins = user.wins,
                Losses = user.losses
            };

            return new HTTPResponse(200, JsonConvert.SerializeObject(userProfile));
        }


        public static HTTPResponse HandleEditUsername(HTTPRequest request)
        {
            // Parse the body for session token and new username
            var body = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Body);
            if (body == null || !body.ContainsKey("Session-Token") || !body.ContainsKey("newUsername"))
            {
                return new HTTPResponse(400, "{\"error\": \"Session token and new username are required\"}");
            }

            string sessionToken = body["Session-Token"];
            string newUsername = body["newUsername"];

            // Initialize repository and get the user by session token
            var userRepository = new PostgreSqlUserRepository();
            var user = userRepository.GetUserBySessionToken(sessionToken);

            if (user == null)
            {
                return new HTTPResponse(401, "{\"error\": \"Invalid session token\"}");
            }

            // Check if the new username is available (i.e., not already taken)
            bool isUsernameAvailable = userRepository.IsUsernameAvailable(newUsername);

            if (!isUsernameAvailable)
            {
                return new HTTPResponse(409, "{\"error\": \"Username already taken\"}");
            }

            // Update the username in the database
            userRepository.UpdateUsername(sessionToken, newUsername);

            // Return success response
            return new HTTPResponse(200, "{\"message\": \"Username updated successfully\"}");
        }


        public static HTTPResponse HandleScoreboard(HTTPRequest request)
        {
            // Initialize repository and get the list of all users, sorted by elo
            var userRepository = new PostgreSqlUserRepository();
            var users = userRepository.GetUsersSortedByElo();

            // Check if users exist
            if (users == null || users.Count == 0)
            {
                return new HTTPResponse(404, "{\"error\": \"No users found\"}");
            }

            // Prepare the scoreboard data
            var scoreboard = users.Select(user => new
            {
                Username = user.username,
                Elo = user.elo,
                Wins = user.wins,
                Losses = user.losses
            }).ToList();

            // Return the scoreboard data
            return new HTTPResponse(200, JsonConvert.SerializeObject(scoreboard));
        }

        public static HTTPResponse HandleAquirePackage(HTTPRequest request)
        {
            // Parse the body for session token
            var body = JsonSerializer.Deserialize<Dictionary<string, string>>(request.Body);
            if (body == null || !body.ContainsKey("Session-Token"))
            {
                return new HTTPResponse(400, "{\"error\": \"Session token is required\"}");
            }

            string sessionToken = body["Session-Token"];

            // Initialize repository and get the user by session token
            var userRepository = new PostgreSqlUserRepository();
            var user = userRepository.GetUserBySessionToken(sessionToken);

            if (user == null)
            {
                return new HTTPResponse(401, "{\"error\": \"Invalid session token\"}");
            }

            // Check if the user has enough coins (at least 5)
            if (user.coins < 5)
            {
                return new HTTPResponse(400, "{\"error\": \"Not enough coins\"}");
            }

            // Deduct the coins for the package
            userRepository.DeductCoins(user.id, 5);

            // List to store acquired cards
            var acquiredCards = new List<Card>();

            // Generate 5 cards (both Monster and Spell types with random element and monster type)
            var cardRepository = new PostgreSqlCardRepository();
            for (int i = 0; i < 5; i++)
            {
                var card = PostgreSqlCardRepository.GenerateRandomCard(user.id);
                cardRepository.AddCard(card);
                acquiredCards.Add(card);
            }

            // Prepare the response message with the acquired cards
            var acquiredCardDetails = acquiredCards.Select(card => new
            {
                card.id,
                card.cardType,
                card._element,
                // Only include MonsterType for MonsterCards
                monsterType = card is MonsterCard monsterCard ? monsterCard.MonsterType : null,
                card.damage
            }).ToList();

            // Return a success message with the acquired cards list
            var response = new
            {
                message = "Package acquired successfully! 5 cards added to your collection.",
                cards = acquiredCardDetails
            };

            return new HTTPResponse(200, JsonConvert.SerializeObject(response));
        }

        public static HTTPResponse HandleCreateDeck(HTTPRequest request)
        {
            try
            {
                // Parse the request body for session token and card IDs
                var body = JsonSerializer.Deserialize<Dictionary<string, object>>(request.Body);

                if (body == null || !body.ContainsKey("Session-Token") || !body.ContainsKey("cardIds"))
                {
                    return new HTTPResponse(400, "{\"error\": \"Session token and card IDs are required\"}");
                }

                string sessionToken = body["Session-Token"].ToString();
                var cardIds = JsonConvert.DeserializeObject<List<Guid>>(body["cardIds"].ToString());

                // Validate card IDs
                if (cardIds == null || cardIds.Count != 4)
                {
                    return new HTTPResponse(400, "{\"error\": \"Exactly 4 card IDs must be provided\"}");
                }

                // Get the user by session token
                var userRepository = new PostgreSqlUserRepository();
                var user = userRepository.GetUserBySessionToken(sessionToken);

                if (user == null)
                {
                    return new HTTPResponse(401, "{\"error\": \"Invalid session token\"}");
                }

                int userId = user.id;

                // Check that the cards belong to the user and are not already in a deck
                var cardRepository = new PostgreSqlCardRepository();
                foreach (var cardId in cardIds)
                {
                    var card = cardRepository.GetCardById(cardId);
                    if (card == null || card.ownerId != userId || card.isInDeck)
                    {
                        return new HTTPResponse(400,
                            "{\"error\": \"All cards must belong to the user and not already be in a deck\"}");
                    }
                }

                // Create a new deck
                var deckRepository = new PostgreSqlDeckRepository();
                int deckId = deckRepository.CreateDeck(userId);

                // Add the cards to the deck
                foreach (var cardId in cardIds)
                {
                    deckRepository.AddCardToDeck(deckId, cardId);

                    // Mark the card as being in a deck
                    cardRepository.MarkCardAsInDeck(cardId);
                }

                return new HTTPResponse(201, "{\"message\": \"Deck created successfully\"}");
            }
            catch (JsonException)
            {
                return new HTTPResponse(400, "{\"error\": \"Invalid JSON format\"}");
            }
            catch (Exception ex)
            {
                return new HTTPResponse(500, $"{{\"error\": \"An error occurred: {ex.Message}\"}}");
            }
        }
    }
}