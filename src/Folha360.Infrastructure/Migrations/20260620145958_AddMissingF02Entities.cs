using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Folha360.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMissingF02Entities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "afastamentos_funcionario",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    data_inicio = table.Column<DateOnly>(type: "date", nullable: false),
                    data_fim_prevista = table.Column<DateOnly>(type: "date", nullable: true),
                    data_fim_efetiva = table.Column<DateOnly>(type: "date", nullable: true),
                    numero_atestado_cid = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_afastamentos_funcionario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "bancos_febraban",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bancos_febraban", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cbos",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    titulo = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cbos", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "configuracoes_bancarias_empresa",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    banco_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agencia = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    agencia_dv = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    conta = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    conta_dv = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    tipo_conta = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    chave_pix = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    finalidade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ativa = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_configuracoes_bancarias_empresa", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "configuracoes_esocial",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ambiente = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    certificado_digital_tipo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    certificado_vencimento = table.Column<DateOnly>(type: "date", nullable: true),
                    versao_layout = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    codigo_transmissor = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    grupo_esocial = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    data_inicio_obrigatoriedade = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_configuracoes_esocial", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "configuracoes_gerais",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    chave = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    valor = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_configuracoes_gerais", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contatos_empresa",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cpf = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    cargo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    email = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    celular = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    contato_principal = table.Column<bool>(type: "boolean", nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_inicio_vigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    data_fim_vigencia = table.Column<DateOnly>(type: "date", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contatos_empresa", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "contratos_trabalho",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lotacao_id = table.Column<Guid>(type: "uuid", nullable: true),
                    cargo_id = table.Column<Guid>(type: "uuid", nullable: true),
                    horario_trabalho_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sindicato_id = table.Column<Guid>(type: "uuid", nullable: true),
                    data_admissao = table.Column<DateOnly>(type: "date", nullable: false),
                    data_desligamento = table.Column<DateOnly>(type: "date", nullable: true),
                    tipo_admissao = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    tipo_contrato = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    data_termino_contrato = table.Column<DateOnly>(type: "date", nullable: true),
                    salario_base = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    tipo_salario = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    carga_horaria_semanal = table.Column<int>(type: "integer", nullable: true),
                    categoria_trabalhador = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    indicativo_admissao = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contratos_trabalho", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dados_bancarios_funcionario",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    banco_id = table.Column<Guid>(type: "uuid", nullable: false),
                    agencia = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    agencia_dv = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    conta = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    conta_dv = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    tipo_conta = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    chave_pix = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    conta_principal = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dados_bancarios_funcionario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "enderecos_empresa",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    logradouro = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    complemento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    cep = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    municipio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    estrangeiro = table.Column<bool>(type: "boolean", nullable: false),
                    pais = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_enderecos_empresa", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "infos_esocial_funcionario",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    indicador_deficiencia = table.Column<bool>(type: "boolean", nullable: false),
                    tipo_deficiencia = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    data_emissao_laudo_deficiencia = table.Column<DateOnly>(type: "date", nullable: true),
                    reservista = table.Column<bool>(type: "boolean", nullable: false),
                    primeiro_emprego = table.Column<bool>(type: "boolean", nullable: false),
                    trabalhador_aposentado = table.Column<bool>(type: "boolean", nullable: false),
                    registro_profissional = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_infos_esocial_funcionario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "movimentacoes_fixas",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rubrica_id = table.Column<Guid>(type: "uuid", nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    quantidade = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movimentacoes_fixas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "movimentacoes_mensais",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rubrica_id = table.Column<Guid>(type: "uuid", nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    mes_ano = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    quantidade = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movimentacoes_mensais", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "municipios_ibge",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo_ibge = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    codigo_uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_municipios_ibge", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "naturezas_juridicas",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_naturezas_juridicas", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "remuneracoes_beneficios",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    convenio_id = table.Column<Guid>(type: "uuid", nullable: true),
                    salario_base = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    valor_hora = table.Column<decimal>(type: "numeric(18,4)", nullable: true),
                    adicional_insalubridade = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    adicional_periculosidade = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    adicional_noturno_percentual = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    adicional_transferencia_percentual = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    vale_transporte = table.Column<bool>(type: "boolean", nullable: false),
                    vale_transporte_valor = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    vale_refeicao = table.Column<bool>(type: "boolean", nullable: false),
                    vale_refeicao_valor_diario = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    plano_saude = table.Column<bool>(type: "boolean", nullable: false),
                    plano_saude_valor = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    plano_odontologico = table.Column<bool>(type: "boolean", nullable: false),
                    plano_odontologico_valor = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    seguro_vida = table.Column<bool>(type: "boolean", nullable: false),
                    seguro_vida_valor = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    previdencia_privada = table.Column<bool>(type: "boolean", nullable: false),
                    previdencia_privada_valor = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_remuneracoes_beneficios", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_afastamentos_funcionario_data_inicio",
                schema: "public",
                table: "afastamentos_funcionario",
                column: "data_inicio");

            migrationBuilder.CreateIndex(
                name: "ix_afastamentos_funcionario_funcionario_id",
                schema: "public",
                table: "afastamentos_funcionario",
                column: "funcionario_id");

            migrationBuilder.CreateIndex(
                name: "ix_afastamentos_funcionario_tipo",
                schema: "public",
                table: "afastamentos_funcionario",
                column: "tipo");

            migrationBuilder.CreateIndex(
                name: "ix_bancos_febraban_codigo",
                schema: "public",
                table: "bancos_febraban",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cbos_codigo",
                schema: "public",
                table: "cbos",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_bancarias_empresa_banco_id",
                schema: "public",
                table: "configuracoes_bancarias_empresa",
                column: "banco_id");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_bancarias_empresa_empresa_id",
                schema: "public",
                table: "configuracoes_bancarias_empresa",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_bancarias_empresa_empresa_id_finalidade",
                schema: "public",
                table: "configuracoes_bancarias_empresa",
                columns: new[] { "empresa_id", "finalidade" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_esocial_empresa_id",
                schema: "public",
                table: "configuracoes_esocial",
                column: "empresa_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_gerais_empresa_id",
                schema: "public",
                table: "configuracoes_gerais",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "ix_configuracoes_gerais_empresa_id_chave",
                schema: "public",
                table: "configuracoes_gerais",
                columns: new[] { "empresa_id", "chave" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contatos_empresa_empresa_id",
                schema: "public",
                table: "contatos_empresa",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "ix_contratos_trabalho_cargo_id",
                schema: "public",
                table: "contratos_trabalho",
                column: "cargo_id");

            migrationBuilder.CreateIndex(
                name: "ix_contratos_trabalho_empresa_id",
                schema: "public",
                table: "contratos_trabalho",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "ix_contratos_trabalho_funcionario_id",
                schema: "public",
                table: "contratos_trabalho",
                column: "funcionario_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contratos_trabalho_horario_trabalho_id",
                schema: "public",
                table: "contratos_trabalho",
                column: "horario_trabalho_id");

            migrationBuilder.CreateIndex(
                name: "ix_contratos_trabalho_lotacao_id",
                schema: "public",
                table: "contratos_trabalho",
                column: "lotacao_id");

            migrationBuilder.CreateIndex(
                name: "ix_contratos_trabalho_sindicato_id",
                schema: "public",
                table: "contratos_trabalho",
                column: "sindicato_id");

            migrationBuilder.CreateIndex(
                name: "ix_contratos_trabalho_status",
                schema: "public",
                table: "contratos_trabalho",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_dados_bancarios_funcionario_banco_id",
                schema: "public",
                table: "dados_bancarios_funcionario",
                column: "banco_id");

            migrationBuilder.CreateIndex(
                name: "ix_dados_bancarios_funcionario_funcionario_id",
                schema: "public",
                table: "dados_bancarios_funcionario",
                column: "funcionario_id");

            migrationBuilder.CreateIndex(
                name: "ix_enderecos_empresa_empresa_id",
                schema: "public",
                table: "enderecos_empresa",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "ix_enderecos_empresa_empresa_id_tipo",
                schema: "public",
                table: "enderecos_empresa",
                columns: new[] { "empresa_id", "tipo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_enderecos_empresa_municipio_id",
                schema: "public",
                table: "enderecos_empresa",
                column: "municipio_id");

            migrationBuilder.CreateIndex(
                name: "ix_infos_esocial_funcionario_funcionario_id",
                schema: "public",
                table: "infos_esocial_funcionario",
                column: "funcionario_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_movimentacoes_fixas_funcionario_id",
                schema: "public",
                table: "movimentacoes_fixas",
                column: "funcionario_id");

            migrationBuilder.CreateIndex(
                name: "ix_movimentacoes_fixas_funcionario_id_rubrica_id",
                schema: "public",
                table: "movimentacoes_fixas",
                columns: new[] { "funcionario_id", "rubrica_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_movimentacoes_fixas_rubrica_id",
                schema: "public",
                table: "movimentacoes_fixas",
                column: "rubrica_id");

            migrationBuilder.CreateIndex(
                name: "ix_movimentacoes_mensais_funcionario_id",
                schema: "public",
                table: "movimentacoes_mensais",
                column: "funcionario_id");

            migrationBuilder.CreateIndex(
                name: "ix_movimentacoes_mensais_funcionario_id_rubrica_id_mes_ano",
                schema: "public",
                table: "movimentacoes_mensais",
                columns: new[] { "funcionario_id", "rubrica_id", "mes_ano" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_movimentacoes_mensais_rubrica_id",
                schema: "public",
                table: "movimentacoes_mensais",
                column: "rubrica_id");

            migrationBuilder.CreateIndex(
                name: "ix_municipios_ibge_codigo_ibge",
                schema: "public",
                table: "municipios_ibge",
                column: "codigo_ibge",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_municipios_ibge_nome",
                schema: "public",
                table: "municipios_ibge",
                column: "nome");

            migrationBuilder.CreateIndex(
                name: "ix_municipios_ibge_uf",
                schema: "public",
                table: "municipios_ibge",
                column: "uf");

            migrationBuilder.CreateIndex(
                name: "ix_naturezas_juridicas_codigo",
                schema: "public",
                table: "naturezas_juridicas",
                column: "codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_remuneracoes_beneficios_convenio_id",
                schema: "public",
                table: "remuneracoes_beneficios",
                column: "convenio_id");

            migrationBuilder.CreateIndex(
                name: "ix_remuneracoes_beneficios_funcionario_id",
                schema: "public",
                table: "remuneracoes_beneficios",
                column: "funcionario_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "afastamentos_funcionario",
                schema: "public");

            migrationBuilder.DropTable(
                name: "bancos_febraban",
                schema: "public");

            migrationBuilder.DropTable(
                name: "cbos",
                schema: "public");

            migrationBuilder.DropTable(
                name: "configuracoes_bancarias_empresa",
                schema: "public");

            migrationBuilder.DropTable(
                name: "configuracoes_esocial",
                schema: "public");

            migrationBuilder.DropTable(
                name: "configuracoes_gerais",
                schema: "public");

            migrationBuilder.DropTable(
                name: "contatos_empresa",
                schema: "public");

            migrationBuilder.DropTable(
                name: "contratos_trabalho",
                schema: "public");

            migrationBuilder.DropTable(
                name: "dados_bancarios_funcionario",
                schema: "public");

            migrationBuilder.DropTable(
                name: "enderecos_empresa",
                schema: "public");

            migrationBuilder.DropTable(
                name: "infos_esocial_funcionario",
                schema: "public");

            migrationBuilder.DropTable(
                name: "movimentacoes_fixas",
                schema: "public");

            migrationBuilder.DropTable(
                name: "movimentacoes_mensais",
                schema: "public");

            migrationBuilder.DropTable(
                name: "municipios_ibge",
                schema: "public");

            migrationBuilder.DropTable(
                name: "naturezas_juridicas",
                schema: "public");

            migrationBuilder.DropTable(
                name: "remuneracoes_beneficios",
                schema: "public");
        }
    }
}
