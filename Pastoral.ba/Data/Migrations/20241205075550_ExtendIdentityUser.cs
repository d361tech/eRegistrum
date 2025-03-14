using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Pastoral.ba.Data.Migrations
{
    /// <inheritdoc />
    public partial class ExtendIdentityUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JMBG",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PunoIme",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ZupaId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Biskupije",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_GenVik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GenVikarId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ID_Ekonom = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EkonomId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Biskupije", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Biskupije_AspNetUsers_EkonomId",
                        column: x => x.EkonomId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Biskupije_AspNetUsers_GenVikarId",
                        column: x => x.GenVikarId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Zupe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_Biskupija = table.Column<int>(type: "int", nullable: false),
                    BiskupijaId = table.Column<int>(type: "int", nullable: false),
                    ID_Zupnik = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ZupnikId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zupe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zupe_AspNetUsers_ZupnikId",
                        column: x => x.ZupnikId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Zupe_Biskupije_BiskupijaId",
                        column: x => x.BiskupijaId,
                        principalTable: "Biskupije",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Osobe",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JMBG = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PunoIme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ID_Zupe = table.Column<int>(type: "int", nullable: false),
                    ZupaId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Osobe", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Osobe_Zupe_ZupaId",
                        column: x => x.ZupaId,
                        principalTable: "Zupe",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_ZupaId",
                table: "AspNetUsers",
                column: "ZupaId");

            migrationBuilder.CreateIndex(
                name: "IX_Biskupije_EkonomId",
                table: "Biskupije",
                column: "EkonomId");

            migrationBuilder.CreateIndex(
                name: "IX_Biskupije_GenVikarId",
                table: "Biskupije",
                column: "GenVikarId");

            migrationBuilder.CreateIndex(
                name: "IX_Osobe_ZupaId",
                table: "Osobe",
                column: "ZupaId");

            migrationBuilder.CreateIndex(
                name: "IX_Zupe_BiskupijaId",
                table: "Zupe",
                column: "BiskupijaId");

            migrationBuilder.CreateIndex(
                name: "IX_Zupe_ZupnikId",
                table: "Zupe",
                column: "ZupnikId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Zupe_ZupaId",
                table: "AspNetUsers",
                column: "ZupaId",
                principalTable: "Zupe",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Zupe_ZupaId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Osobe");

            migrationBuilder.DropTable(
                name: "Zupe");

            migrationBuilder.DropTable(
                name: "Biskupije");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_ZupaId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "JMBG",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PunoIme",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ZupaId",
                table: "AspNetUsers");
        }
    }
}
