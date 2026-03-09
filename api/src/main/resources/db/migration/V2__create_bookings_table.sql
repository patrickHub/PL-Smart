CREATE TABLE bookings (
    id TEXT PRIMARY KEY NOT NULL,
    machine_id TEXT NOT NULL,
    start_time TEXT NOT NULL,
    end_time TEXT NOT NULL,
    customer_name TEXT NOT NULL,
    status TEXT NOT NULL
);