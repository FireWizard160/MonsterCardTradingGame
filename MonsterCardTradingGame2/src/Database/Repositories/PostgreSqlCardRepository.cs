using MonsterCardTradingGame.Cards;
using MonsterCardTradingGame.src;
using Npgsql;
using Random = System.Random;
using Npgsql;


namespace MonsterCardTradingGame.Repositories
{
    public class PostgreSqlCardRepository
    {
        private readonly string _connectionString = "Host=localhost;Username=postgres;Password=1234;Database=mtcg";

        // Add a card to the database
        //
        //
        public static Card GenerateRandomCard(int userId)


        {
            // Randomly determine if the card is a Monster or Spell
            var random = new Random();
            var isMonsterCard = random.Next(2) == 0;

            // Randomly choose element type: Fire, Water, or Normal using the enum
            var elementTypes = Enum.GetValues(typeof(Element));
            Element elementType = (Element)elementTypes.GetValue(random.Next(elementTypes.Length));

            // Create the card
            Card card;

            if (isMonsterCard)
            {
                // If it's a MonsterCard, randomly choose the monster type (subclass)
                var monsterTypes = new[] { "Goblins", "Dragons", "Knights", "FireElves", "Wizard", "Orks", "Kraken" };
                string monsterType = monsterTypes[random.Next(7)];

                // Create the correct MonsterCard subclass based on the chosen monsterType
                switch (monsterType)
                {
                    case "Dragons":
                        card = new Dragon
                        {
                            damage = 80,
                            _element = elementType,
                            cardType = "monster",
                            ownerId = userId,
                            MonsterType = "Dragons"
                        };
                        break;

                    case "Goblins":
                        card = new Goblin
                        {
                            damage = 30,
                            _element = elementType,
                            cardType = "monster",
                            ownerId = userId,
                            MonsterType = "Goblins"
                        };
                        break;

                    case "Knights":
                        card = new Knight
                        {
                            damage = 50,
                            _element = elementType,
                            cardType = "monster",
                            ownerId = userId,
                            MonsterType = "Knights"
                        };
                        break;

                    case "FireElves":
                        card = new FireElf
                        {
                            damage = 40,
                            _element = elementType,
                            cardType = "monster",
                            ownerId = userId,
                            MonsterType = "FireElves"
                        };
                        break;

                    case "Wizard":
                        card = new Wizard
                        {
                            damage = 80,
                            _element = elementType,
                            cardType = "monster",
                            ownerId = userId,
                            MonsterType = "Wizard"
                        };
                        break;

                    case "Orks":
                        card = new Ork
                        {
                            damage = 50,
                            _element = elementType,
                            cardType = "monster",
                            ownerId = userId,
                            MonsterType = "Orks"
                        };
                        break;

                    case "Kraken":
                        card = new Kraken
                        {
                            damage = 60,
                            _element = elementType,
                            cardType = "monster",
                            ownerId = userId,
                            MonsterType = "Kraken"
                        };
                        break;

                    default:
                        throw new InvalidOperationException("Unknown monster type");
                }
            }
            else
            {
                // If it's a SpellCard, no monsterType is needed
                card = new SpellCard
                {
                    damage = 50,
                    _element = elementType,
                    cardType = "spell",
                    ownerId = userId
                };
            }

            return card;
        }


        public void AddCard(Card card)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand(
                           "INSERT INTO cards (id, damage, element_type, card_type, owner_id, monster_type) VALUES (gen_random_uuid(), @damage, @element_type, @card_type, @owner_id, @monster_type)",
                           conn))
                {
                    cmd.Parameters.AddWithValue("damage", card.damage);

                    // Convert the Element enum to its string representation
                    cmd.Parameters.AddWithValue("element_type",
                        card._element.ToString().ToLower()); // Ensure it is lowercase (fire, water, normal)
                    cmd.Parameters.AddWithValue("card_type", card.cardType);
                    cmd.Parameters.AddWithValue("owner_id", card.ownerId);

                    // Check if the card is a MonsterCard and set monster_type accordingly
                    if (card is MonsterCard monsterCard)
                    {
                        // If it's a MonsterCard, set the monster type
                        cmd.Parameters.AddWithValue("monster_type", monsterCard.MonsterType);
                    }
                    else
                    {
                        // If it's not a MonsterCard, set monster_type to DBNull.Value
                        cmd.Parameters.AddWithValue("monster_type", DBNull.Value);
                    }

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public Card GetCardById(Guid cardId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("SELECT * FROM cards WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", cardId);

                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            string cardType = reader.GetString(reader.GetOrdinal("card_type"));

                            if (cardType == "monster")
                            {
                                return new MonsterCard
                                {
                                    id = reader.GetGuid(reader.GetOrdinal("id")), // Handle UUID
                                    damage = reader.GetInt32(reader.GetOrdinal("damage")),
                                    _element = Enum.Parse<Element>(reader.GetString(reader.GetOrdinal("element_type")),
                                        true),
                                    cardType = cardType,
                                    ownerId = reader.IsDBNull(reader.GetOrdinal("owner_id"))
                                        ? null
                                        : reader.GetInt32(reader.GetOrdinal("owner_id")),
                                    isInDeck = reader.GetBoolean(reader.GetOrdinal("is_in_deck")),
                                    MonsterType = reader.IsDBNull(reader.GetOrdinal("monster_type"))
                                        ? null
                                        : reader.GetString(reader.GetOrdinal("monster_type"))
                                };
                            }
                            else if (cardType == "spell")
                            {
                                return new SpellCard
                                {
                                    id = reader.GetGuid(reader.GetOrdinal("id")), // Handle UUID
                                    damage = reader.GetInt32(reader.GetOrdinal("damage")),
                                    _element = Enum.Parse<Element>(reader.GetString(reader.GetOrdinal("element_type")),
                                        true),
                                    cardType = cardType,
                                    ownerId = reader.IsDBNull(reader.GetOrdinal("owner_id"))
                                        ? null
                                        : reader.GetInt32(reader.GetOrdinal("owner_id")),
                                    isInDeck = reader.GetBoolean(reader.GetOrdinal("is_in_deck"))
                                };
                            }
                            else
                            {
                                throw new InvalidOperationException("Unknown card type in the database.");
                            }
                        }
                    }
                }
            }

// If no rows were returned, return null or throw an exception
            return null;
        }


        public void MarkCardAsInDeck(Guid cardId)
        {
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new NpgsqlCommand("UPDATE cards SET is_in_deck = TRUE WHERE id = @id", conn))
                {
                    cmd.Parameters.AddWithValue("id", cardId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}