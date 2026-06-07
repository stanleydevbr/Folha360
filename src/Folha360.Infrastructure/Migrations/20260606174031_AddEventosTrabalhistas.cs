using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Folha360.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEventosTrabalhistas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admissao",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_admissao = table.Column<DateOnly>(type: "date", nullable: false),
                    cargo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    salario_inicial = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    tipo_contrato = table.Column<int>(type: "integer", nullable: false),
                    periodo_experiencia_meses = table.Column<int>(type: "integer", nullable: true),
                    xml_content = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_admissao", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "afastamento",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    data_fim_prevista = table.Column<DateOnly>(type: "date", nullable: false),
                    data_fim_efetiva = table.Column<DateOnly>(type: "date", nullable: true),
                    tipo_afastamento = table.Column<int>(type: "integer", nullable: false),
                    cid = table.Column<string>(type: "text", nullable: true),
                    xml_content = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_afastamento", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "alteracao_contratual",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_alteracao = table.Column<DateOnly>(type: "date", nullable: false),
                    campos_alterados = table.Column<string>(type: "jsonb", nullable: true),
                    valor_anterior = table.Column<string>(type: "jsonb", nullable: true),
                    valor_novo = table.Column<string>(type: "jsonb", nullable: true),
                    xml_content = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_alteracao_contratual", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "desligamento",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_desligamento = table.Column<DateOnly>(type: "date", nullable: false),
                    motivo_desligamento = table.Column<int>(type: "integer", nullable: false),
                    verbas_rescisorias = table.Column<string>(type: "jsonb", nullable: true),
                    xml_content = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_desligamento", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "ferias",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    dias_gozo = table.Column<int>(type: "integer", nullable: false),
                    periodo_aquisitivo_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    periodo_aquisitivo_fim = table.Column<DateOnly>(type: "date", nullable: false),
                    tipo_ferias = table.Column<int>(type: "integer", nullable: false),
                    xml_content = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ferias", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_admissao_data_admissao",
                schema: "public",
                table: "admissao",
                column: "data_admissao");

            migrationBuilder.CreateIndex(
                name: "ix_admissao_empresa_id",
                schema: "public",
                table: "admissao",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "ix_admissao_funcionario_id",
                schema: "public",
                table: "admissao",
                column: "funcionario_id",
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_afastamento_empresa_id",
                schema: "public",
                table: "afastamento",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "ix_afastamento_funcionario_id",
                schema: "public",
                table: "afastamento",
                column: "funcionario_id",
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_alteracao_contratual_empresa_id",
                schema: "public",
                table: "alteracao_contratual",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "ix_alteracao_contratual_funcionario_id",
                schema: "public",
                table: "alteracao_contratual",
                column: "funcionario_id",
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_desligamento_empresa_id",
                schema: "public",
                table: "desligamento",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "ix_desligamento_funcionario_id",
                schema: "public",
                table: "desligamento",
                column: "funcionario_id",
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_ferias_empresa_id",
                schema: "public",
                table: "ferias",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "ix_ferias_funcionario_id",
                schema: "public",
                table: "ferias",
                column: "funcionario_id",
                filter: "deleted_at IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admissao",
                schema: "public");

            migrationBuilder.DropTable(
                name: "afastamento",
                schema: "public");

            migrationBuilder.DropTable(
                name: "alteracao_contratual",
                schema: "public");

            migrationBuilder.DropTable(
                name: "desligamento",
                schema: "public");

            migrationBuilder.DropTable(
                name: "ferias",
                schema: "public");
        }
    }
}
