using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Folha360.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRubricasSubsystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "formula_calculo",
                schema: "public",
                table: "rubrica",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ativo",
                schema: "public",
                table: "rubrica",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_fim_vigencia",
                schema: "public",
                table: "rubrica",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "data_inicio_vigencia",
                schema: "public",
                table: "rubrica",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "descricao_abreviada",
                schema: "public",
                table: "rubrica",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "enviar_esocial",
                schema: "public",
                table: "rubrica",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "grupo_rubrica_id",
                schema: "public",
                table: "rubrica",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "incide_adiantamento",
                schema: "public",
                table: "rubrica",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "incide_auxilio_doenca",
                schema: "public",
                table: "rubrica",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "incide_dissidio",
                schema: "public",
                table: "rubrica",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "incide_rescisao",
                schema: "public",
                table: "rubrica",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "incide_salario_maternidade",
                schema: "public",
                table: "rubrica",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "observacao",
                schema: "public",
                table: "rubrica",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ordem_calculo",
                schema: "public",
                table: "rubrica",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "percentual",
                schema: "public",
                table: "rubrica",
                type: "numeric(7,4)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "piso_minimo",
                schema: "public",
                table: "rubrica",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "prioridade_desconto",
                schema: "public",
                table: "rubrica",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "rubrica_base_id",
                schema: "public",
                table: "rubrica",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "teto_maximo",
                schema: "public",
                table: "rubrica",
                type: "numeric(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "tipo_calculo",
                schema: "public",
                table: "rubrica",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: string.Empty);

            migrationBuilder.AddColumn<decimal>(
                name: "valor_fixo",
                schema: "public",
                table: "rubrica",
                type: "numeric(18,4)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "grupo_rubrica",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descricao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    natureza = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ordem_exibicao = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_grupo_rubrica", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "processo_administrativo",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    numero_processo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    orgao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    data_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_fim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    observacao = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_processo_administrativo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rubrica_composicao",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rubrica_principal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rubrica_componente_id = table.Column<Guid>(type: "uuid", nullable: false),
                    operador = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    percentual_composicao = table.Column<decimal>(type: "numeric(7,4)", nullable: true),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    obrigatorio = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rubrica_composicao", x => x.id);
                    table.ForeignKey(
                        name: "fk_rubrica_composicao_rubrica_rubrica_componente_id",
                        column: x => x.rubrica_componente_id,
                        principalSchema: "public",
                        principalTable: "rubrica",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_rubrica_composicao_rubrica_rubrica_principal_id",
                        column: x => x.rubrica_principal_id,
                        principalSchema: "public",
                        principalTable: "rubrica",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "rubrica_formula",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rubrica_id = table.Column<Guid>(type: "uuid", nullable: false),
                    expressao = table.Column<string>(type: "text", nullable: false),
                    parametros = table.Column<string>(type: "jsonb", nullable: true),
                    descricao_formal = table.Column<string>(type: "text", nullable: true),
                    versao = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rubrica_formula", x => x.id);
                    table.ForeignKey(
                        name: "fk_rubrica_formula_rubrica_rubrica_id",
                        column: x => x.rubrica_id,
                        principalSchema: "public",
                        principalTable: "rubrica",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rubrica_historico",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rubrica_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dados_anteriores = table.Column<string>(type: "jsonb", nullable: true),
                    dados_novos = table.Column<string>(type: "jsonb", nullable: false),
                    motivo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    usuario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rubrica_historico", x => x.id);
                    table.ForeignKey(
                        name: "fk_rubrica_historico_rubrica_rubrica_id",
                        column: x => x.rubrica_id,
                        principalSchema: "public",
                        principalTable: "rubrica",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rubrica_incidencia",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rubrica_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_incidencia = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rubrica_incidencia", x => x.id);
                    table.ForeignKey(
                        name: "fk_rubrica_incidencia_rubrica_rubrica_id",
                        column: x => x.rubrica_id,
                        principalSchema: "public",
                        principalTable: "rubrica",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rubrica_tabela_progressiva",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rubrica_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ano_vigencia = table.Column<int>(type: "integer", nullable: false),
                    faixa_de = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    faixa_ate = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    aliquota = table.Column<decimal>(type: "numeric(7,4)", nullable: false),
                    deducao = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rubrica_tabela_progressiva", x => x.id);
                    table.ForeignKey(
                        name: "fk_rubrica_tabela_progressiva_rubrica_rubrica_id",
                        column: x => x.rubrica_id,
                        principalSchema: "public",
                        principalTable: "rubrica",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "rubrica_processo",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rubrica_id = table.Column<Guid>(type: "uuid", nullable: false),
                    processo_administrativo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rubrica_processo", x => x.id);
                    table.ForeignKey(
                        name: "fk_rubrica_processo_processo_administrativo_processo_administr",
                        column: x => x.processo_administrativo_id,
                        principalSchema: "public",
                        principalTable: "processo_administrativo",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_rubrica_processo_rubrica_rubrica_id",
                        column: x => x.rubrica_id,
                        principalSchema: "public",
                        principalTable: "rubrica",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_rubrica_grupo_rubrica_id",
                schema: "public",
                table: "rubrica",
                column: "grupo_rubrica_id");

            migrationBuilder.CreateIndex(
                name: "ix_rubrica_rubrica_base_id",
                schema: "public",
                table: "rubrica",
                column: "rubrica_base_id");

            migrationBuilder.CreateIndex(
                name: "ix_grupo_rubrica_empresa_id_codigo",
                schema: "public",
                table: "grupo_rubrica",
                columns: new[] { "empresa_id", "codigo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_processo_administrativo_empresa_id",
                schema: "public",
                table: "processo_administrativo",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "ix_rubrica_composicao_rubrica_componente_id",
                schema: "public",
                table: "rubrica_composicao",
                column: "rubrica_componente_id");

            migrationBuilder.CreateIndex(
                name: "ix_rubrica_composicao_rubrica_principal_id_rubrica_componente_",
                schema: "public",
                table: "rubrica_composicao",
                columns: new[] { "rubrica_principal_id", "rubrica_componente_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_rubrica_formula_rubrica_id",
                schema: "public",
                table: "rubrica_formula",
                column: "rubrica_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_rubrica_historico_rubrica_id_created_at",
                schema: "public",
                table: "rubrica_historico",
                columns: new[] { "rubrica_id", "created_at" },
                descending: new[] { false, true });

            migrationBuilder.CreateIndex(
                name: "ix_rubrica_incidencia_rubrica_id_tipo_incidencia",
                schema: "public",
                table: "rubrica_incidencia",
                columns: new[] { "rubrica_id", "tipo_incidencia" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_rubrica_processo_processo_administrativo_id",
                schema: "public",
                table: "rubrica_processo",
                column: "processo_administrativo_id");

            migrationBuilder.CreateIndex(
                name: "ix_rubrica_processo_rubrica_id_processo_administrativo_id",
                schema: "public",
                table: "rubrica_processo",
                columns: new[] { "rubrica_id", "processo_administrativo_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_rubrica_tabela_progressiva_rubrica_id_ano_vigencia_ordem",
                schema: "public",
                table: "rubrica_tabela_progressiva",
                columns: new[] { "rubrica_id", "ano_vigencia", "ordem" });

            migrationBuilder.AddForeignKey(
                name: "fk_rubrica_grupos_rubrica_grupo_rubrica_id",
                schema: "public",
                table: "rubrica",
                column: "grupo_rubrica_id",
                principalSchema: "public",
                principalTable: "grupo_rubrica",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_rubrica_rubrica_rubrica_base_id",
                schema: "public",
                table: "rubrica",
                column: "rubrica_base_id",
                principalSchema: "public",
                principalTable: "rubrica",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_rubrica_grupos_rubrica_grupo_rubrica_id",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropForeignKey(
                name: "fk_rubrica_rubrica_rubrica_base_id",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropTable(
                name: "grupo_rubrica",
                schema: "public");

            migrationBuilder.DropTable(
                name: "rubrica_composicao",
                schema: "public");

            migrationBuilder.DropTable(
                name: "rubrica_formula",
                schema: "public");

            migrationBuilder.DropTable(
                name: "rubrica_historico",
                schema: "public");

            migrationBuilder.DropTable(
                name: "rubrica_incidencia",
                schema: "public");

            migrationBuilder.DropTable(
                name: "rubrica_processo",
                schema: "public");

            migrationBuilder.DropTable(
                name: "rubrica_tabela_progressiva",
                schema: "public");

            migrationBuilder.DropTable(
                name: "processo_administrativo",
                schema: "public");

            migrationBuilder.DropIndex(
                name: "ix_rubrica_grupo_rubrica_id",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropIndex(
                name: "ix_rubrica_rubrica_base_id",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "ativo",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "data_fim_vigencia",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "data_inicio_vigencia",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "descricao_abreviada",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "enviar_esocial",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "grupo_rubrica_id",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "incide_adiantamento",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "incide_auxilio_doenca",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "incide_dissidio",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "incide_rescisao",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "incide_salario_maternidade",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "observacao",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "ordem_calculo",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "percentual",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "piso_minimo",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "prioridade_desconto",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "rubrica_base_id",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "teto_maximo",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "tipo_calculo",
                schema: "public",
                table: "rubrica");

            migrationBuilder.DropColumn(
                name: "valor_fixo",
                schema: "public",
                table: "rubrica");

            migrationBuilder.AlterColumn<string>(
                name: "formula_calculo",
                schema: "public",
                table: "rubrica",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }
    }
}
