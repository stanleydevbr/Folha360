using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Folha360.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIntegracaoEsocial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "certificado_digital",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    arquivo_pfx = table.Column<byte[]>(type: "bytea", nullable: true),
                    caminho_token = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    slot_id = table.Column<long>(type: "bigint", nullable: true),
                    emitente = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    data_expiracao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_certificado_digital", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "evento_esocial",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tipo_evento = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    xml_conteudo = table.Column<string>(type: "xml", nullable: false),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    lote_id = table.Column<Guid>(type: "uuid", nullable: true),
                    id_evento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    certificado_id = table.Column<Guid>(type: "uuid", nullable: true),
                    hash_assinatura = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                    processado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_evento_esocial", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "falha_esocial",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    evento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lote_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tipo_erro = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    codigo_erro = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    mensagem_erro = table.Column<string>(type: "text", nullable: false),
                    xml_original = table.Column<string>(type: "xml", nullable: true),
                    tentativas = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    data_ultima_tentativa = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resolvido_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_falha_esocial", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lote_esocial",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_ambiente = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    status = table.Column<string>(type: "character varying(25)", maxLength: 25, nullable: false),
                    protocolo_envio = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    recibo_governo_json = table.Column<string>(type: "jsonb", nullable: true),
                    quantidade_eventos = table.Column<int>(type: "integer", nullable: false),
                    data_envio = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_processamento = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lote_esocial", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "relatorio_agendamento",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_relatorio = table.Column<int>(type: "integer", nullable: false),
                    formato = table.Column<int>(type: "integer", nullable: false),
                    recorrencia = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    destinatarios = table.Column<string>(type: "jsonb", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    atualizado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_relatorio_agendamento", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "relatorio_arquivo",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo_relatorio = table.Column<int>(type: "integer", nullable: false),
                    periodo = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    formato = table.Column<int>(type: "integer", nullable: false),
                    bucket = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    chave = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    tamanho_bytes = table.Column<long>(type: "bigint", nullable: false),
                    criado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_relatorio_arquivo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "relatorio_execucao",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    agendamento_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    iniciado_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    concluido_em = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    link_arquivo = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    log_erros = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_relatorio_execucao", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_certificado_digital_data_expiracao",
                schema: "public",
                table: "certificado_digital",
                column: "data_expiracao");

            migrationBuilder.CreateIndex(
                name: "ix_certificado_digital_empresa_id_ativo",
                schema: "public",
                table: "certificado_digital",
                columns: new[] { "empresa_id", "ativo" });

            migrationBuilder.CreateIndex(
                name: "ix_evento_esocial_empresa_id_tipo_evento_status",
                schema: "public",
                table: "evento_esocial",
                columns: new[] { "empresa_id", "tipo_evento", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_evento_esocial_id_evento",
                schema: "public",
                table: "evento_esocial",
                column: "id_evento",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_evento_esocial_lote_id",
                schema: "public",
                table: "evento_esocial",
                column: "lote_id");

            migrationBuilder.CreateIndex(
                name: "ix_falha_esocial_evento_id",
                schema: "public",
                table: "falha_esocial",
                column: "evento_id");

            migrationBuilder.CreateIndex(
                name: "ix_falha_esocial_tipo_erro_resolvido_em",
                schema: "public",
                table: "falha_esocial",
                columns: new[] { "tipo_erro", "resolvido_em" });

            migrationBuilder.CreateIndex(
                name: "ix_lote_esocial_empresa_id_status",
                schema: "public",
                table: "lote_esocial",
                columns: new[] { "empresa_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_lote_esocial_protocolo_envio",
                schema: "public",
                table: "lote_esocial",
                column: "protocolo_envio",
                unique: true,
                filter: "protocolo_envio IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_relatorio_agendamento_empresa_id_ativo",
                schema: "public",
                table: "relatorio_agendamento",
                columns: new[] { "empresa_id", "ativo" });

            migrationBuilder.CreateIndex(
                name: "ix_relatorio_arquivo_empresa_id_periodo",
                schema: "public",
                table: "relatorio_arquivo",
                columns: new[] { "empresa_id", "periodo" });

            migrationBuilder.CreateIndex(
                name: "ix_relatorio_execucao_agendamento_id",
                schema: "public",
                table: "relatorio_execucao",
                column: "agendamento_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "certificado_digital",
                schema: "public");

            migrationBuilder.DropTable(
                name: "evento_esocial",
                schema: "public");

            migrationBuilder.DropTable(
                name: "falha_esocial",
                schema: "public");

            migrationBuilder.DropTable(
                name: "lote_esocial",
                schema: "public");

            migrationBuilder.DropTable(
                name: "relatorio_agendamento",
                schema: "public");

            migrationBuilder.DropTable(
                name: "relatorio_arquivo",
                schema: "public");

            migrationBuilder.DropTable(
                name: "relatorio_execucao",
                schema: "public");
        }
    }
}
