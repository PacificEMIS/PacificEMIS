﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace opensis.data.Migrations.MySqlMigrations
{
    public partial class AddMembershipIdInStaffSchoolInfoTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "membership_id",
                table: "staff_school_info",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "honor_roll",
                table: "honor_rolls",
                type: "varchar(50)",
                maxLength: 50,
                nullable: true,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(20)",
                oldMaxLength: 20,
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");

            migrationBuilder.CreateIndex(
                name: "IX_staff_school_info_tenant_id_school_id_membership_id",
                table: "staff_school_info",
                columns: new[] { "tenant_id", "school_id", "membership_id" });

            migrationBuilder.AddForeignKey(
                name: "staff_school_info$FK_membership",
                table: "staff_school_info",
                columns: new[] { "tenant_id", "school_id", "membership_id" },
                principalTable: "membership",
                principalColumns: new[] { "tenant_id", "school_id", "membership_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "staff_school_info$FK_membership",
                table: "staff_school_info");

            migrationBuilder.DropIndex(
                name: "IX_staff_school_info_tenant_id_school_id_membership_id",
                table: "staff_school_info");

            migrationBuilder.DropColumn(
                name: "membership_id",
                table: "staff_school_info");

            migrationBuilder.AlterColumn<string>(
                name: "honor_roll",
                table: "honor_rolls",
                type: "varchar(20)",
                maxLength: 20,
                nullable: true,
                collation: "utf8mb4_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(50)",
                oldMaxLength: 50,
                oldNullable: true)
                .OldAnnotation("Relational:Collation", "utf8mb4_general_ci");
        }
    }
}
