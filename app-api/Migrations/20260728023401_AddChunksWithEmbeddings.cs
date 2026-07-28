using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace app_api.Migrations
{
    /// <inheritdoc />
    public partial class AddChunksWithEmbeddings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:vector", ",,");

            migrationBuilder.CreateTable(
                name: "chunks",
                columns: table => new
                {
                    chunk_id = table.Column<Guid>(type: "uuid", nullable: false),
                    DocumentId = table.Column<Guid>(type: "uuid", nullable: false),
                    chunk_index = table.Column<int>(type: "integer", nullable: false),
                    chunk_text = table.Column<string>(type: "text", nullable: false),
                    Embedding = table.Column<Vector>(type: "vector(1024)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("chunks_pkey", x => x.chunk_id);
                    table.ForeignKey(
                        name: "chunks_document_id_fkey",
                        column: x => x.DocumentId,
                        principalTable: "documents",
                        principalColumn: "document_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_chunks_DocumentId",
                table: "chunks",
                column: "DocumentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "chunks");

            migrationBuilder.AlterDatabase()
                .OldAnnotation("Npgsql:PostgresExtension:vector", ",,");
        }
    }
}
