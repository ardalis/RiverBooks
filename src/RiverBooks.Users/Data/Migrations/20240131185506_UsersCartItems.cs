using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RiverBooks.Users.Data.Migrations;

  /// <inheritdoc />
  public partial class UsersCartItems : Migration
  {
      /// <inheritdoc />
      protected override void Up(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.EnsureSchema(
              name: "Users");

          migrationBuilder.CreateTable(
              name: "AspNetRoles",
              schema: "Users",
              columns: table => new
              {
                  Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                  Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                  NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                  ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_AspNetRoles", x => x.Id);
              });

          migrationBuilder.CreateTable(
              name: "AspNetUsers",
              schema: "Users",
              columns: table => new
              {
                  Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                  Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                  FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                  UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                  NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                  Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                  NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                  EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                  PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                  SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                  ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                  PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                  PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                  TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                  LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                  LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                  AccessFailedCount = table.Column<int>(type: "int", nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_AspNetUsers", x => x.Id);
              });

          migrationBuilder.CreateTable(
              name: "AspNetRoleClaims",
              schema: "Users",
              columns: table => new
              {
                  Id = table.Column<int>(type: "int", nullable: false)
                      .Annotation("SqlServer:Identity", "1, 1"),
                  RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                  ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                  ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                  table.ForeignKey(
                      name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                      column: x => x.RoleId,
                      principalSchema: "Users",
                      principalTable: "AspNetRoles",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

          migrationBuilder.CreateTable(
              name: "AspNetUserClaims",
              schema: "Users",
              columns: table => new
              {
                  Id = table.Column<int>(type: "int", nullable: false)
                      .Annotation("SqlServer:Identity", "1, 1"),
                  UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                  ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                  ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                  table.ForeignKey(
                      name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                      column: x => x.UserId,
                      principalSchema: "Users",
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

          migrationBuilder.CreateTable(
              name: "AspNetUserLogins",
              schema: "Users",
              columns: table => new
              {
                  LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                  ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                  ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                  UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                  table.ForeignKey(
                      name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                      column: x => x.UserId,
                      principalSchema: "Users",
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

          migrationBuilder.CreateTable(
              name: "AspNetUserRoles",
              schema: "Users",
              columns: table => new
              {
                  UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                  RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                  table.ForeignKey(
                      name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                      column: x => x.RoleId,
                      principalSchema: "Users",
                      principalTable: "AspNetRoles",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
                  table.ForeignKey(
                      name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                      column: x => x.UserId,
                      principalSchema: "Users",
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

          migrationBuilder.CreateTable(
              name: "AspNetUserTokens",
              schema: "Users",
              columns: table => new
              {
                  UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                  LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                  Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                  Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                  table.ForeignKey(
                      name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                      column: x => x.UserId,
                      principalSchema: "Users",
                      principalTable: "AspNetUsers",
                      principalColumn: "Id",
                      onDelete: ReferentialAction.Cascade);
              });

          migrationBuilder.CreateTable(
              name: "CartItem",
              schema: "Users",
              columns: table => new
              {
                  Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  BookId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                  Quantity = table.Column<int>(type: "int", nullable: false),
                  UnitPrice = table.Column<decimal>(type: "decimal(18,6)", precision: 18, scale: 6, nullable: false),
                  ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
              },
              constraints: table =>
              {
                  table.PrimaryKey("PK_CartItem", x => x.Id);
                  table.ForeignKey(
                      name: "FK_CartItem_AspNetUsers_ApplicationUserId",
                      column: x => x.ApplicationUserId,
                      principalSchema: "Users",
                      principalTable: "AspNetUsers",
                      principalColumn: "Id");
              });

          migrationBuilder.CreateIndex(
              name: "IX_AspNetRoleClaims_RoleId",
              schema: "Users",
              table: "AspNetRoleClaims",
              column: "RoleId");

          migrationBuilder.CreateIndex(
              name: "RoleNameIndex",
              schema: "Users",
              table: "AspNetRoles",
              column: "NormalizedName",
              unique: true,
              filter: "[NormalizedName] IS NOT NULL");

          migrationBuilder.CreateIndex(
              name: "IX_AspNetUserClaims_UserId",
              schema: "Users",
              table: "AspNetUserClaims",
              column: "UserId");

          migrationBuilder.CreateIndex(
              name: "IX_AspNetUserLogins_UserId",
              schema: "Users",
              table: "AspNetUserLogins",
              column: "UserId");

          migrationBuilder.CreateIndex(
              name: "IX_AspNetUserRoles_RoleId",
              schema: "Users",
              table: "AspNetUserRoles",
              column: "RoleId");

          migrationBuilder.CreateIndex(
              name: "EmailIndex",
              schema: "Users",
              table: "AspNetUsers",
              column: "NormalizedEmail");

          migrationBuilder.CreateIndex(
              name: "UserNameIndex",
              schema: "Users",
              table: "AspNetUsers",
              column: "NormalizedUserName",
              unique: true,
              filter: "[NormalizedUserName] IS NOT NULL");

          migrationBuilder.CreateIndex(
              name: "IX_CartItem_ApplicationUserId",
              schema: "Users",
              table: "CartItem",
              column: "ApplicationUserId");
      }

      /// <inheritdoc />
      protected override void Down(MigrationBuilder migrationBuilder)
      {
          migrationBuilder.DropTable(
              name: "AspNetRoleClaims",
              schema: "Users");

          migrationBuilder.DropTable(
              name: "AspNetUserClaims",
              schema: "Users");

          migrationBuilder.DropTable(
              name: "AspNetUserLogins",
              schema: "Users");

          migrationBuilder.DropTable(
              name: "AspNetUserRoles",
              schema: "Users");

          migrationBuilder.DropTable(
              name: "AspNetUserTokens",
              schema: "Users");

          migrationBuilder.DropTable(
              name: "CartItem",
              schema: "Users");

          migrationBuilder.DropTable(
              name: "AspNetRoles",
              schema: "Users");

          migrationBuilder.DropTable(
              name: "AspNetUsers",
              schema: "Users");
      }
  }
