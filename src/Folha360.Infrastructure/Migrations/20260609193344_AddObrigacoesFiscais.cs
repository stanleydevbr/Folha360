using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Folha360.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddObrigacoesFiscais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "apuracao_fiscal",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    periodo = table.Column<DateOnly>(type: "date", nullable: false),
                    processamento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tributo = table.Column<int>(type: "integer", nullable: false),
                    base_calculo = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    aliquota = table.Column<decimal>(type: "numeric(7,4)", nullable: false),
                    valor_devido = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor_pago = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    data_vencimento = table.Column<DateOnly>(type: "date", nullable: false),
                    regra_fiscal_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_apuracao_fiscal", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cadeia_fechamento",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    periodo = table.Column<DateOnly>(type: "date", nullable: false),
                    processamento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    etapa = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    versao = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    historico_versoes = table.Column<string>(type: "jsonb", nullable: false, defaultValue: "[]"),
                    data_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_fim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    erro = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cadeia_fechamento", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "guia_recolhimento",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    periodo = table.Column<DateOnly>(type: "date", nullable: false),
                    apuracao_fiscal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_guia = table.Column<int>(type: "integer", nullable: false),
                    tributo = table.Column<int>(type: "integer", nullable: false),
                    codigo_receita = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    data_vencimento = table.Column<DateOnly>(type: "date", nullable: false),
                    minio_key = table.Column<string>(type: "text", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    data_pagamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    valor_pago = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    comprovante_minio_key = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_guia_recolhimento", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "holerite",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    processamento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    minio_key = table.Column<string>(type: "text", nullable: false),
                    data_geracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_holerite", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "item_folha",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    processamento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rubrica_id = table.Column<Guid>(type: "uuid", nullable: false),
                    fase = table.Column<int>(type: "integer", nullable: false),
                    base_calculo = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    formula_aplicada = table.Column<string>(type: "text", nullable: true),
                    ordem = table.Column<int>(type: "integer", nullable: false),
                    data_calculo = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_item_folha", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lancamento_contabil",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    periodo = table.Column<DateOnly>(type: "date", nullable: false),
                    apuracao_fiscal_id = table.Column<Guid>(type: "uuid", nullable: false),
                    data = table.Column<DateOnly>(type: "date", nullable: false),
                    conta_debito = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    conta_credito = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    historico = table.Column<string>(type: "text", nullable: false),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    tributo = table.Column<int>(type: "integer", nullable: false),
                    formato = table.Column<int>(type: "integer", nullable: false),
                    minio_key = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lancamento_contabil", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "processamento_folha",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    periodo = table.Column<DateOnly>(type: "date", nullable: false),
                    tipo_calculo = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    versao = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    processamento_original_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reaberto_por = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    reaberto_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    motivo_reabertura = table.Column<string>(type: "text", nullable: true),
                    total_funcionarios = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    funcionarios_processados = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    funcionarios_com_erro = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    total_vencimentos = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    total_descontos = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    total_liquido = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    total_fgts = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 0m),
                    data_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_fim = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    erro = table.Column<string>(type: "text", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_processamento_folha", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "regra_fiscal",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tributo = table.Column<int>(type: "integer", nullable: false),
                    versao = table.Column<int>(type: "integer", nullable: false),
                    vigencia_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    vigencia_fim = table.Column<DateOnly>(type: "date", nullable: true),
                    parametros = table.Column<string>(type: "jsonb", nullable: false),
                    codigo_receita = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_regra_fiscal", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_apuracao_fiscal_empresa_id_periodo",
                schema: "public",
                table: "apuracao_fiscal",
                columns: new[] { "empresa_id", "periodo" });

            migrationBuilder.CreateIndex(
                name: "ix_apuracao_fiscal_empresa_id_periodo_tributo_processamento_id",
                schema: "public",
                table: "apuracao_fiscal",
                columns: new[] { "empresa_id", "periodo", "tributo", "processamento_id" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_apuracao_fiscal_processamento_id",
                schema: "public",
                table: "apuracao_fiscal",
                column: "processamento_id");

            migrationBuilder.CreateIndex(
                name: "ix_cadeia_fechamento_empresa_id_periodo",
                schema: "public",
                table: "cadeia_fechamento",
                columns: new[] { "empresa_id", "periodo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cadeia_fechamento_status",
                schema: "public",
                table: "cadeia_fechamento",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_guia_recolhimento_apuracao_fiscal_id",
                schema: "public",
                table: "guia_recolhimento",
                column: "apuracao_fiscal_id");

            migrationBuilder.CreateIndex(
                name: "ix_guia_recolhimento_data_vencimento",
                schema: "public",
                table: "guia_recolhimento",
                column: "data_vencimento");

            migrationBuilder.CreateIndex(
                name: "ix_guia_recolhimento_empresa_id_periodo",
                schema: "public",
                table: "guia_recolhimento",
                columns: new[] { "empresa_id", "periodo" });

            migrationBuilder.CreateIndex(
                name: "ix_guia_recolhimento_status",
                schema: "public",
                table: "guia_recolhimento",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_holerite_funcionario_id",
                schema: "public",
                table: "holerite",
                column: "funcionario_id");

            migrationBuilder.CreateIndex(
                name: "ix_holerite_processamento_id",
                schema: "public",
                table: "holerite",
                column: "processamento_id");

            migrationBuilder.CreateIndex(
                name: "ix_item_folha_processamento_id_funcionario_id",
                schema: "public",
                table: "item_folha",
                columns: new[] { "processamento_id", "funcionario_id" });

            migrationBuilder.CreateIndex(
                name: "ix_item_folha_rubrica_id",
                schema: "public",
                table: "item_folha",
                column: "rubrica_id");

            migrationBuilder.CreateIndex(
                name: "ix_lancamento_contabil_apuracao_fiscal_id",
                schema: "public",
                table: "lancamento_contabil",
                column: "apuracao_fiscal_id");

            migrationBuilder.CreateIndex(
                name: "ix_lancamento_contabil_empresa_id_periodo",
                schema: "public",
                table: "lancamento_contabil",
                columns: new[] { "empresa_id", "periodo" });

            migrationBuilder.CreateIndex(
                name: "ix_processamento_folha_empresa_id_periodo",
                schema: "public",
                table: "processamento_folha",
                columns: new[] { "empresa_id", "periodo" });

            migrationBuilder.CreateIndex(
                name: "ix_processamento_folha_empresa_id_periodo_tipo_calculo_versao",
                schema: "public",
                table: "processamento_folha",
                columns: new[] { "empresa_id", "periodo", "tipo_calculo", "versao" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_processamento_folha_status",
                schema: "public",
                table: "processamento_folha",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_regra_fiscal_tributo_versao",
                schema: "public",
                table: "regra_fiscal",
                columns: new[] { "tributo", "versao" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_regra_fiscal_tributo_vigencia_inicio_vigencia_fim",
                schema: "public",
                table: "regra_fiscal",
                columns: new[] { "tributo", "vigencia_inicio", "vigencia_fim" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "apuracao_fiscal",
                schema: "public");

            migrationBuilder.DropTable(
                name: "cadeia_fechamento",
                schema: "public");

            migrationBuilder.DropTable(
                name: "guia_recolhimento",
                schema: "public");

            migrationBuilder.DropTable(
                name: "holerite",
                schema: "public");

            migrationBuilder.DropTable(
                name: "item_folha",
                schema: "public");

            migrationBuilder.DropTable(
                name: "lancamento_contabil",
                schema: "public");

            migrationBuilder.DropTable(
                name: "processamento_folha",
                schema: "public");

            migrationBuilder.DropTable(
                name: "regra_fiscal",
                schema: "public");
        }
    }
}
