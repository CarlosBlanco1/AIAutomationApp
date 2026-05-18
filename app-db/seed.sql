CREATE TABLE users (
    user_id integer PRIMARY KEY,
    first_name varchar(255) NOT NULL,
    last_name varchar(255) NOT NULL,
    email varchar(255) NOT NULL UNIQUE,
    password_hash varchar(255) NOT NULL,
    created_at date NOT NULL
);

CREATE TABLE workspaces (
    workspace_id integer PRIMARY KEY,
    workspace_name varchar(255) NOT NULL,
    owner_id integer REFERENCES users (user_id) ON DELETE CASCADE
);

CREATE TABLE documents (
    document_id integer PRIMARY KEY,
    workspace_id integer REFERENCES workspaces (workspace_id),
    file_name varchar(255) NOT NULL,
    file_path varchar(255) NOT NULL,
    file_text text NOT NULL,
    summary text NOT NULL,
    created_at timestamp NOT NULL
);

CREATE TABLE automations (
    automation_id integer PRIMARY KEY,
    workspace_id integer REFERENCES workspaces (workspace_id),
    automation_name varchar(255) NOT NULL,
    trigger_type varchar(255) NOT NULL,
    action_type varchar(255) NOT NULL,
    webhook_url varchar(255) NOT NULL,
    is_active boolean NOT NULL
);

CREATE TABLE automation_logs (
    automation_log_id integer PRIMARY KEY,
    automation_id integer REFERENCES automations (automation_id),
    log_status varchar(255) NOT NULL,
    log_message text NOT NULL,
    created_at timestamp NOT NULL
);

INSERT INTO users VALUES (1, 'carlos','blanco','carlos@gmail.com','123abc','2026-07-02');

