﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "mst_user",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying", nullable: false),
                    email = table.Column<string>(type: "character varying", nullable: false),
                    password = table.Column<string>(type: "character varying", nullable: false),
                    role = table.Column<string>(type: "character varying", nullable: false),
                    balance = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("mst_user_pk", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "mst_loans",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    borrower_id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    interest_rate = table.Column<decimal>(type: "numeric", nullable: false),
                    duration_month = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_mst_loans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_mst_loans_mst_user_borrower_id",
                        column: x => x.borrower_id,
                        principalTable: "mst_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "trn_funding",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    loan_id = table.Column<string>(type: "text", nullable: false),
                    lender_id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    funded_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_funding", x => x.Id);
                    table.ForeignKey(
                        name: "FK_trn_funding_mst_loans_loan_id",
                        column: x => x.loan_id,
                        principalTable: "mst_loans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_trn_funding_mst_user_UserId",
                        column: x => x.UserId,
                        principalTable: "mst_user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_mst_loans_borrower_id",
                table: "mst_loans",
                column: "borrower_id");

            migrationBuilder.CreateIndex(
                name: "IX_trn_funding_loan_id",
                table: "trn_funding",
                column: "loan_id");

            migrationBuilder.CreateIndex(
                name: "IX_trn_funding_UserId",
                table: "trn_funding",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trn_funding");

            migrationBuilder.DropTable(
                name: "mst_loans");

            migrationBuilder.DropTable(
                name: "mst_user");
        }
    }
}
