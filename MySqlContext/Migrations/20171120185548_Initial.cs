using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace MySqlContext.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    City = table.Column<string>(type: "longtext", nullable: true),
                    Country = table.Column<string>(type: "longtext", nullable: true),
                    State = table.Column<string>(type: "longtext", nullable: true),
                    Street = table.Column<string>(type: "longtext", nullable: true),
                    ZipCode = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EMail = table.Column<string>(type: "longtext", nullable: true),
                    Password = table.Column<string>(type: "longtext", nullable: true),
                    RefreshToken = table.Column<string>(type: "longtext", nullable: true),
                    RefreshTokenExpiration = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ResetHash = table.Column<string>(type: "longtext", nullable: true),
                    Salt = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAttributeSpecificationBases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    AttributeType = table.Column<string>(type: "longtext", nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    Discriminator = table.Column<string>(type: "longtext", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAttributeSpecificationBases", x => new { x.Id, x.ServiceTypeId });
                    table.ForeignKey(
                        name: "FK_ServiceAttributeSpecificationBases_ServiceTypes_ServiceTypeId",
                        column: x => x.ServiceTypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Issuer = table.Column<string>(type: "longtext", nullable: true),
                    OriginalIssuer = table.Column<string>(type: "longtext", nullable: true),
                    Type = table.Column<string>(type: "longtext", nullable: false),
                    User_Id = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: false),
                    ValueType = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Claims_Users_User_Id",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Discriminator = table.Column<string>(type: "longtext", nullable: false),
                    Firstname = table.Column<string>(type: "longtext", nullable: true),
                    Lastname = table.Column<string>(type: "longtext", nullable: true),
                    ProfilePicture = table.Column<byte[]>(type: "longblob", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                    table.ForeignKey(
                        name: "FK_People_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Budget = table.Column<decimal>(type: "decimal(65, 30)", nullable: true),
                    Description = table.Column<string>(type: "longtext", nullable: true),
                    End = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    HostId = table.Column<int>(type: "int", nullable: false),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "longtext", nullable: true),
                    Start = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Events_People_HostId",
                        column: x => x.HostId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Events_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "EventService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    LocationId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Profile = table.Column<string>(type: "longtext", nullable: true),
                    Salary = table.Column<decimal>(type: "decimal(65, 30)", nullable: false),
                    TypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventService", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EventService_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventService_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventService_ServiceTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AttendeeRelationships",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttendeeRelationships", x => new { x.EventId, x.PersonId });
                    table.ForeignKey(
                        name: "FK_AttendeeRelationships_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AttendeeRelationships_People_PersonId",
                        column: x => x.PersonId,
                        principalTable: "People",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ServiceSlots",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    EventId = table.Column<int>(type: "int", nullable: false),
                    BudgetTarget = table.Column<decimal>(type: "decimal(65, 30)", nullable: true),
                    End = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    Start = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TypeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceSlots", x => new { x.Id, x.EventId });
                    table.ForeignKey(
                        name: "FK_ServiceSlots_Events_EventId",
                        column: x => x.EventId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServiceSlots_ServiceTypes_TypeId",
                        column: x => x.TypeId,
                        principalTable: "ServiceTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAttributes",
                columns: table => new
                {
                    EventServiceModelId = table.Column<int>(type: "int", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    ServiceAttributeSpecificationId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAttributes", x => new { x.EventServiceModelId, x.ServiceTypeId, x.ServiceAttributeSpecificationId });
                    table.ForeignKey(
                        name: "FK_ServiceAttributes_EventService_EventServiceModelId",
                        column: x => x.EventServiceModelId,
                        principalTable: "EventService",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceAttributes_ServiceAttributeSpecificationBases_ServiceAttributeSpecificationId_ServiceTypeId",
                        columns: x => new { x.ServiceAttributeSpecificationId, x.ServiceTypeId },
                        principalTable: "ServiceAttributeSpecificationBases",
                        principalColumns: new[] { "Id", "ServiceTypeId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAgreements",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false),
                    ServiceSlotId = table.Column<int>(type: "int", nullable: false),
                    Comment = table.Column<string>(type: "longtext", nullable: true),
                    Cost = table.Column<int>(type: "int", nullable: false),
                    End = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    EventServiceModelId = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAgreements", x => new { x.EventId, x.ServiceSlotId });
                    table.ForeignKey(
                        name: "FK_ServiceAgreements_EventService_EventServiceModelId",
                        column: x => x.EventServiceModelId,
                        principalTable: "EventService",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceAgreements_ServiceSlots_ServiceSlotId_EventId",
                        columns: x => new { x.ServiceSlotId, x.EventId },
                        principalTable: "ServiceSlots",
                        principalColumns: new[] { "Id", "EventId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ServiceAgreementAttributes",
                columns: table => new
                {
                    EventId = table.Column<int>(type: "int", nullable: false),
                    ServiceAgreementAttributeSpecificationId = table.Column<int>(type: "int", nullable: false),
                    ServiceSlotId = table.Column<int>(type: "int", nullable: false),
                    ServiceTypeId = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<string>(type: "longtext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceAgreementAttributes", x => new { x.EventId, x.ServiceAgreementAttributeSpecificationId, x.ServiceSlotId, x.ServiceTypeId });
                    table.ForeignKey(
                        name: "FK_ServiceAgreementAttributes_ServiceAgreements_EventId_ServiceSlotId",
                        columns: x => new { x.EventId, x.ServiceSlotId },
                        principalTable: "ServiceAgreements",
                        principalColumns: new[] { "EventId", "ServiceSlotId" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ServiceAgreementAttributes_ServiceAttributeSpecificationBases_ServiceAgreementAttributeSpecificationId_ServiceTypeId",
                        columns: x => new { x.ServiceAgreementAttributeSpecificationId, x.ServiceTypeId },
                        principalTable: "ServiceAttributeSpecificationBases",
                        principalColumns: new[] { "Id", "ServiceTypeId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AttendeeRelationships_PersonId",
                table: "AttendeeRelationships",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_User_Id",
                table: "Claims",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Events_HostId",
                table: "Events",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_Events_LocationId",
                table: "Events",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EventService_LocationId",
                table: "EventService",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EventService_PersonId",
                table: "EventService",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_EventService_TypeId",
                table: "EventService",
                column: "TypeId");

            migrationBuilder.CreateIndex(
                name: "IX_People_UserId",
                table: "People",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAgreementAttributes_EventId_ServiceSlotId",
                table: "ServiceAgreementAttributes",
                columns: new[] { "EventId", "ServiceSlotId" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAgreementAttributes_ServiceAgreementAttributeSpecificationId_ServiceTypeId",
                table: "ServiceAgreementAttributes",
                columns: new[] { "ServiceAgreementAttributeSpecificationId", "ServiceTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAgreements_EventServiceModelId",
                table: "ServiceAgreements",
                column: "EventServiceModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAgreements_ServiceSlotId_EventId",
                table: "ServiceAgreements",
                columns: new[] { "ServiceSlotId", "EventId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAttributes_ServiceAttributeSpecificationId_ServiceTypeId",
                table: "ServiceAttributes",
                columns: new[] { "ServiceAttributeSpecificationId", "ServiceTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceAttributeSpecificationBases_ServiceTypeId",
                table: "ServiceAttributeSpecificationBases",
                column: "ServiceTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSlots_EventId",
                table: "ServiceSlots",
                column: "EventId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceSlots_TypeId",
                table: "ServiceSlots",
                column: "TypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AttendeeRelationships");

            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "ServiceAgreementAttributes");

            migrationBuilder.DropTable(
                name: "ServiceAttributes");

            migrationBuilder.DropTable(
                name: "ServiceAgreements");

            migrationBuilder.DropTable(
                name: "ServiceAttributeSpecificationBases");

            migrationBuilder.DropTable(
                name: "EventService");

            migrationBuilder.DropTable(
                name: "ServiceSlots");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "ServiceTypes");

            migrationBuilder.DropTable(
                name: "People");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
