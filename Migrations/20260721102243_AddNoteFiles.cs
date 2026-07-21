using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudentHub.API.Migrations
{
    /// <inheritdoc />
    public partial class AddNoteFiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUrls",
                table: "Notes");

            migrationBuilder.AddColumn<int>(
                name: "ViewCount",
                table: "Notes",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: false),
                    Size = table.Column<float>(type: "REAL", nullable: false),
                    NoteId = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Files_Notes_NoteId",
                        column: x => x.NoteId,
                        principalTable: "Notes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Files_NoteId",
                table: "Files",
                column: "NoteId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Files");

            migrationBuilder.DropColumn(
                name: "ViewCount",
                table: "Notes");

            migrationBuilder.AddColumn<string>(
                name: "FileUrls",
                table: "Notes",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
