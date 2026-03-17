using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class CompleteServiceComboVoucherFlows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "PlatformFeeAmount",
                table: "Services",
                type: "decimal(15,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "AdditionalServices",
                table: "Combos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Combos",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ComboId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DiscountAmount",
                table: "Bookings",
                type: "decimal(15,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "FinalAmount",
                table: "Bookings",
                type: "decimal(15,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalAmount",
                table: "Bookings",
                type: "decimal(15,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "VoucherCode",
                table: "Bookings",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VoucherId",
                table: "Bookings",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vouchers_VoucherCode",
                table: "Vouchers",
                column: "VoucherCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_ComboId",
                table: "Bookings",
                column: "ComboId");

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_VoucherId",
                table: "Bookings",
                column: "VoucherId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Combos_ComboId",
                table: "Bookings",
                column: "ComboId",
                principalTable: "Combos",
                principalColumn: "ComboId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Vouchers_VoucherId",
                table: "Bookings",
                column: "VoucherId",
                principalTable: "Vouchers",
                principalColumn: "VoucherId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Combos_ComboId",
                table: "Bookings");

            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Vouchers_VoucherId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Vouchers_VoucherCode",
                table: "Vouchers");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_ComboId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_VoucherId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "PlatformFeeAmount",
                table: "Services");

            migrationBuilder.DropColumn(
                name: "AdditionalServices",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Combos");

            migrationBuilder.DropColumn(
                name: "ComboId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "DiscountAmount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "FinalAmount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "OriginalAmount",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "VoucherCode",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "VoucherId",
                table: "Bookings");
        }
    }
}
