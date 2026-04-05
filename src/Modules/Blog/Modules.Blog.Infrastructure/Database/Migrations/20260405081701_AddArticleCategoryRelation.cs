using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modules.Blog.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddArticleCategoryRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoryId",
                schema: "blog",
                table: "Articles",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Categories",
                schema: "blog",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Slug = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ModifiedAtUtc = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_CategoryId",
                schema: "blog",
                table: "Articles",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Slug",
                schema: "blog",
                table: "Categories",
                column: "Slug",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Articles_Categories_CategoryId",
                schema: "blog",
                table: "Articles",
                column: "CategoryId",
                principalSchema: "blog",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Articles_Categories_CategoryId",
                schema: "blog",
                table: "Articles");

            migrationBuilder.DropTable(
                name: "Categories",
                schema: "blog");

            migrationBuilder.DropIndex(
                name: "IX_Articles_CategoryId",
                schema: "blog",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                schema: "blog",
                table: "Articles");
        }
    }
}
