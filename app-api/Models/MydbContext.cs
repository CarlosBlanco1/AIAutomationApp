using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace app_api.Models;

public partial class MydbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
{
    public MydbContext()
    {
    }

    public MydbContext(DbContextOptions<MydbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Automation> Automations { get; set; }

    public virtual DbSet<AutomationLog> AutomationLogs { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Workspace> Workspaces { get; set; }
    public virtual DbSet<Chunk> Chunks { get; set; }
    public virtual DbSet<RefreshToken> RefreshTokens {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<Automation>(entity =>
        {
            entity.HasKey(e => e.AutomationId).HasName("automations_pkey");

            entity.ToTable("automations");

            entity.Property(e => e.AutomationId)
                .HasColumnName("automation_id");
            entity.Property(e => e.ActionType)
                .HasMaxLength(255)
                .HasColumnName("action_type");
            entity.Property(e => e.AutomationName)
                .HasMaxLength(255)
                .HasColumnName("automation_name");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.TriggerType)
                .HasMaxLength(255)
                .HasColumnName("trigger_type");
            entity.Property(e => e.WebhookUrl)
                .HasMaxLength(255)
                .HasColumnName("webhook_url");
            entity.Property(e => e.WorkspaceId).HasColumnName("workspace_id");

            entity.HasOne(d => d.Workspace).WithMany(p => p.Automations)
                .HasForeignKey(d => d.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("automations_workspace_id_fkey");
        });

        modelBuilder.Entity<AutomationLog>(entity =>
        {
            entity.HasKey(e => e.AutomationLogId).HasName("automation_logs_pkey");

            entity.ToTable("automation_logs");

            entity.Property(e => e.AutomationLogId)
                .HasColumnName("automation_log_id");
            entity.Property(e => e.AutomationId).HasColumnName("automation_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.LogMessage).HasColumnName("log_message");
            entity.Property(e => e.LogStatus)
                .HasMaxLength(255)
                .HasColumnName("log_status");

            entity.HasOne(d => d.Automation).WithMany(p => p.AutomationLogs)
                .HasForeignKey(d => d.AutomationId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("automation_logs_automation_id_fkey");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("documents_pkey");

            entity.ToTable("documents");

            entity.Property(e => e.DocumentId)
                .HasColumnName("document_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FileName)
                .HasMaxLength(50)
                .HasColumnName("file_name");
            entity.Property(e => e.BlobKey)
                .HasMaxLength(500)
                .HasColumnName("blob_key");
            entity.Property(e => e.FileSizeBytes).HasColumnName("file_size_bytes");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.Summary).HasColumnName("summary");
            entity.Property(e => e.ProcessingStatus)
                .HasConversion<string>()
                .HasColumnName("processing_status");
            entity.Property(e => e.ProcessingError)
                .HasMaxLength(500)
                .HasColumnName("processing_error");
            entity.Property(e => e.WorkspaceId).HasColumnName("workspace_id");
            entity.HasOne(d => d.Workspace).WithMany(p => p.Documents)
                .HasForeignKey(d => d.WorkspaceId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("documents_workspace_id_fkey");
        });

        modelBuilder.Entity<Chunk>(entity =>
        {
            entity.HasKey(e => e.ChunkId).HasName("chunks_pkey");

            entity.ToTable("chunks");

            entity.Property(e => e.ChunkId)
                .HasColumnName("chunk_id");
            entity.Property(e => e.ChunkIndex).HasColumnName("chunk_index");
            entity.Property(e => e.ChunkText).HasColumnName("chunk_text");
            entity.Property(e => e.Embedding).HasColumnType("vector(1024)");
            entity.Property(e => e.TokenSize).HasColumnName("token_size");

            entity.HasOne(c => c.Document).WithMany(d => d.Chunks)
                .HasForeignKey(c => c.DocumentId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("chunks_document_id_fkey");

            entity.HasIndex(e => e.Embedding)
            .HasMethod("hnsw")
            .HasOperators("vector_cosine_ops");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.CreatedAt).HasColumnName("created_at");

            entity.Property(e => e.FirstName)
                .HasMaxLength(255)
                .HasColumnName("first_name");

            entity.Property(e => e.LastName)
                .HasMaxLength(255)
                .HasColumnName("last_name");
        });

        modelBuilder.Entity<Workspace>(entity =>
        {
            entity.HasKey(e => e.WorkspaceId).HasName("workspaces_pkey");

            entity.ToTable("workspaces");

            entity.Property(e => e.WorkspaceId)
                .HasColumnName("workspace_id");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.WorkspaceName)
                .HasMaxLength(255)
                .HasColumnName("workspace_name");

            entity.HasOne(d => d.Owner).WithMany(p => p.Workspaces)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("workspaces_owner_id_fkey");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.TokenId).HasName("refresh_tokens_pkey");

            entity.ToTable("refresh_tokens");

            entity.Property(e => e.UserId)
            .HasColumnName("user_id")
            .IsRequired();

            entity.Property(e => e.TokenHash)
            .HasColumnName("token_hash")
            .IsRequired();

            entity.Property(e => e.CreatedAt)
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone");

            entity.Property(e => e.ExpiresAt)
            .HasColumnName("expires_at")
            .HasColumnType("timestamp with time zone");

            entity.HasIndex(e => e.UserId)
            .IsUnique()
            .HasDatabaseName("ux_refresh_tokens_user_id");

            entity.HasOne(rt => rt.TokenUser).WithOne(u => u.RefreshToken)
            .HasForeignKey<RefreshToken>(rt => rt.UserId)
            .HasConstraintName("refresh_tokens_user_id_fkey");
        });

        modelBuilder.Entity<IdempotencyRecord>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("idempotency_records_pkey");

            entity.ToTable("idempotency_records");

            entity.Property(e => e.UserId)
            .HasColumnName("user_id")
            .IsRequired();

            entity.Property(e => e.Operation)
            .HasColumnName("operation")
            .IsRequired();

            entity.Property(e => e.ClientKey)
            .HasColumnName("client_key")
            .IsRequired();

            entity.Property(e => e.RequestBodyHash)
            .HasColumnName("request_body_hash")
            .IsRequired();

            entity.Property(e => e.ResponseStatusCode)
            .HasColumnName("response_status_code");

            entity.Property(e => e.ResponseBody)
            .HasColumnName("response_body");

            entity.Property(e => e.Status)
                .HasConversion<string>()
                .HasColumnName("status")
                .IsRequired();

            entity.Property(e => e.ExpirationDate)
            .HasColumnName("expiration_date")
            .HasColumnType("timestamp with time zone")
            .IsRequired();

            entity.HasIndex((e) => new {e.UserId, e.Operation, e.ClientKey})
            .IsUnique()
            .HasDatabaseName("ux_idempotency_records_user_id_operation_key");

            entity.HasOne(ir => ir.Originator).WithMany(u => u.IdempotencyRecords)
            .HasForeignKey(ir => ir.UserId)
            .HasConstraintName("idempotency_records_user_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
