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

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=localhost;Database=mydb;Username=myuser;Password=mypassword");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Automation>(entity =>
        {
            entity.HasKey(e => e.AutomationId).HasName("automations_pkey");

            entity.ToTable("automations");

            entity.Property(e => e.AutomationId)
                .ValueGeneratedNever()
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
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("automations_workspace_id_fkey");
        });

        modelBuilder.Entity<AutomationLog>(entity =>
        {
            entity.HasKey(e => e.AutomationLogId).HasName("automation_logs_pkey");

            entity.ToTable("automation_logs");

            entity.Property(e => e.AutomationLogId)
                .ValueGeneratedNever()
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
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("automation_logs_automation_id_fkey");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.DocumentId).HasName("documents_pkey");

            entity.ToTable("documents");

            entity.Property(e => e.DocumentId)
                .ValueGeneratedNever()
                .HasColumnName("document_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("created_at");
            entity.Property(e => e.FileName)
                .HasMaxLength(255)
                .HasColumnName("file_name");
            entity.Property(e => e.FilePath)
                .HasMaxLength(255)
                .HasColumnName("file_path");
            entity.Property(e => e.FileText).HasColumnName("file_text");
            entity.Property(e => e.Summary).HasColumnName("summary");
            entity.Property(e => e.WorkspaceId).HasColumnName("workspace_id");

            entity.HasOne(d => d.Workspace).WithMany(p => p.Documents)
                .HasForeignKey(d => d.WorkspaceId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("documents_workspace_id_fkey");
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
                .ValueGeneratedNever()
                .HasColumnName("workspace_id");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.WorkspaceName)
                .HasMaxLength(255)
                .HasColumnName("workspace_name");

            entity.HasOne(d => d.Owner).WithMany(p => p.Workspaces)
                .HasForeignKey(d => d.OwnerId)
                .HasConstraintName("workspaces_owner_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
