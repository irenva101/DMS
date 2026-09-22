using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DLMS.Tenants.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "tenant");

            migrationBuilder.CreateTable(
                name: "permissions",
                schema: "tenant",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    value = table.Column<string>(type: "varchar", maxLength: 40, nullable: false, defaultValue: "test"),
                    dms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permissions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "permission_entity_role_entity",
                schema: "tenant",
                columns: table => new
                {
                    permission_id = table.Column<long>(type: "bigint", nullable: false),
                    role_id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_permission_entity_role_entity", x => new { x.permission_id, x.role_id });
                    table.ForeignKey(
                        name: "FK_PermissionRole_Permission",
                        column: x => x.permission_id,
                        principalSchema: "tenant",
                        principalTable: "permissions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "roles",
                schema: "tenant",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    status = table.Column<string>(type: "varchar", maxLength: 40, nullable: false, defaultValue: "Inactive"),
                    utility_id = table.Column<long>(type: "bigint", nullable: false),
                    dms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "users",
                schema: "tenant",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    first_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    middle_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    last_name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    email = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<long>(type: "bigint", nullable: true),
                    status = table.Column<string>(type: "varchar", maxLength: 40, nullable: false, defaultValue: "Inactive"),
                    address = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    image = table.Column<string>(type: "text", nullable: true),
                    created_by = table.Column<long>(type: "bigint", nullable: true),
                    created_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    utility_id = table.Column<long>(type: "bigint", nullable: true),
                    is_admin = table.Column<bool>(type: "boolean", nullable: false),
                    dms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.id);
                    table.ForeignKey(
                        name: "FK_users_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "tenant",
                        principalTable: "roles",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_users_users_created_by",
                        column: x => x.created_by,
                        principalSchema: "tenant",
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "Utilities",
                schema: "tenant",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    name = table.Column<string>(type: "text", nullable: false),
                    acronym = table.Column<string>(type: "text", nullable: false),
                    contact_person = table.Column<long>(type: "bigint", nullable: true),
                    api_key = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<string>(type: "varchar", maxLength: 40, nullable: false, defaultValue: "Active"),
                    address = table.Column<string>(type: "text", nullable: false),
                    website_address = table.Column<string>(type: "text", nullable: false),
                    time_zone = table.Column<int>(type: "integer", nullable: false),
                    creation_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    logo_path = table.Column<string>(type: "text", nullable: true),
                    dms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Utilities", x => x.id);
                    table.ForeignKey(
                        name: "FK_Utilities_users_contact_person",
                        column: x => x.contact_person,
                        principalSchema: "tenant",
                        principalTable: "users",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_permission_entity_role_entity_role_id",
                schema: "tenant",
                table: "permission_entity_role_entity",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_permissions_value",
                schema: "tenant",
                table: "permissions",
                column: "value",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_roles_created_by",
                schema: "tenant",
                table: "roles",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_roles_utility_id",
                schema: "tenant",
                table: "roles",
                column: "utility_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_created_by",
                schema: "tenant",
                table: "users",
                column: "created_by");

            migrationBuilder.CreateIndex(
                name: "IX_users_role_id",
                schema: "tenant",
                table: "users",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "IX_users_utility_id",
                schema: "tenant",
                table: "users",
                column: "utility_id");

            migrationBuilder.CreateIndex(
                name: "IX_Utilities_contact_person",
                schema: "tenant",
                table: "Utilities",
                column: "contact_person");

            migrationBuilder.AddForeignKey(
                name: "FK_PermissionRole_Role",
                schema: "tenant",
                table: "permission_entity_role_entity",
                column: "role_id",
                principalSchema: "tenant",
                principalTable: "roles",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_roles_Utilities_utility_id",
                schema: "tenant",
                table: "roles",
                column: "utility_id",
                principalSchema: "tenant",
                principalTable: "Utilities",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_roles_users_created_by",
                schema: "tenant",
                table: "roles",
                column: "created_by",
                principalSchema: "tenant",
                principalTable: "users",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_users_Utilities_utility_id",
                schema: "tenant",
                table: "users",
                column: "utility_id",
                principalSchema: "tenant",
                principalTable: "Utilities",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_users_roles_role_id",
                schema: "tenant",
                table: "users");

            migrationBuilder.DropForeignKey(
                name: "FK_users_Utilities_utility_id",
                schema: "tenant",
                table: "users");

            migrationBuilder.DropTable(
                name: "permission_entity_role_entity",
                schema: "tenant");

            migrationBuilder.DropTable(
                name: "permissions",
                schema: "tenant");

            migrationBuilder.DropTable(
                name: "roles",
                schema: "tenant");

            migrationBuilder.DropTable(
                name: "Utilities",
                schema: "tenant");

            migrationBuilder.DropTable(
                name: "users",
                schema: "tenant");
        }
    }
}
