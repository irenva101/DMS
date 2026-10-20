using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace DMS.Library.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dms");

            migrationBuilder.CreateTable(
                name: "code_sequences",
                schema: "dms",
                columns: table => new
                {
                    prefix = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    year = table.Column<int>(type: "integer", nullable: false),
                    last_value = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_code_sequences", x => new { x.prefix, x.year });
                });

            migrationBuilder.CreateTable(
                name: "companies",
                schema: "dms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tax_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    registration_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    country_code = table.Column<string>(type: "char(2)", maxLength: 2, nullable: false),
                    address = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    dms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_companies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "form_templates",
                schema: "dms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    category = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    dms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_form_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "process_types",
                schema: "dms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    module = table.Column<string>(type: "varchar", maxLength: 40, nullable: false, defaultValue: "BusinessTransaction"),
                    code_prefix = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    dms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_process_types", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "processes",
                schema: "dms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    company_id = table.Column<long>(type: "bigint", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    sent_to_sef = table.Column<bool>(type: "boolean", nullable: false),
                    process_type_id = table.Column<long>(type: "bigint", nullable: false),
                    parent_process_id = table.Column<long>(type: "bigint", nullable: true),
                    dms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_processes", x => x.id);
                    table.ForeignKey(
                        name: "FK_processes_companies_company_id",
                        column: x => x.company_id,
                        principalSchema: "dms",
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_processes_process_types_process_type_id",
                        column: x => x.process_type_id,
                        principalSchema: "dms",
                        principalTable: "process_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_processes_processes_parent_process_id",
                        column: x => x.parent_process_id,
                        principalSchema: "dms",
                        principalTable: "processes",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "step_definitions",
                schema: "dms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    step_order = table.Column<int>(type: "integer", nullable: false),
                    process_type_id = table.Column<long>(type: "bigint", nullable: false),
                    dms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_step_definitions", x => x.id);
                    table.ForeignKey(
                        name: "FK_step_definitions_process_types_process_type_id",
                        column: x => x.process_type_id,
                        principalSchema: "dms",
                        principalTable: "process_types",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bills",
                schema: "dms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    issue_date = table.Column<DateOnly>(type: "date", nullable: false),
                    maturity_date = table.Column<DateOnly>(type: "date", nullable: false),
                    company_id = table.Column<long>(type: "bigint", nullable: false),
                    process_id = table.Column<long>(type: "bigint", nullable: true),
                    dms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_bills", x => x.id);
                    table.ForeignKey(
                        name: "FK_bills_companies_company_id",
                        column: x => x.company_id,
                        principalSchema: "dms",
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_bills_processes_process_id",
                        column: x => x.process_id,
                        principalSchema: "dms",
                        principalTable: "processes",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "document_requirements",
                schema: "dms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    is_required = table.Column<bool>(type: "boolean", nullable: false),
                    step_definition_id = table.Column<long>(type: "bigint", nullable: false),
                    dms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_document_requirements", x => x.id);
                    table.ForeignKey(
                        name: "FK_document_requirements_step_definitions_step_definition_id",
                        column: x => x.step_definition_id,
                        principalSchema: "dms",
                        principalTable: "step_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "steps",
                schema: "dms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    is_verified = table.Column<bool>(type: "boolean", nullable: false),
                    process_id = table.Column<long>(type: "bigint", nullable: false),
                    step_definition_id = table.Column<long>(type: "bigint", nullable: false),
                    dms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_steps", x => x.id);
                    table.ForeignKey(
                        name: "FK_steps_processes_process_id",
                        column: x => x.process_id,
                        principalSchema: "dms",
                        principalTable: "processes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_steps_step_definitions_step_definition_id",
                        column: x => x.step_definition_id,
                        principalSchema: "dms",
                        principalTable: "step_definitions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contracts",
                schema: "dms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    signed_date = table.Column<DateOnly>(type: "date", nullable: false),
                    expiry_date = table.Column<DateOnly>(type: "date", nullable: true),
                    value = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: true),
                    currency = table.Column<string>(type: "char(3)", maxLength: 3, nullable: false),
                    company_id = table.Column<long>(type: "bigint", nullable: false),
                    fulfills_step_id = table.Column<long>(type: "bigint", nullable: true),
                    dms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_contracts", x => x.id);
                    table.ForeignKey(
                        name: "FK_contracts_companies_company_id",
                        column: x => x.company_id,
                        principalSchema: "dms",
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_contracts_steps_fulfills_step_id",
                        column: x => x.fulfills_step_id,
                        principalSchema: "dms",
                        principalTable: "steps",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "documents",
                schema: "dms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    original_file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    storage_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    content_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    size_bytes = table.Column<long>(type: "bigint", nullable: false),
                    step_id = table.Column<long>(type: "bigint", nullable: true),
                    requirement_id = table.Column<long>(type: "bigint", nullable: true),
                    contract_id = table.Column<long>(type: "bigint", nullable: true),
                    bill_id = table.Column<long>(type: "bigint", nullable: true),
                    form_template_id = table.Column<long>(type: "bigint", nullable: true),
                    dms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_documents", x => x.id);
                    table.CheckConstraint("CK_documents_requirement_needs_step", "requirement_id IS NULL OR step_id IS NOT NULL");
                    table.CheckConstraint("CK_documents_single_owner", "num_nonnulls(step_id, contract_id, bill_id, form_template_id) <= 1");
                    table.ForeignKey(
                        name: "FK_documents_bills_bill_id",
                        column: x => x.bill_id,
                        principalSchema: "dms",
                        principalTable: "bills",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_documents_contracts_contract_id",
                        column: x => x.contract_id,
                        principalSchema: "dms",
                        principalTable: "contracts",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_documents_document_requirements_requirement_id",
                        column: x => x.requirement_id,
                        principalSchema: "dms",
                        principalTable: "document_requirements",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_documents_form_templates_form_template_id",
                        column: x => x.form_template_id,
                        principalSchema: "dms",
                        principalTable: "form_templates",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "FK_documents_steps_step_id",
                        column: x => x.step_id,
                        principalSchema: "dms",
                        principalTable: "steps",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "audit_logs",
                schema: "dms",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn),
                    time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "varchar", maxLength: 40, nullable: false, defaultValue: "DocumentAttached"),
                    process_id = table.Column<long>(type: "bigint", nullable: true),
                    step_id = table.Column<long>(type: "bigint", nullable: true),
                    document_id = table.Column<long>(type: "bigint", nullable: true),
                    contract_id = table.Column<long>(type: "bigint", nullable: true),
                    bill_id = table.Column<long>(type: "bigint", nullable: true),
                    description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    dms_id = table.Column<Guid>(type: "uuid", nullable: false),
                    is_deleted = table.Column<bool>(type: "boolean", nullable: false),
                    last_update_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_audit_logs", x => x.id);
                    table.ForeignKey(
                        name: "FK_audit_logs_bills_bill_id",
                        column: x => x.bill_id,
                        principalSchema: "dms",
                        principalTable: "bills",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_audit_logs_contracts_contract_id",
                        column: x => x.contract_id,
                        principalSchema: "dms",
                        principalTable: "contracts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_audit_logs_documents_document_id",
                        column: x => x.document_id,
                        principalSchema: "dms",
                        principalTable: "documents",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_audit_logs_processes_process_id",
                        column: x => x.process_id,
                        principalSchema: "dms",
                        principalTable: "processes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_audit_logs_steps_step_id",
                        column: x => x.step_id,
                        principalSchema: "dms",
                        principalTable: "steps",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_bill_id",
                schema: "dms",
                table: "audit_logs",
                column: "bill_id");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_contract_id",
                schema: "dms",
                table: "audit_logs",
                column: "contract_id");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_document_id",
                schema: "dms",
                table: "audit_logs",
                column: "document_id");

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_process_id_time",
                schema: "dms",
                table: "audit_logs",
                columns: new[] { "process_id", "time" });

            migrationBuilder.CreateIndex(
                name: "IX_audit_logs_step_id",
                schema: "dms",
                table: "audit_logs",
                column: "step_id");

            migrationBuilder.CreateIndex(
                name: "IX_bills_code",
                schema: "dms",
                table: "bills",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_bills_company_id",
                schema: "dms",
                table: "bills",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_bills_process_id",
                schema: "dms",
                table: "bills",
                column: "process_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_companies_tax_id",
                schema: "dms",
                table: "companies",
                column: "tax_id",
                unique: true,
                filter: "tax_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_contracts_code",
                schema: "dms",
                table: "contracts",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_contracts_company_id",
                schema: "dms",
                table: "contracts",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_contracts_fulfills_step_id",
                schema: "dms",
                table: "contracts",
                column: "fulfills_step_id");

            migrationBuilder.CreateIndex(
                name: "IX_document_requirements_step_definition_id",
                schema: "dms",
                table: "document_requirements",
                column: "step_definition_id");

            migrationBuilder.CreateIndex(
                name: "IX_documents_bill_id",
                schema: "dms",
                table: "documents",
                column: "bill_id");

            migrationBuilder.CreateIndex(
                name: "IX_documents_contract_id",
                schema: "dms",
                table: "documents",
                column: "contract_id");

            migrationBuilder.CreateIndex(
                name: "IX_documents_form_template_id",
                schema: "dms",
                table: "documents",
                column: "form_template_id");

            migrationBuilder.CreateIndex(
                name: "IX_documents_requirement_id",
                schema: "dms",
                table: "documents",
                column: "requirement_id");

            migrationBuilder.CreateIndex(
                name: "IX_documents_step_id",
                schema: "dms",
                table: "documents",
                column: "step_id");

            migrationBuilder.CreateIndex(
                name: "IX_documents_storage_key",
                schema: "dms",
                table: "documents",
                column: "storage_key",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_form_templates_code",
                schema: "dms",
                table: "form_templates",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_processes_code",
                schema: "dms",
                table: "processes",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_processes_company_id",
                schema: "dms",
                table: "processes",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_processes_parent_process_id",
                schema: "dms",
                table: "processes",
                column: "parent_process_id");

            migrationBuilder.CreateIndex(
                name: "IX_processes_process_type_id",
                schema: "dms",
                table: "processes",
                column: "process_type_id");

            migrationBuilder.CreateIndex(
                name: "IX_step_definitions_process_type_id_step_order",
                schema: "dms",
                table: "step_definitions",
                columns: new[] { "process_type_id", "step_order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_steps_process_id_step_definition_id",
                schema: "dms",
                table: "steps",
                columns: new[] { "process_id", "step_definition_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_steps_step_definition_id",
                schema: "dms",
                table: "steps",
                column: "step_definition_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_logs",
                schema: "dms");

            migrationBuilder.DropTable(
                name: "code_sequences",
                schema: "dms");

            migrationBuilder.DropTable(
                name: "documents",
                schema: "dms");

            migrationBuilder.DropTable(
                name: "bills",
                schema: "dms");

            migrationBuilder.DropTable(
                name: "contracts",
                schema: "dms");

            migrationBuilder.DropTable(
                name: "document_requirements",
                schema: "dms");

            migrationBuilder.DropTable(
                name: "form_templates",
                schema: "dms");

            migrationBuilder.DropTable(
                name: "steps",
                schema: "dms");

            migrationBuilder.DropTable(
                name: "processes",
                schema: "dms");

            migrationBuilder.DropTable(
                name: "step_definitions",
                schema: "dms");

            migrationBuilder.DropTable(
                name: "companies",
                schema: "dms");

            migrationBuilder.DropTable(
                name: "process_types",
                schema: "dms");
        }
    }
}
