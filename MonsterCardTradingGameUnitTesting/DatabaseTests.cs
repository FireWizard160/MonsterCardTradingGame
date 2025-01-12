using Moq;
using NUnit.Framework;
using MonsterCardTradingGame.Repositories;
using MonsterCardTradingGame.Cards;
using System;
using System.Data;
using MonsterCardTradingGame.src;
using Npgsql;

namespace MonsterCardTradingGame.Tests
{
    [TestFixture]
    public class DatabaseTests
    {
        private Mock<IDbConnection> _mockConnection;
        private Mock<IDbCommand> _mockCommand;
        private Mock<IDataReader> _mockReader;
        private PostgreSqlUserRepository _repository;
        private PostgreSqlCardRepository _cardRepository;
        private PostgreSqlDeckRepository _deckRepository;
        private Guid _cardId;
        private int _userId;

        [SetUp]
        public void Setup()
        {
            // Create a mock of IDbConnection
            _mockConnection = new Mock<IDbConnection>();
            _mockCommand = new Mock<IDbCommand>();
            _mockReader = new Mock<IDataReader>();

            _cardRepository = new PostgreSqlCardRepository();
            _deckRepository = new PostgreSqlDeckRepository();
            _userId = 1;
            _cardId = Guid.NewGuid();

            // Setup mock connection behavior
            _mockConnection.Setup(c => c.Open()).Verifiable();
            _mockConnection.Setup(c => c.CreateCommand()).Returns(_mockCommand.Object); // Return mocked command

            // Setup the reader to return fake card data
            _mockReader.Setup(r => r.Read()).Returns(true);
            _mockReader.Setup(r => r.GetGuid(It.IsAny<int>())).Returns(_cardId);
            _mockReader.Setup(r => r.GetInt32(It.IsAny<int>())).Returns(50);
            _mockReader.Setup(r => r.GetString(It.IsAny<int>())).Returns("fire");
            _mockReader.Setup(r => r.GetBoolean(It.IsAny<int>())).Returns(true);
            _mockReader.Setup(r => r.IsDBNull(It.IsAny<int>())).Returns(false);

            // Initialize repository with the mock connection
            _repository =
                new PostgreSqlUserRepository();

            // Mock ExecuteNonQuery to return 1 for a successful insert
            _mockCommand.Setup(c => c.ExecuteNonQuery()).Returns(1);
        }

        [Test]
        public void AddUser_ShouldReturnFalse_WhenUserNotAdded()
        {

            var username = "existingUser";
            var password = "testpassword";


            _mockCommand.Setup(c => c.ExecuteNonQuery()).Returns(0);


            var result = _repository.AddUser(username, password);


            Assert.That(result, Is.False, "AddUser should return false when the user is not added.");
        }

        [Test]
        public void ValidateUser_ShouldReturnTrue_WhenUserExists()
        {

            var username = "testuser";
            var password = "testpassword";


            _mockCommand.Setup(c => c.ExecuteScalar()).Returns(1);


            var result = _repository.ValidateUser(username, password);


            Assert.That(result, Is.True,
                "ValidateUser should return true when the user exists and the password is correct.");
        }

        [Test]
        public void ValidateUser_ShouldReturnFalse_WhenUserDoesNotExist()
        {

            var username = "invalidUser";
            var password = "wrongpassword";


            _mockCommand.Setup(c => c.ExecuteScalar()).Returns(0);


            var result = _repository.ValidateUser(username, password);

            Assert.That(result, Is.False,
                "ValidateUser should return false when the user does not exist or the password is incorrect.");
        }

        [Test]
        public void IsUsernameAvailable_ShouldReturnTrue_WhenUsernameIsAvailable()
        {

            var newUsername = "newUser";


            _mockCommand.Setup(c => c.ExecuteScalar()).Returns(0);


            var result = _repository.IsUsernameAvailable(newUsername);


            Assert.That(result, Is.True, "IsUsernameAvailable should return true when the username is available.");
        }

        [Test]
        public void GenerateRandomCard_ShouldReturnCardOfTypeMonsterOrSpell()
        {

            int userId = 1;


            var card = PostgreSqlCardRepository.GenerateRandomCard(userId);


            Assert.That(card, Is.Not.Null, "Card should not be null.");
            Assert.That(card.ownerId, Is.EqualTo(userId), "Owner ID should match.");
            Assert.That(card.cardType, Is.AnyOf("monster", "spell"),
                "Card type should be either 'monster' or 'spell'.");
        }



        [Test]
        public void GetCardById_ShouldReturnCard_WhenCardExists()
        {

            var existingCardId =
                Guid.Parse(
                    "65ac12d2-a320-4aec-870c-815d37cc46aa"); // Set the cardId to the one that exists in the database
            _mockCommand.Setup(c => c.ExecuteReader()).Returns(_mockReader.Object);


            var card = _cardRepository.GetCardById(existingCardId); // Use the existing cardId


            Assert.That(card, Is.Not.Null, "Card should not be null.");
            Assert.That(card.id, Is.EqualTo(existingCardId), "Card ID should match.");
            Assert.That(card.cardType, Is.EqualTo("spell"), "Card type should be 'monster'.");
        }


        [Test]
        public void CreateDeck_ShouldReturnNewDeckId()
        {


            int deckId = _deckRepository.CreateDeck(_userId);


            Assert.That(deckId, Is.Not.EqualTo(0),
                "Deck ID should not be null (0)."); // Ensure it's not 0 (which is the default for int)
        }





    }
}