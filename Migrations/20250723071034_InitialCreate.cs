using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication6.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FicheProjets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumeroProjet = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NumeroAffaire = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContratNumero = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NomClient = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelephoneContact = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdresseLivraison = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModalitePaiement = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DernierDelaiLivraison = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DernierDelaiExecution = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DureeGarantie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DetailFormation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DossierAMSSNUR = table.Column<bool>(type: "bit", nullable: false),
                    ContratMaintenance = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FicheProjets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FicheDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Equipements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Accessoires = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TravauxPromamec = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Commentaires = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FicheProjetId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FicheDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FicheDetails_FicheProjets_FicheProjetId",
                        column: x => x.FicheProjetId,
                        principalTable: "FicheProjets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FicheMetaDonnees",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FicheProjetId = table.Column<int>(type: "int", nullable: false),
                    CreePar = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdresseIP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SessionMachine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FicheMetaDonnees", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FicheMetaDonnees_FicheProjets_FicheProjetId",
                        column: x => x.FicheProjetId,
                        principalTable: "FicheProjets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FicheModifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FicheProjetId = table.Column<int>(type: "int", nullable: false),
                    ChampsModifie = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AncienneValeur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NouvelleValeur = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateModification = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FicheModifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FicheModifications_FicheProjets_FicheProjetId",
                        column: x => x.FicheProjetId,
                        principalTable: "FicheProjets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FicheDetails_FicheProjetId",
                table: "FicheDetails",
                column: "FicheProjetId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FicheMetaDonnees_FicheProjetId",
                table: "FicheMetaDonnees",
                column: "FicheProjetId");

            migrationBuilder.CreateIndex(
                name: "IX_FicheModifications_FicheProjetId",
                table: "FicheModifications",
                column: "FicheProjetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FicheDetails");

            migrationBuilder.DropTable(
                name: "FicheMetaDonnees");

            migrationBuilder.DropTable(
                name: "FicheModifications");

            migrationBuilder.DropTable(
                name: "FicheProjets");
        }
    }
}
