using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourceWork.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleToEmployee : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Employee",
                columns: table => new
                {
                    employee_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    hire_date = table.Column<DateOnly>(type: "date", nullable: false),
                    position = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    login = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employee__C52E0BA8625D2B30", x => x.employee_id);
                });

            migrationBuilder.CreateTable(
                name: "Genre",
                columns: table => new
                {
                    genre_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Genre__18428D426CCEE2DA", x => x.genre_id);
                });

            migrationBuilder.CreateTable(
                name: "Guest",
                columns: table => new
                {
                    guest_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Guest__19778E352E3B73BE", x => x.guest_id);
                });

            migrationBuilder.CreateTable(
                name: "Schedule",
                columns: table => new
                {
                    schedule_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    week_number = table.Column<int>(type: "int", nullable: false),
                    month_number = table.Column<int>(type: "int", nullable: false),
                    year = table.Column<int>(type: "int", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Schedule__C46A8A6FB9054CF4", x => x.schedule_id);
                });

            migrationBuilder.CreateTable(
                name: "TVShow",
                columns: table => new
                {
                    show_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    genre_id = table.Column<int>(type: "int", nullable: false),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    duration = table.Column<int>(type: "int", nullable: false),
                    rating = table.Column<decimal>(type: "decimal(3,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TVShow__2B97D71C540A5873", x => x.show_id);
                    table.ForeignKey(
                        name: "FK__TVShow__genre_id__3D5E1FD2",
                        column: x => x.genre_id,
                        principalTable: "Genre",
                        principalColumn: "genre_id");
                });

            migrationBuilder.CreateTable(
                name: "CitizenAppeal",
                columns: table => new
                {
                    appeal_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    full_name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    organization = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    show_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__CitizenA__DFAC766DB2A29641", x => x.appeal_id);
                    table.ForeignKey(
                        name: "FK__CitizenAp__show___4AB81AF0",
                        column: x => x.show_id,
                        principalTable: "TVShow",
                        principalColumn: "show_id");
                });

            migrationBuilder.CreateTable(
                name: "Schedule_TVShow",
                columns: table => new
                {
                    schedule_tvshow_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    schedule_id = table.Column<int>(type: "int", nullable: false),
                    show_id = table.Column<int>(type: "int", nullable: false),
                    start_time = table.Column<DateTime>(type: "datetime", nullable: false),
                    end_time = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Schedule__B35EF5FE082385AC", x => x.schedule_tvshow_id);
                    table.ForeignKey(
                        name: "FK__Schedule___sched__4D94879B",
                        column: x => x.schedule_id,
                        principalTable: "Schedule",
                        principalColumn: "schedule_id");
                    table.ForeignKey(
                        name: "FK__Schedule___show___4E88ABD4",
                        column: x => x.show_id,
                        principalTable: "TVShow",
                        principalColumn: "show_id");
                });

            migrationBuilder.CreateTable(
                name: "TVShow_Employee",
                columns: table => new
                {
                    show_id = table.Column<int>(type: "int", nullable: false),
                    employee_id = table.Column<int>(type: "int", nullable: false),
                    tvshow_employee_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TVShow_E__B7C537A655637BFA", x => new { x.show_id, x.employee_id });
                    table.ForeignKey(
                        name: "FK__TVShow_Em__emplo__412EB0B6",
                        column: x => x.employee_id,
                        principalTable: "Employee",
                        principalColumn: "employee_id");
                    table.ForeignKey(
                        name: "FK__TVShow_Em__show___403A8C7D",
                        column: x => x.show_id,
                        principalTable: "TVShow",
                        principalColumn: "show_id");
                });

            migrationBuilder.CreateTable(
                name: "TVShow_Guest",
                columns: table => new
                {
                    show_id = table.Column<int>(type: "int", nullable: false),
                    guest_id = table.Column<int>(type: "int", nullable: false),
                    tvshow_guest_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__TVShow_G__6A00AFFF1DFDF410", x => new { x.show_id, x.guest_id });
                    table.ForeignKey(
                        name: "FK__TVShow_Gu__guest__44FF419A",
                        column: x => x.guest_id,
                        principalTable: "Guest",
                        principalColumn: "guest_id");
                    table.ForeignKey(
                        name: "FK__TVShow_Gu__show___440B1D61",
                        column: x => x.show_id,
                        principalTable: "TVShow",
                        principalColumn: "show_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CitizenAppeal_show_id",
                table: "CitizenAppeal",
                column: "show_id");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_TVShow_schedule_id",
                table: "Schedule_TVShow",
                column: "schedule_id");

            migrationBuilder.CreateIndex(
                name: "IX_Schedule_TVShow_show_id",
                table: "Schedule_TVShow",
                column: "show_id");

            migrationBuilder.CreateIndex(
                name: "IX_TVShow_genre_id",
                table: "TVShow",
                column: "genre_id");

            migrationBuilder.CreateIndex(
                name: "IX_TVShow_Employee_employee_id",
                table: "TVShow_Employee",
                column: "employee_id");

            migrationBuilder.CreateIndex(
                name: "IX_TVShow_Guest_guest_id",
                table: "TVShow_Guest",
                column: "guest_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CitizenAppeal");

            migrationBuilder.DropTable(
                name: "Schedule_TVShow");

            migrationBuilder.DropTable(
                name: "TVShow_Employee");

            migrationBuilder.DropTable(
                name: "TVShow_Guest");

            migrationBuilder.DropTable(
                name: "Schedule");

            migrationBuilder.DropTable(
                name: "Employee");

            migrationBuilder.DropTable(
                name: "Guest");

            migrationBuilder.DropTable(
                name: "TVShow");

            migrationBuilder.DropTable(
                name: "Genre");
        }
    }
}
