using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Maturaball_Tischreservierung.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: false),
                    Surname = table.Column<string>(type: "longtext", nullable: false),
                    Email = table.Column<string>(type: "longtext", nullable: false),
                    Class = table.Column<string>(type: "longtext", nullable: false),
                    EmailVerified = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Version = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ServerSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    IsRegistrationOpen = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsReservationOpen = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsPageOpen = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    IsInEarlyAccess = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    GreetMessage = table.Column<string>(type: "longtext", nullable: false),
                    Version = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServerSettings", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TableReservations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Version = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TableReservations", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClientAccessCodes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    AccessCode = table.Column<string>(type: "longtext", nullable: false),
                    ClientId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Version = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientAccessCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientAccessCodes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClientSessions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ClientId = table.Column<Guid>(type: "char(36)", nullable: false),
                    Version = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientSessions_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ClientAccessCodes_ClientId",
                table: "ClientAccessCodes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientSessions_ClientId",
                table: "ClientSessions",
                column: "ClientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientAccessCodes");

            migrationBuilder.DropTable(
                name: "ClientSessions");

            migrationBuilder.DropTable(
                name: "ServerSettings");

            migrationBuilder.DropTable(
                name: "TableReservations");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
