CREATE TABLE machine_status_audits (
    id TEXT PRIMARY KEY NOT NULL,
    machine_id TEXT NOT NULL,
    old_status TEXT NOT NULL,
    new_status TEXT NOT NULL,
    occurred_on TEXT NOT NULL
);