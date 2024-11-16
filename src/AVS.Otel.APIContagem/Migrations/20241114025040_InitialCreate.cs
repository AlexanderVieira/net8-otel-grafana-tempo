using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AVS.Otel.APIContagem.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoricoContagem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DataProcessamento = table.Column<DateTime>(type: "TIMESTAMP", nullable: false),
                    ValorAtual = table.Column<int>(type: "INTEGER", nullable: false),
                    Producer = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                    Kernel = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Framework = table.Column<string>(type: "TEXT", maxLength: 80, nullable: false),
                    Mensagem = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoContagem", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricoContagem");
        }
    }
}
