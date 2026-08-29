using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Estudaki.Modules.Questions.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialAreas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Questions");

            migrationBuilder.CreateTable(
                name: "Areas",
                schema: "Questions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    Type = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Areas", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Areas_Type_Name",
                schema: "Questions",
                table: "Areas",
                columns: new[] { "Type", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Areas",
                schema: "Questions");
        }
    }
}
