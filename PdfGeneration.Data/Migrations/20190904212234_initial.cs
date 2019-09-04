using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PdfGeneration.Data.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FirstName = table.Column<string>(nullable: true),
                    MiddleName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Ssn = table.Column<string>(nullable: true),
                    Suffix = table.Column<string>(nullable: true),
                    Nickname = table.Column<string>(nullable: true),
                    HomePhone = table.Column<string>(nullable: true),
                    DutyPhone = table.Column<string>(nullable: true),
                    OtherPhone = table.Column<string>(nullable: true),
                    Dob = table.Column<DateTime>(nullable: true),
                    StateOfBirth = table.Column<string>(nullable: true),
                    CityOfBirth = table.Column<string>(nullable: true),
                    MothersMaidenName = table.Column<string>(nullable: true),
                    Religion = table.Column<string>(nullable: true),
                    Race = table.Column<string>(nullable: true),
                    FingerPrints = table.Column<string>(nullable: true),
                    Bdi = table.Column<string>(nullable: true),
                    Unit = table.Column<string>(nullable: true),
                    HairColor = table.Column<string>(nullable: true),
                    SectionAssigned = table.Column<string>(nullable: true),
                    EyeColor = table.Column<string>(nullable: true),
                    AttachedSection = table.Column<string>(nullable: true),
                    BloodType = table.Column<string>(nullable: true),
                    MosRate = table.Column<string>(nullable: true),
                    Height = table.Column<string>(nullable: true),
                    Weight = table.Column<string>(nullable: true),
                    Gender = table.Column<string>(nullable: true),
                    Mpc = table.Column<string>(nullable: true),
                    Rank = table.Column<string>(nullable: true),
                    Basd = table.Column<string>(nullable: true),
                    Ets = table.Column<string>(nullable: true),
                    Dor = table.Column<string>(nullable: true),
                    Pebd = table.Column<string>(nullable: true),
                    Branch = table.Column<string>(nullable: true),
                    Edipi = table.Column<string>(nullable: true),
                    Allergies = table.Column<string>(nullable: true),
                    Remarks = table.Column<string>(nullable: true),
                    PicUrl = table.Column<string>(nullable: true),
                    PicPath = table.Column<string>(nullable: true),
                    PicFile = table.Column<string>(nullable: true),
                    PicName = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Guid = table.Column<Guid>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    LastName = table.Column<string>(nullable: true),
                    Username = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    SocketName = table.Column<string>(nullable: true),
                    Theme = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonAssociate",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    AssociateId = table.Column<int>(nullable: false),
                    PersonId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonAssociate", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PersonAssociate_Person_AssociateId",
                        column: x => x.AssociateId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PersonAssociate_Person_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Person",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Upload",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserId = table.Column<int>(nullable: false),
                    Url = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    File = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    FileType = table.Column<string>(nullable: true),
                    Size = table.Column<long>(nullable: false),
                    UploadDate = table.Column<DateTime>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Upload", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Upload_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonAssociate_AssociateId",
                table: "PersonAssociate",
                column: "AssociateId");

            migrationBuilder.CreateIndex(
                name: "IX_PersonAssociate_PersonId",
                table: "PersonAssociate",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Upload_UserId",
                table: "Upload",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonAssociate");

            migrationBuilder.DropTable(
                name: "Upload");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
