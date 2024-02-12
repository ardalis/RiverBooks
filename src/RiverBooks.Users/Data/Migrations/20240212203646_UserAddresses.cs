using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverBooks.Users.Data.Migrations;

/// <inheritdoc />
public partial class UserAddresses : Migration
{
  /// <inheritdoc />
  protected override void Up(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.CreateTable(
        name: "UserStreetAddress",
        schema: "Users",
        columns: table => new
        {
          Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
          UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
          ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
          StreetAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
          StreetAddress_Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
          StreetAddress_PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
          StreetAddress_State = table.Column<string>(type: "nvarchar(max)", nullable: false),
          StreetAddress_Street1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
          StreetAddress_Street2 = table.Column<string>(type: "nvarchar(max)", nullable: false)
        },
        constraints: table =>
        {
          table.PrimaryKey("PK_UserStreetAddress", x => x.Id);
          table.ForeignKey(
                    name: "FK_UserStreetAddress_AspNetUsers_ApplicationUserId",
                    column: x => x.ApplicationUserId,
                    principalSchema: "Users",
                    principalTable: "AspNetUsers",
                    principalColumn: "Id");
        });

    migrationBuilder.CreateIndex(
        name: "IX_UserStreetAddress_ApplicationUserId",
        schema: "Users",
        table: "UserStreetAddress",
        column: "ApplicationUserId");
  }

  /// <inheritdoc />
  protected override void Down(MigrationBuilder migrationBuilder)
  {
    migrationBuilder.DropTable(
        name: "UserStreetAddress",
        schema: "Users");
  }
}
