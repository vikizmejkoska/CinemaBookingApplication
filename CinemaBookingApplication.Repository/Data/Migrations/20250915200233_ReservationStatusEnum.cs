using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CinemaBookingApplication.Web.Data.Migrations
{
  

    public partial class ReservationStatusEnum : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1) Add a temporary int column
            migrationBuilder.AddColumn<int>(
                name: "StatusInt",
                table: "Reservations",
                type: "int",
                nullable: true);

            // 2) Copy values from the old string Status into the new int column
            //    Adjust the mapping if your enum values differ.
            migrationBuilder.Sql(@"
            UPDATE Reservations
            SET StatusInt =
                CASE UPPER(LTRIM(RTRIM(ISNULL(Status, ''))))
                    WHEN 'PENDING'   THEN 0
                    WHEN 'CONFIRMED' THEN 1
                    WHEN 'CANCELLED' THEN 2
                    ELSE 0
                END
        ");

            // 3) Drop the old string column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reservations");

            // 4) Rename the temp column to the final column name
            migrationBuilder.RenameColumn(
                name: "StatusInt",
                table: "Reservations",
                newName: "Status");

            // 5) Make it NOT NULL with default 0 (Pending)
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Reservations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1) Add a temp string column
            migrationBuilder.AddColumn<string>(
                name: "StatusText",
                table: "Reservations",
                type: "nvarchar(50)",
                nullable: true);

            // 2) Copy back ints → strings
            migrationBuilder.Sql(@"
            UPDATE Reservations
            SET StatusText =
                CASE Status
                    WHEN 0 THEN 'Pending'
                    WHEN 1 THEN 'Confirmed'
                    WHEN 2 THEN 'Cancelled'
                    ELSE 'Pending'
                END
        ");

            // 3) Drop the int column
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Reservations");

            // 4) Rename temp back to Status
            migrationBuilder.RenameColumn(
                name: "StatusText",
                table: "Reservations",
                newName: "Status");
        }
    }

}
