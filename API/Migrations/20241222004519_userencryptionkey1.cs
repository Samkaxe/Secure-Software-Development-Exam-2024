using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class userencryptionkey1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptionKey",
                table: "MedicalRecords");

            migrationBuilder.AddColumn<byte[]>(
                name: "EncryptedUek",
                table: "Users",
                type: "BLOB",
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AlterColumn<byte[]>(
                name: "RecordData",
                table: "MedicalRecords",
                type: "BLOB",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedUek",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "RecordData",
                table: "MedicalRecords",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "BLOB");

            migrationBuilder.AddColumn<string>(
                name: "EncryptionKey",
                table: "MedicalRecords",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }
    }
}
