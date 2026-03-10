using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Randevu365.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUserInformations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Surname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUserInformations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: false),
                    TableName = table.Column<string>(type: "text", nullable: false),
                    DateTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    OldValues = table.Column<string>(type: "text", nullable: true),
                    NewValues = table.Column<string>(type: "text", nullable: true),
                    AffectedColumns = table.Column<string>(type: "text", nullable: true),
                    PrimaryKey = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "text", nullable: false),
                    ConversationId = table.Column<string>(type: "text", nullable: false),
                    SenderId = table.Column<int>(type: "integer", nullable: false),
                    ReceiverId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Password = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    AppUserInformationId = table.Column<int>(type: "integer", nullable: true),
                    RefreshToken = table.Column<string>(type: "text", nullable: true),
                    RefreshTokenExpiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppUsers_AppUserInformations_AppUserInformationId",
                        column: x => x.AppUserInformationId,
                        principalTable: "AppUserInformations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Businesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BusinessName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BusinessAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    BusinessCity = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BusinessPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    BusinessEmail = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    BusinessCountry = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BusinessCategory = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AppUserId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Businesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Businesses_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BusinessId = table.Column<int>(type: "integer", nullable: false),
                    AppUserId = table.Column<int>(type: "integer", nullable: false),
                    Comment = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessComments_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessComments_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessHours",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Day = table.Column<string>(type: "text", nullable: false),
                    OpenTime = table.Column<string>(type: "text", nullable: false),
                    CloseTime = table.Column<string>(type: "text", nullable: false),
                    BusinessId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessHours", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessHours_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessLocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BusinessId = table.Column<int>(type: "integer", nullable: false),
                    Latitude = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    Longitude = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessLocations_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessLogos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LogoUrl = table.Column<string>(type: "text", nullable: false),
                    BusinessId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessLogos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessLogos_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessPhotos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BusinessId = table.Column<int>(type: "integer", nullable: false),
                    PhotoPath = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessPhotos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessPhotos_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessRatings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BusinessId = table.Column<int>(type: "integer", nullable: false),
                    AppUserId = table.Column<int>(type: "integer", nullable: false),
                    Rating = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessRatings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessRatings_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessRatings_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ServiceTitle = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ServiceContent = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    MaxConcurrentCustomers = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    ServicePrice = table.Column<decimal>(type: "numeric", nullable: false),
                    BusinessId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessServices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessServices_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BusinessSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppUserId = table.Column<int>(type: "integer", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "numeric(10,2)", nullable: false),
                    PaymentStatus = table.Column<int>(type: "integer", nullable: false),
                    PaymentMethod = table.Column<int>(type: "integer", nullable: false),
                    ExternalTransactionId = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    PaidAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false),
                    UsedForBusinessId = table.Column<int>(type: "integer", nullable: true),
                    UsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Notes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    PackageId = table.Column<Guid>(type: "uuid", nullable: true),
                    PackageType = table.Column<int>(type: "integer", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BusinessSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BusinessSlots_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BusinessSlots_Businesses_UsedForBusinessId",
                        column: x => x.UsedForBusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppUserId = table.Column<int>(type: "integer", nullable: false),
                    BusinessId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favorites_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AppUserId = table.Column<int>(type: "integer", nullable: false),
                    BusinessId = table.Column<int>(type: "integer", nullable: false),
                    BusinessServiceId = table.Column<int>(type: "integer", nullable: false),
                    AppointmentDate = table.Column<DateOnly>(type: "date", nullable: false),
                    RequestedStartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    RequestedEndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    ApproveStartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    ApproveEndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CustomerNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    BusinessNotes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_AppUsers_AppUserId",
                        column: x => x.AppUserId,
                        principalTable: "AppUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_BusinessServices_BusinessServiceId",
                        column: x => x.BusinessServiceId,
                        principalTable: "BusinessServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Appointments_Businesses_BusinessId",
                        column: x => x.BusinessId,
                        principalTable: "Businesses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Conversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    OtherUserId = table.Column<int>(type: "integer", nullable: false),
                    ConversationId = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AppointmentId = table.Column<int>(type: "integer", nullable: true),
                    IsClosed = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Conversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Conversations_Appointments_AppointmentId",
                        column: x => x.AppointmentId,
                        principalTable: "Appointments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "AppUserInformations",
                columns: new[] { "Id", "CreatedAt", "Name", "PhoneNumber", "Surname", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Admin", "05001234567", "Randevu365", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ahmet", "05321234567", "Yılmaz", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Elif", "05331234567", "Kaya", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Mehmet", "05341234567", "Demir", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ayşe", "05351234567", "Çelik", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Ali", "05361234567", "Öztürk", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Zeynep", "05371234567", "Arslan", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Burak", "05381234567", "Doğan", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Seda", "05391234567", "Kılıç", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Emre", "05401234567", "Aslan", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Merve", "05411234567", "Çetin", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "AppUsers",
                columns: new[] { "Id", "AppUserInformationId", "CreatedAt", "Email", "Password", "RefreshToken", "RefreshTokenExpiry", "Role", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "admin@randevu365.com", "$2a$11$iTswnw/5VyzC43Wy13w9vOTPQ.rwl6BZjcx3pH5XUdsGhzEwmVPNS", null, null, "Administrator", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "owner1@randevu365.com", "$2a$11$Rqi6dTA8Q42o/eCnzg9prOPAPj2tRB3SX5O5HneHtE2w85nv4VKP.", null, null, "BusinessOwner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 3, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "owner2@randevu365.com", "$2a$11$Rqi6dTA8Q42o/eCnzg9prOPAPj2tRB3SX5O5HneHtE2w85nv4VKP.", null, null, "BusinessOwner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 4, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "owner3@randevu365.com", "$2a$11$Rqi6dTA8Q42o/eCnzg9prOPAPj2tRB3SX5O5HneHtE2w85nv4VKP.", null, null, "BusinessOwner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 5, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "owner4@randevu365.com", "$2a$11$Rqi6dTA8Q42o/eCnzg9prOPAPj2tRB3SX5O5HneHtE2w85nv4VKP.", null, null, "BusinessOwner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, 6, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "owner5@randevu365.com", "$2a$11$Rqi6dTA8Q42o/eCnzg9prOPAPj2tRB3SX5O5HneHtE2w85nv4VKP.", null, null, "BusinessOwner", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 7, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer1@randevu365.com", "$2a$11$IGW3Vm5qpmfE4a.Yf2qUXuoeZL72Yl7SSljF1zpU00THU4a8h97kO", null, null, "Customer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 8, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer2@randevu365.com", "$2a$11$IGW3Vm5qpmfE4a.Yf2qUXuoeZL72Yl7SSljF1zpU00THU4a8h97kO", null, null, "Customer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 9, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer3@randevu365.com", "$2a$11$IGW3Vm5qpmfE4a.Yf2qUXuoeZL72Yl7SSljF1zpU00THU4a8h97kO", null, null, "Customer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 10, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer4@randevu365.com", "$2a$11$IGW3Vm5qpmfE4a.Yf2qUXuoeZL72Yl7SSljF1zpU00THU4a8h97kO", null, null, "Customer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 11, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "customer5@randevu365.com", "$2a$11$IGW3Vm5qpmfE4a.Yf2qUXuoeZL72Yl7SSljF1zpU00THU4a8h97kO", null, null, "Customer", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "Businesses",
                columns: new[] { "Id", "AppUserId", "BusinessAddress", "BusinessCategory", "BusinessCity", "BusinessCountry", "BusinessEmail", "BusinessName", "BusinessPhone", "CreatedAt", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 2, "Dummy Adres No:1", "kuafor", "İstanbul", "Türkiye", "owner1@randevu365.com", "Business Owner 1 - İşletme 1", "02120000001", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 2, "Dummy Adres No:2", "kuafor", "İstanbul", "Türkiye", "owner1@randevu365.com", "Business Owner 1 - İşletme 2", "02120000002", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 2, "Dummy Adres No:3", "kuafor", "İstanbul", "Türkiye", "owner1@randevu365.com", "Business Owner 1 - İşletme 3", "02120000003", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 2, "Dummy Adres No:4", "kuafor", "İstanbul", "Türkiye", "owner1@randevu365.com", "Business Owner 1 - İşletme 4", "02120000004", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 2, "Dummy Adres No:5", "kuafor", "İstanbul", "Türkiye", "owner1@randevu365.com", "Business Owner 1 - İşletme 5", "02120000005", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, 3, "Dummy Adres No:6", "guzellik", "İstanbul", "Türkiye", "owner2@randevu365.com", "Business Owner 2 - İşletme 1", "02120000006", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 3, "Dummy Adres No:7", "guzellik", "İstanbul", "Türkiye", "owner2@randevu365.com", "Business Owner 2 - İşletme 2", "02120000007", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 3, "Dummy Adres No:8", "guzellik", "İstanbul", "Türkiye", "owner2@randevu365.com", "Business Owner 2 - İşletme 3", "02120000008", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 3, "Dummy Adres No:9", "guzellik", "İstanbul", "Türkiye", "owner2@randevu365.com", "Business Owner 2 - İşletme 4", "02120000009", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 3, "Dummy Adres No:10", "guzellik", "İstanbul", "Türkiye", "owner2@randevu365.com", "Business Owner 2 - İşletme 5", "02120000010", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 4, "Dummy Adres No:11", "saglik", "İstanbul", "Türkiye", "owner3@randevu365.com", "Business Owner 3 - İşletme 1", "02120000011", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, 4, "Dummy Adres No:12", "saglik", "İstanbul", "Türkiye", "owner3@randevu365.com", "Business Owner 3 - İşletme 2", "02120000012", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, 4, "Dummy Adres No:13", "saglik", "İstanbul", "Türkiye", "owner3@randevu365.com", "Business Owner 3 - İşletme 3", "02120000013", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, 4, "Dummy Adres No:14", "saglik", "İstanbul", "Türkiye", "owner3@randevu365.com", "Business Owner 3 - İşletme 4", "02120000014", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, 4, "Dummy Adres No:15", "saglik", "İstanbul", "Türkiye", "owner3@randevu365.com", "Business Owner 3 - İşletme 5", "02120000015", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, 5, "Dummy Adres No:16", "fitness", "İstanbul", "Türkiye", "owner4@randevu365.com", "Business Owner 4 - İşletme 1", "02120000016", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, 5, "Dummy Adres No:17", "fitness", "İstanbul", "Türkiye", "owner4@randevu365.com", "Business Owner 4 - İşletme 2", "02120000017", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, 5, "Dummy Adres No:18", "fitness", "İstanbul", "Türkiye", "owner4@randevu365.com", "Business Owner 4 - İşletme 3", "02120000018", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, 5, "Dummy Adres No:19", "fitness", "İstanbul", "Türkiye", "owner4@randevu365.com", "Business Owner 4 - İşletme 4", "02120000019", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, 5, "Dummy Adres No:20", "fitness", "İstanbul", "Türkiye", "owner4@randevu365.com", "Business Owner 4 - İşletme 5", "02120000020", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, 6, "Dummy Adres No:21", "dis", "İstanbul", "Türkiye", "owner5@randevu365.com", "Business Owner 5 - İşletme 1", "02120000021", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, 6, "Dummy Adres No:22", "dis", "İstanbul", "Türkiye", "owner5@randevu365.com", "Business Owner 5 - İşletme 2", "02120000022", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, 6, "Dummy Adres No:23", "dis", "İstanbul", "Türkiye", "owner5@randevu365.com", "Business Owner 5 - İşletme 3", "02120000023", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, 6, "Dummy Adres No:24", "dis", "İstanbul", "Türkiye", "owner5@randevu365.com", "Business Owner 5 - İşletme 4", "02120000024", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, 6, "Dummy Adres No:25", "dis", "İstanbul", "Türkiye", "owner5@randevu365.com", "Business Owner 5 - İşletme 5", "02120000025", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.InsertData(
                table: "BusinessComments",
                columns: new[] { "Id", "AppUserId", "BusinessId", "Comment", "CreatedAt", "IsDeleted", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 7, 1, "Çok memnun kaldım, teşekkürler!", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 2, 8, 2, "Hizmet kalitesi güzeldi.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 3, 9, 2, "Kesinlikle tavsiye ederim.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 4, 9, 3, "Kesinlikle tavsiye ederim.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 5, 10, 4, "Gayet profesyonel bir işletme.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 6, 11, 4, "Tekrar geleceğim, memnun kaldım.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 7, 11, 5, "Tekrar geleceğim, memnun kaldım.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 8, 7, 6, "Çok memnun kaldım, teşekkürler!", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 9, 8, 6, "Hizmet kalitesi güzeldi.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 10, 8, 7, "Hizmet kalitesi güzeldi.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 11, 9, 8, "Kesinlikle tavsiye ederim.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 12, 10, 8, "Gayet profesyonel bir işletme.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 13, 10, 9, "Gayet profesyonel bir işletme.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 14, 11, 10, "Tekrar geleceğim, memnun kaldım.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 15, 7, 10, "Çok memnun kaldım, teşekkürler!", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 16, 7, 11, "Çok memnun kaldım, teşekkürler!", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 17, 8, 12, "Hizmet kalitesi güzeldi.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 18, 9, 12, "Kesinlikle tavsiye ederim.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 19, 9, 13, "Kesinlikle tavsiye ederim.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 20, 10, 14, "Gayet profesyonel bir işletme.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 21, 11, 14, "Tekrar geleceğim, memnun kaldım.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 22, 11, 15, "Tekrar geleceğim, memnun kaldım.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 23, 7, 16, "Çok memnun kaldım, teşekkürler!", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 24, 8, 16, "Hizmet kalitesi güzeldi.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 25, 8, 17, "Hizmet kalitesi güzeldi.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 26, 9, 18, "Kesinlikle tavsiye ederim.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 27, 10, 18, "Gayet profesyonel bir işletme.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 28, 10, 19, "Gayet profesyonel bir işletme.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 29, 11, 20, "Tekrar geleceğim, memnun kaldım.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 30, 7, 20, "Çok memnun kaldım, teşekkürler!", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 31, 7, 21, "Çok memnun kaldım, teşekkürler!", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 32, 8, 22, "Hizmet kalitesi güzeldi.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 33, 9, 22, "Kesinlikle tavsiye ederim.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 34, 9, 23, "Kesinlikle tavsiye ederim.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 35, 10, 24, "Gayet profesyonel bir işletme.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 36, 11, 24, "Tekrar geleceğim, memnun kaldım.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { 37, 11, 25, "Tekrar geleceğim, memnun kaldım.", new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), false, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_AppUserId",
                table: "Appointments",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_BusinessId_AppointmentDate_RequestedStartTime",
                table: "Appointments",
                columns: new[] { "BusinessId", "AppointmentDate", "RequestedStartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_BusinessServiceId",
                table: "Appointments",
                column: "BusinessServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_AppUserInformationId",
                table: "AppUsers",
                column: "AppUserInformationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_Email",
                table: "AppUsers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessComments_AppUserId",
                table: "BusinessComments",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessComments_BusinessId",
                table: "BusinessComments",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_Businesses_AppUserId",
                table: "Businesses",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessHours_BusinessId",
                table: "BusinessHours",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLocations_BusinessId",
                table: "BusinessLocations",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessLogos_BusinessId",
                table: "BusinessLogos",
                column: "BusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusinessPhotos_BusinessId",
                table: "BusinessPhotos",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRatings_AppUserId",
                table: "BusinessRatings",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessRatings_BusinessId",
                table: "BusinessRatings",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessServices_BusinessId",
                table: "BusinessServices",
                column: "BusinessId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessSlots_AppUserId_IsUsed_PaymentStatus",
                table: "BusinessSlots",
                columns: new[] { "AppUserId", "IsUsed", "PaymentStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_BusinessSlots_PackageId",
                table: "BusinessSlots",
                column: "PackageId");

            migrationBuilder.CreateIndex(
                name: "IX_BusinessSlots_UsedForBusinessId",
                table: "BusinessSlots",
                column: "UsedForBusinessId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_AppointmentId",
                table: "Conversations",
                column: "AppointmentId",
                unique: true,
                filter: "\"AppointmentId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Conversations_ConversationId",
                table: "Conversations",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_AppUserId_BusinessId",
                table: "Favorites",
                columns: new[] { "AppUserId", "BusinessId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_BusinessId",
                table: "Favorites",
                column: "BusinessId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "BusinessComments");

            migrationBuilder.DropTable(
                name: "BusinessHours");

            migrationBuilder.DropTable(
                name: "BusinessLocations");

            migrationBuilder.DropTable(
                name: "BusinessLogos");

            migrationBuilder.DropTable(
                name: "BusinessPhotos");

            migrationBuilder.DropTable(
                name: "BusinessRatings");

            migrationBuilder.DropTable(
                name: "BusinessSlots");

            migrationBuilder.DropTable(
                name: "Conversations");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "BusinessServices");

            migrationBuilder.DropTable(
                name: "Businesses");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropTable(
                name: "AppUserInformations");
        }
    }
}
