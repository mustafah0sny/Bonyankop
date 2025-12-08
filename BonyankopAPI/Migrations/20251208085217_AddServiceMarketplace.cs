using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BonyankopAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceMarketplace : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceRequests",
                columns: table => new
                {
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DiagnosticId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CitizenId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProblemTitle = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProblemDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    ProblemCategory = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AdditionalImages = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferredProviderType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PreferredServiceDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PropertyType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PropertyAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ContactPhone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SelectedQuoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    QuotesCount = table.Column<int>(type: "int", nullable: false),
                    ViewsCount = table.Column<int>(type: "int", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceRequests", x => x.RequestId);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_Diagnostics_DiagnosticId",
                        column: x => x.DiagnosticId,
                        principalTable: "Diagnostics",
                        principalColumn: "DiagnosticId",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ServiceRequests_Users_CitizenId",
                        column: x => x.CitizenId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Quotes",
                columns: table => new
                {
                    QuoteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProviderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EstimatedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CostBreakdownJson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedDurationDays = table.Column<int>(type: "int", nullable: true),
                    TechnicalAssessment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ProposedSolution = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    MaterialsIncluded = table.Column<bool>(type: "bit", nullable: false),
                    WarrantyPeriodMonths = table.Column<int>(type: "int", nullable: true),
                    TermsAndConditions = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ValidityPeriodDays = table.Column<int>(type: "int", nullable: false),
                    Attachments = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotes", x => x.QuoteId);
                    table.ForeignKey(
                        name: "FK_Quotes_ProviderProfiles_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "ProviderProfiles",
                        principalColumn: "ProviderId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Quotes_ServiceRequests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "ServiceRequests",
                        principalColumn: "RequestId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_ProviderId",
                table: "Quotes",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotes_RequestId",
                table: "Quotes",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_CitizenId",
                table: "ServiceRequests",
                column: "CitizenId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceRequests_DiagnosticId",
                table: "ServiceRequests",
                column: "DiagnosticId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Quotes");

            migrationBuilder.DropTable(
                name: "ServiceRequests");
        }
    }
}
