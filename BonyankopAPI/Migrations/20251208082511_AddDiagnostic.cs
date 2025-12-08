using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonyankopAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddDiagnostic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Diagnostics",
                columns: table => new
                {
                    DiagnosticId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ImageMetadataJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RiskLevel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProblemCategory = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProblemSubcategory = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProbableCause = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    RiskPrediction = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    RecommendedAction = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    AiConfidenceScore = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    AiModelVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProcessingTimeMs = table.Column<int>(type: "int", nullable: true),
                    IsDiyPossible = table.Column<bool>(type: "bit", nullable: false),
                    EstimatedCostRange = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UrgencyLevel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagnostics", x => x.DiagnosticId);
                    table.ForeignKey(
                        name: "FK_Diagnostics_Users_CitizenId",
                        column: x => x.CitizenId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Diagnostics_CitizenId",
                table: "Diagnostics",
                column: "CitizenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Diagnostics");
        }
    }
}
