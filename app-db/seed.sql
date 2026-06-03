CREATE TABLE workspaces (
    workspace_id UUID PRIMARY KEY,
    workspace_name varchar(255) NOT NULL,
    owner_id UUID NOT NULL REFERENCES users (user_id) ON DELETE CASCADE
);

CREATE TABLE documents (
    document_id UUID PRIMARY KEY,
    workspace_id UUID NOT NULL REFERENCES workspaces (workspace_id),
    file_name varchar(255) NOT NULL,
    file_path varchar(255) NOT NULL,
    file_text text NOT NULL,
    summary text NOT NULL,
    created_at timestamp NOT NULL
);

CREATE TABLE automations (
    automation_id UUID PRIMARY KEY,
    workspace_id UUID NOT NULL REFERENCES workspaces (workspace_id),
    automation_name varchar(255) NOT NULL,
    trigger_type varchar(255) NOT NULL,
    action_type varchar(255) NOT NULL,
    webhook_url varchar(255) NOT NULL,
    is_active boolean NOT NULL
);

CREATE TABLE automation_logs (
    automation_log_id UUID PRIMARY KEY,
    automation_id UUID NOT NULL REFERENCES automations (automation_id),
    log_status varchar(255) NOT NULL,
    log_message text NOT NULL,
    created_at timestamp NOT NULL
);
