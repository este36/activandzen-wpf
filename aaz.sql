PRAGMA foreign_keys = ON;

-- Création de la table clients
CREATE TABLE clients (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    is_active INTEGER NOT NULL DEFAULT 1,
    name TEXT NOT NULL
);

-- Création de la table employees
CREATE TABLE employees (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    is_active INTEGER NOT NULL DEFAULT 1,
    client_id INTEGER NOT NULL,
    first_name TEXT NOT NULL,
    last_name TEXT NOT NULL,
    email TEXT,
    phone TEXT,
    special_notes TEXT,
    FOREIGN KEY (client_id) REFERENCES clients(id) ON DELETE CASCADE
);

-- Création de la table registered_payments avec une clé étrangère vers la table clients
CREATE TABLE payed_slots (
    client_id INTEGER NOT NULL,
    employe_id INTEGER NOT NULL,
    payment_date DATE NOT NULL,
    slot_quantity INTEGER NOT NULL,
    FOREIGN KEY (client_id) REFERENCES clients(id) ON DELETE CASCADE,
    FOREIGN KEY (employe_id) REFERENCES employees(id) ON DELETE CASCADE
);

-- Création de la table past_slots avec une clé étrangère vers la table clients
CREATE TABLE past_slots (
    client_id INTEGER NOT NULL,
    employe_id INTEGER NOT NULL,
    slot_date INTEGER NOT NULL,
    FOREIGN KEY (client_id) REFERENCES clients(id) ON DELETE CASCADE,
    FOREIGN KEY (employe_id) REFERENCES employees(id) ON DELETE CASCADE
);

-- Insertion des données dans la table clients
INSERT INTO clients (name) VALUES 
    ('Hitachi'),
    ('Rolex'),
	('Macdo');

-- Insertion des données dans la table employees
INSERT INTO employees (client_id, first_name, last_name, email, phone, special_notes) VALUES  
    (1, 'Jean', 'Carnacier', 'jean.carnacier@example.com', '+41 78 123 45 67', 'Au top'),
    (1, 'Remi', 'Herbivore', 'remi.herbivore@example.com', '+41 79 987 65 43', 'Tu me passes le lead ?'),
    (1, 'Anaïs', 'Petit', 'anais.petit@example.com', '+41 79 543 21 09', 'Mauvais temps pour les grenouilles'),
    (2, 'Paul', 'Mieux', 'paul.mieux@example.com', '+41 71 234 56 78', NULL),
    (2, 'Benoit', 'Pavraiment', 'benoit.pavraiment@example.com', '+41 78 345 67 89', 'Aimes le lundi.'),
    (2, 'Eude', 'Doume', 'eude.doume@example.com', '+41 79 654 32 10', 'Partage d''écran obligatoire !');
