using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Project.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Address = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ClientType = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    KrsNumber = table.Column<string>(type: "nchar(10)", fixedLength: true, maxLength: 10, nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Pesel = table.Column<string>(type: "nchar(11)", fixedLength: true, maxLength: 11, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Discounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiscountType = table.Column<int>(type: "int", nullable: false),
                    Percentage = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discounts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Login = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SoftwareSystems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UpfrontCost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    SubscriptionCost = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SoftwareSystems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Versions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    SoftwareSystemId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Versions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Versions_SoftwareSystems_SoftwareSystemId",
                        column: x => x.SoftwareSystemId,
                        principalTable: "SoftwareSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Contracts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    SoftwareSystemId = table.Column<int>(type: "int", nullable: false),
                    SoftwareVersionId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    SupportYears = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contracts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Contracts_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contracts_SoftwareSystems_SoftwareSystemId",
                        column: x => x.SoftwareSystemId,
                        principalTable: "SoftwareSystems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Contracts_Versions_SoftwareVersionId",
                        column: x => x.SoftwareVersionId,
                        principalTable: "Versions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContractId = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Contracts_ContractId",
                        column: x => x.ContractId,
                        principalTable: "Contracts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Address", "ClientType", "Email", "FirstName", "IsDeleted", "LastName", "Pesel", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "ul. Marszałkowska 1, 00-001 Warszawa", "Individual", "jan.kowalski@email.com", "Jan", false, "Kowalski", "85010112345", "+48123456789" },
                    { 2, "ul. Nowy Świat 15, 00-001 Warszawa", "Individual", "anna.nowak@email.com", "Anna", false, "Nowak", "90020287654", "+48987654321" },
                    { 3, "ul. Krakowskie Przedmieście 3, 31-013 Kraków", "Individual", "piotr.wisniewski@email.com", "Piotr", false, "Wiśniewski", "75121565432", "+48555123456" },
                    { 4, "ul. Piotrkowska 100, 90-001 Łódź", "Individual", "maria.wojcik@email.com", "Maria", false, "Wójcik", "88030445678", "+48666789012" },
                    { 5, "ul. Świdnicka 5, 50-001 Wrocław", "Individual", "tomasz.kaczmarek@email.com", "Tomasz", false, "Kaczmarek", "92111098765", "+48777234567" }
                });

            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "Address", "ClientType", "CompanyName", "Email", "KrsNumber", "PhoneNumber" },
                values: new object[,]
                {
                    { 6, "al. Jerozolimskie 123, 02-001 Warszawa", "Company", "TechSoft Sp. z o.o.", "kontakt@techsoft.pl", "0000123456", "+48222345678" },
                    { 7, "ul. Floriańska 12, 31-019 Kraków", "Company", "Global Solutions S.A.", "biuro@globalsolutions.pl", "0000234567", "+48123456789" },
                    { 8, "ul. Narutowicza 88, 80-001 Gdańsk", "Company", "Innowacja Polska Sp. z o.o.", "info@innowacja.pl", "0000345678", "+48581234567" },
                    { 9, "ul. Andersa 7, 61-001 Poznań", "Company", "Digital Partners S.A.", "contact@digitalpartners.pl", "0000456789", "+48616789012" },
                    { 10, "ul. Piłsudskiego 25, 90-001 Łódź", "Company", "Future Tech Sp. z o.o.", "hello@futuretech.pl", "0000567890", "+48423456789" }
                });

            migrationBuilder.InsertData(
                table: "Discounts",
                columns: new[] { "Id", "DiscountType", "EndDate", "Name", "Percentage", "StartDate" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2025, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Black Friday Discount", 10, new DateTime(2025, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, 0, new DateTime(2025, 7, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "New Year Discount", 15, new DateTime(2025, 6, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, 0, new DateTime(2025, 5, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "Black Friday Discount", 25, new DateTime(2025, 4, 13, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "SoftwareSystems",
                columns: new[] { "Id", "Category", "Description", "Name", "SubscriptionCost", "UpfrontCost" },
                values: new object[,]
                {
                    { 1, "Finance", "Comprehensive accounting software for small to medium businesses", "AccountingPro", 29.99m, 299.99m },
                    { 2, "Education", "Complete student management system for educational institutions", "EduManager", 49.99m, null },
                    { 3, "Finance", "Simple invoicing solution for freelancers and consultants", "InvoiceExpress", 15.99m, 159.99m },
                    { 4, "Education", "E-learning platform with course management and analytics", "LearningHub", null, 1999.99m },
                    { 5, "Retail", "Advanced inventory management system for retail businesses", "InventoryMaster", 39.99m, null }
                });

            migrationBuilder.InsertData(
                table: "Versions",
                columns: new[] { "Id", "IsCurrent", "Name", "SoftwareSystemId" },
                values: new object[,]
                {
                    { 1, false, "1.0.0", 1 },
                    { 2, false, "2.0.0", 1 },
                    { 3, true, "3.0.0", 1 },
                    { 4, false, "1.0.0", 2 },
                    { 5, false, "1.5.0", 2 },
                    { 6, true, "2.0.0", 2 },
                    { 7, false, "1.0.0", 3 },
                    { 8, false, "1.1.0", 3 },
                    { 9, true, "2.0.0", 3 },
                    { 10, false, "1.0.0", 4 },
                    { 11, false, "2.0.0", 4 },
                    { 12, true, "2.1.0", 4 },
                    { 13, false, "1.0.0", 5 },
                    { 14, false, "1.2.0", 5 },
                    { 15, true, "2.0.0", 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Email",
                table: "Clients",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_KrsNumber",
                table: "Clients",
                column: "KrsNumber",
                unique: true,
                filter: "[KrsNumber] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_Pesel",
                table: "Clients",
                column: "Pesel",
                unique: true,
                filter: "[Pesel] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_ClientId",
                table: "Contracts",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_SoftwareSystemId",
                table: "Contracts",
                column: "SoftwareSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Contracts_SoftwareVersionId",
                table: "Contracts",
                column: "SoftwareVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_Login",
                table: "Employees",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ContractId",
                table: "Payments",
                column: "ContractId");

            migrationBuilder.CreateIndex(
                name: "IX_Versions_SoftwareSystemId",
                table: "Versions",
                column: "SoftwareSystemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Discounts");

            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Contracts");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Versions");

            migrationBuilder.DropTable(
                name: "SoftwareSystems");
        }
    }
}
