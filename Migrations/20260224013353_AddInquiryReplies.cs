using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OECLWebsite.Migrations
{
    /// <inheritdoc />
    public partial class AddInquiryReplies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InquiryReplies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceInquiryId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Body = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SentByUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    SentByName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmailSentSuccessfully = table.Column<bool>(type: "bit", nullable: false),
                    IsInternalNote = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InquiryReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InquiryReplies_ServiceInquiries_ServiceInquiryId",
                        column: x => x.ServiceInquiryId,
                        principalTable: "ServiceInquiries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InquiryReplies_ServiceInquiryId",
                table: "InquiryReplies",
                column: "ServiceInquiryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InquiryReplies");
        }
    }
}
