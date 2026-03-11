CREATE TABLE machines (
    id UUID PRIMARY KEY,
    name VARCHAR(255) NOT NULL,
    price_per_cycle NUMERIC(10,2) NOT NULL,
    status VARCHAR(50) NOT NULL
);