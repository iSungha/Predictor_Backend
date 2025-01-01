CREATE TABLE IF NOT EXISTS product_prices (
    id SERIAL PRIMARY KEY,
    product_name VARCHAR(255) NOT NULL,
    month DATE NOT NULL,
    price NUMERIC(10, 2) NOT NULL
);
