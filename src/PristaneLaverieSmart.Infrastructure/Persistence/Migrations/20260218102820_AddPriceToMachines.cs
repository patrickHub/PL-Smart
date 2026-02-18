using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PristaneLaverieSmart.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPriceToMachines : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PricePerCicle",
                table: "Machines",
                newName: "PricePerCycle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PricePerCycle",
                table: "Machines",
                newName: "PricePerCicle");
        }
    }
}
