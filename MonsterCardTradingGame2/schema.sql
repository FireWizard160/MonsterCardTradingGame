-- Users table
CREATE TABLE users (
                       id SERIAL PRIMARY KEY,
                       username VARCHAR(50) UNIQUE NOT NULL,
                       password VARCHAR(255) NOT NULL,
                       coins INT DEFAULT 20,
                       elo INT DEFAULT 100,
                       games_played INT DEFAULT 0,
                       wins INT DEFAULT 0,
                       losses INT DEFAULT 0,
                       session_token VARCHAR(255)
);

-- Cards table
CREATE TABLE cards (
                       id UUID PRIMARY KEY,
                       name VARCHAR(100) NOT NULL,
                       damage INT NOT NULL,
                       element_type VARCHAR(10) CHECK (element_type IN ('fire', 'water', 'normal')),
                       card_type VARCHAR(10) CHECK (card_type IN ('monster', 'spell')),
                       owner_id INT REFERENCES users(id) ON DELETE SET NULL,
                       monster_type VARCHAR(10) CHECK (monster_type IN ('Goblins','Dragons','Knights','FireElves','Wizard','Orks','Kraken'))

);

-- Decks table (cards assigned to a player's deck)
CREATE TABLE decks (
                       id SERIAL PRIMARY KEY,
                       user_id INT REFERENCES users(id) ON DELETE CASCADE,
                       card_id UUID REFERENCES cards(id) ON DELETE CASCADE,
                       UNIQUE (user_id, card_id)
);

-- Packages table (packages of 5 cards that users can buy)
CREATE TABLE packages (
                          id SERIAL PRIMARY KEY,
                          card_ids UUID[] NOT NULL
);

-- Trades table (trading deals in the store)
CREATE TABLE trades (
                        id SERIAL PRIMARY KEY,
                        offered_card_id UUID REFERENCES cards(id) ON DELETE CASCADE,
                        required_card_type VARCHAR(10) CHECK (required_card_type IN ('monster', 'spell')),
                        required_min_damage INT DEFAULT 0,
                        is_active BOOLEAN DEFAULT TRUE,
                        created_by INT REFERENCES users(id) ON DELETE CASCADE
);

    -- Battles table (logging battles between players)
    CREATE TABLE battles (
                             id SERIAL PRIMARY KEY,
                             player1_id INT REFERENCES users(id) ON DELETE CASCADE,
                             player2_id INT REFERENCES users(id) ON DELETE CASCADE,
                             winner_id INT REFERENCES users(id) ON DELETE SET NULL,
                             battle_log TEXT NOT NULL,
                             created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
    );
