using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TrainTicketApi.Migrations
{
    /// <inheritdoc />
    public partial class InitTrainTicketManagement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdentityNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Routes",
                columns: table => new
                {
                    RouteId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartureStation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ArrivalStation = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DistanceKm = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Routes", x => x.RouteId);
                    table.CheckConstraint("CK_Routes_DistanceKm", "[DistanceKm] > 0");
                });

            migrationBuilder.CreateTable(
                name: "TrainTrips",
                columns: table => new
                {
                    TrainTripId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrainCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    RouteId = table.Column<int>(type: "int", nullable: false),
                    DepartureTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ArrivalTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalSeats = table.Column<int>(type: "int", nullable: false),
                    AvailableSeats = table.Column<int>(type: "int", nullable: false),
                    BaseTicketPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrainTrips", x => x.TrainTripId);
                    table.CheckConstraint("CK_TrainTrips_ArrivalTime", "[ArrivalTime] > [DepartureTime]");
                    table.CheckConstraint("CK_TrainTrips_AvailableSeats", "[AvailableSeats] >= 0 AND [AvailableSeats] <= [TotalSeats]");
                    table.CheckConstraint("CK_TrainTrips_BaseTicketPrice", "[BaseTicketPrice] > 0");
                    table.CheckConstraint("CK_TrainTrips_TotalSeats", "[TotalSeats] > 0");
                    table.ForeignKey(
                        name: "FK_TrainTrips_Routes_RouteId",
                        column: x => x.RouteId,
                        principalTable: "Routes",
                        principalColumn: "RouteId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tickets",
                columns: table => new
                {
                    TicketId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TicketCode = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TrainTripId = table.Column<int>(type: "int", nullable: false),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    SeatNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    BookingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    TicketStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tickets", x => x.TicketId);
                    table.CheckConstraint("CK_Tickets_Price", "[Price] > 0");
                    table.ForeignKey(
                        name: "FK_Tickets_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tickets_TrainTrips_TrainTripId",
                        column: x => x.TrainTripId,
                        principalTable: "TrainTrips",
                        principalColumn: "TrainTripId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "CustomerId", "Address", "Email", "FullName", "IdentityNumber", "PhoneNumber" },
                values: new object[] { 1, "Ha Noi", "nguyenvana@example.com", "Nguyen Van A", "012345678901", "0912345678" });

            migrationBuilder.InsertData(
                table: "Routes",
                columns: new[] { "RouteId", "ArrivalStation", "DepartureStation", "DistanceKm", "Status" },
                values: new object[,]
                {
                    { 1, "Hai Phong", "Ha Noi", 102m, "Active" },
                    { 2, "Da Nang", "Ha Noi", 764m, "Active" }
                });

            migrationBuilder.InsertData(
                table: "TrainTrips",
                columns: new[] { "TrainTripId", "ArrivalTime", "AvailableSeats", "BaseTicketPrice", "DepartureTime", "RouteId", "Status", "TotalSeats", "TrainCode" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 6, 1, 10, 30, 0, 0, DateTimeKind.Unspecified), 119, 180000m, new DateTime(2026, 6, 1, 8, 0, 0, 0, DateTimeKind.Unspecified), 1, "Scheduled", 120, "SE01" },
                    { 2, new DateTime(2026, 6, 2, 20, 0, 0, 0, DateTimeKind.Unspecified), 200, 850000m, new DateTime(2026, 6, 2, 7, 30, 0, 0, DateTimeKind.Unspecified), 2, "Scheduled", 200, "SE03" }
                });

            migrationBuilder.InsertData(
                table: "Tickets",
                columns: new[] { "TicketId", "BookingDate", "CustomerId", "PaymentStatus", "Price", "SeatNumber", "TicketCode", "TicketStatus", "TrainTripId" },
                values: new object[] { 1, new DateTime(2026, 5, 1, 10, 0, 0, 0, DateTimeKind.Unspecified), 1, "Paid", 180000m, "A01", "TICKET-20260501-0001", "Booked", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_IdentityNumber",
                table: "Customers",
                column: "IdentityNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_PhoneNumber",
                table: "Customers",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CustomerId",
                table: "Tickets",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TicketCode",
                table: "Tickets",
                column: "TicketCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_TrainTripId_SeatNumber",
                table: "Tickets",
                columns: new[] { "TrainTripId", "SeatNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrainTrips_RouteId",
                table: "TrainTrips",
                column: "RouteId");

            migrationBuilder.CreateIndex(
                name: "IX_TrainTrips_TrainCode",
                table: "TrainTrips",
                column: "TrainCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tickets");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "TrainTrips");

            migrationBuilder.DropTable(
                name: "Routes");
        }
    }
}
