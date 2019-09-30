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

    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Locations");
    }
}
