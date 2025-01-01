CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE IF NOT EXISTS Prices (
    id uuid default uuid_generate_v4() not null PRIMARY KEY,
    Year INT NOT NULL,
    Month INT NOT NULL,
    ItemName TEXT NOT NULL,
    Price NUMERIC NOT NULL,
    Model_Used TEXT NOT NULL

);
