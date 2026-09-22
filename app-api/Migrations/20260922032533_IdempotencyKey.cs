using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace app_api.Migrations
{
    /// <inheritdoc />
    public partial class IdempotencyKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "idempotency_records",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    operation = table.Column<string>(type: "text", nullable: false),
                    client_key = table.Column<string>(type: "text", nullable: false),
                    request_body_hash = table.Column<string>(type: "text", nullable: false),
                    response_status_code = table.Column<int>(type: "integer", nullable: true),
                    response_body = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<string>(type: "text", nullable: false),
                    expiration_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("idempotency_records_pkey", x => x.Id);
                    table.ForeignKey(
                        name: "idempotency_records_user_id_fkey",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ux_idempotency_records_user_id_operation_key",
                table: "idempotency_records",
                columns: new[] { "user_id", "operation", "client_key" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "idempotency_records");
        }
    }
}
