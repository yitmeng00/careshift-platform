using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicalScheduler.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLeaveTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"LeaveRequests\" SET \"LeaveType\" = 'Annual' WHERE \"LeaveType\" = 'Training'");
            migrationBuilder.Sql("UPDATE \"LeaveRequests\" SET \"LeaveType\" = 'Emergency' WHERE \"LeaveType\" = 'Personal'");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"LeaveRequests\" SET \"LeaveType\" = 'Training' WHERE \"LeaveType\" = 'Annual'");
            migrationBuilder.Sql("UPDATE \"LeaveRequests\" SET \"LeaveType\" = 'Personal' WHERE \"LeaveType\" = 'Emergency'");
        }
    }
}
