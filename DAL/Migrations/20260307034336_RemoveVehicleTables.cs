using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVehicleTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleRentalBookings");

            migrationBuilder.DropTable(
                name: "Vehicles");

            migrationBuilder.DropTable(
                name: "VehicleCategory");

            migrationBuilder.DropTable(
                name: "VehicleRentalServices");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleRentalBookings",
                columns: table => new
                {
                    VehicleRentalBookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BookingStatus = table.Column<int>(type: "int", nullable: false),
                    DepositPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DriverIdentification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DriverRequired = table.Column<bool>(type: "bit", nullable: false),
                    PickupLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RentalEndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RentalHours = table.Column<int>(type: "int", nullable: false),
                    RentalStartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnLocation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleRentalBookings", x => x.VehicleRentalBookingId);
                    table.ForeignKey(
                        name: "FK_VehicleRentalBookings_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "BookingId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VehicleRentalServices",
                columns: table => new
                {
                    RentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BusinessLicense = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CleaningFee = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DeliveryAvailable = table.Column<bool>(type: "bit", nullable: false),
                    EstimatedFuelConsumption = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    FuelPolicy = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    FuelTankCapacity = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    InsurancePolicy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LateReturnFeePerHour = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    MaxRentalDays = table.Column<int>(type: "int", nullable: false),
                    MinRentalHours = table.Column<int>(type: "int", nullable: false),
                    OperatingHours = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PickupLocations = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RentalType = table.Column<int>(type: "int", nullable: false),
                    SmokingPenalty = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleRentalServices", x => x.RentalId);
                    table.ForeignKey(
                        name: "FK_VehicleRentalServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "ServiceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VehicleCategory",
                columns: table => new
                {
                    CategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RentalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleCategory", x => x.CategoryId);
                    table.ForeignKey(
                        name: "FK_VehicleCategory_VehicleRentalServices_RentalId",
                        column: x => x.RentalId,
                        principalTable: "VehicleRentalServices",
                        principalColumn: "RentalId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Vehicles",
                columns: table => new
                {
                    VehicleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Features = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FuelType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Images = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicensePlate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LittersPer100Km = table.Column<int>(type: "int", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Seats = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransmissionType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VehicleCategoryCategoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    VehicleType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vehicles", x => x.VehicleId);
                    table.ForeignKey(
                        name: "FK_Vehicles_VehicleCategory_VehicleCategoryCategoryId",
                        column: x => x.VehicleCategoryCategoryId,
                        principalTable: "VehicleCategory",
                        principalColumn: "CategoryId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleCategory_RentalId",
                table: "VehicleCategory",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRentalBookings_BookingId",
                table: "VehicleRentalBookings",
                column: "BookingId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleRentalServices_ServiceId",
                table: "VehicleRentalServices",
                column: "ServiceId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicles_VehicleCategoryCategoryId",
                table: "Vehicles",
                column: "VehicleCategoryCategoryId");
        }
    }
}
