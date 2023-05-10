using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VkInternship.Data.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "content");

            migrationBuilder.CreateTable(
                name: "user_groups",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_groups", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user_states",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    code = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_states", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "content",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    login = table.Column<string>(type: "text", nullable: false),
                    password = table.Column<string>(type: "text", nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    group_id = table.Column<int>(type: "integer", nullable: false),
                    state_id = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_users", x => x.id);
                    table.ForeignKey(
                        name: "fk_users_user_groups_group_id",
                        column: x => x.group_id,
                        principalSchema: "content",
                        principalTable: "user_groups",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_users_user_states_state_id",
                        column: x => x.state_id,
                        principalSchema: "content",
                        principalTable: "user_states",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "content",
                table: "user_groups",
                columns: new[] { "id", "code", "description" },
                values: new object[,]
                {
                    { 1, "Admin", "" },
                    { 2, "User", "" }
                });

            migrationBuilder.InsertData(
                schema: "content",
                table: "user_states",
                columns: new[] { "id", "code", "description" },
                values: new object[,]
                {
                    { 1, "Active", "" },
                    { 2, "Blocked", "" }
                });

            migrationBuilder.CreateIndex(
                name: "ix_user_groups_code",
                schema: "content",
                table: "user_groups",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_states_code",
                schema: "content",
                table: "user_states",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_users_group_id",
                schema: "content",
                table: "users",
                column: "group_id");

            migrationBuilder.CreateIndex(
                name: "ix_users_id_state_id",
                schema: "content",
                table: "users",
                columns: new[] { "id", "state_id" });

            migrationBuilder.CreateIndex(
                name: "ix_users_state_id",
                schema: "content",
                table: "users",
                column: "state_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "users",
                schema: "content");

            migrationBuilder.DropTable(
                name: "user_groups",
                schema: "content");

            migrationBuilder.DropTable(
                name: "user_states",
                schema: "content");
        }
    }
}
