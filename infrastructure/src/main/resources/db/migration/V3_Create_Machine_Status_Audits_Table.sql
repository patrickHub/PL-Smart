CREATE TABLE machine_status_audits (
    id UUID PRIMARY KEY,
    machine_id UUID NOT NULL,
    old_status VARCHAR(50) NOT NULL,
    new_status VARCHAR(50) NOT NULL,
    occurred_on TIMESTAMP WITH TIME ZONE NOT NULL
);