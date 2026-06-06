using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Folha360.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "public");

            migrationBuilder.CreateTable(
                name: "audit_log",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    schema_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    table_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    old_data = table.Column<string>(type: "jsonb", nullable: true),
                    new_data = table.Column<string>(type: "jsonb", nullable: true),
                    changed_by = table.Column<Guid>(type: "uuid", nullable: false),
                    changed_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "cargo",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cbo = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    salario_base_minimo = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    salario_base_maximo = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cargo", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "convenio",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    operadora = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    valor_mensal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    percentual_empresa = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    percentual_funcionario = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_convenio", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "dependente",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cpf = table.Column<string>(type: "text", nullable: false),
                    data_nascimento = table.Column<DateOnly>(type: "date", nullable: false),
                    tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    grau_parentesco = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    dependente_irrf = table.Column<bool>(type: "boolean", nullable: false),
                    dependente_salario_familia = table.Column<bool>(type: "boolean", nullable: false),
                    pensao_alimenticia_valor = table.Column<decimal>(type: "numeric(18,2)", nullable: true),
                    pensao_alimenticia_percentual = table.Column<decimal>(type: "numeric(5,2)", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dependente", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "documento",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    funcionario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    numero = table.Column<string>(type: "text", nullable: false),
                    data_emissao = table.Column<DateOnly>(type: "date", nullable: true),
                    data_validade = table.Column<DateOnly>(type: "date", nullable: true),
                    orgao_emissor = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    uf_emissor = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    arquivo_path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_documento", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "empresa",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: false),
                    razao_social = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    nome_fantasia = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cnae = table.Column<string>(type: "character varying(7)", maxLength: 7, nullable: true),
                    regime_tributario = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    fpas = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    codigo_terceiros = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    classificacao_tributaria = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    matriz_filial = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    cnpj_matriz = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    endereco_logradouro = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    endereco_numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    endereco_complemento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    endereco_bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    endereco_cep = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    endereco_municipio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    endereco_uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_empresa", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "funcionario",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cpf = table.Column<string>(type: "text", nullable: false),
                    cpf_hash = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    data_nascimento = table.Column<DateOnly>(type: "date", nullable: true),
                    sexo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    estado_civil = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    nacionalidade = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    nome_mae = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    nome_pai = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    endereco_logradouro = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    endereco_numero = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    endereco_complemento = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    endereco_bairro = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    endereco_cep = table.Column<string>(type: "character varying(8)", maxLength: 8, nullable: true),
                    endereco_municipio = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    endereco_uf = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    telefone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    data_admissao = table.Column<DateOnly>(type: "date", nullable: false),
                    data_desligamento = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Ativo"),
                    cargo_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lotacao_id = table.Column<Guid>(type: "uuid", nullable: false),
                    salario_base = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    tipo_contrato = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    jornada_horas_semanais = table.Column<int>(type: "integer", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_funcionario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "horario_trabalho",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tipo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    carga_horaria_diaria = table.Column<int>(type: "integer", nullable: false),
                    carga_horaria_semanal = table.Column<int>(type: "integer", nullable: false),
                    inicio_jornada = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    fim_jornada = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    inicio_intervalo = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    fim_intervalo = table.Column<TimeOnly>(type: "time without time zone", nullable: true),
                    tolerancia_atraso_minutos = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_horario_trabalho", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "lotacao",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tipo_esocial = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_lotacao", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "rubrica",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    descricao = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    natureza = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    tipo_esocial = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    incide_inss = table.Column<bool>(type: "boolean", nullable: false),
                    incide_irrf = table.Column<bool>(type: "boolean", nullable: false),
                    incide_fgts = table.Column<bool>(type: "boolean", nullable: false),
                    incide_contribuicao_sindical = table.Column<bool>(type: "boolean", nullable: false),
                    incide_decimo_terceiro = table.Column<bool>(type: "boolean", nullable: false),
                    incide_ferias = table.Column<bool>(type: "boolean", nullable: false),
                    incide_aviso_previo = table.Column<bool>(type: "boolean", nullable: false),
                    formula_calculo = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ordem_exibicao = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rubrica", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sindicato",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    empresa_id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    nome = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    cnpj = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    tipo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    contribuicao_sindical_percentual = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    contribuicao_assistencial_percentual = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sindicato", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "tenant",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    schema_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                schema: "public",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    senha_hash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    nome = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    perfil = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    ultimo_login = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usuario", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_changed_at",
                schema: "public",
                table: "audit_log",
                column: "changed_at");

            migrationBuilder.CreateIndex(
                name: "ix_audit_log_table_name_record_id",
                schema: "public",
                table: "audit_log",
                columns: new[] { "table_name", "record_id" });

            migrationBuilder.CreateIndex(
                name: "ix_cargo_cbo",
                schema: "public",
                table: "cargo",
                column: "cbo");

            migrationBuilder.CreateIndex(
                name: "ix_cargo_empresa_id",
                schema: "public",
                table: "cargo",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "ix_convenio_empresa_id",
                schema: "public",
                table: "convenio",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "ix_dependente_funcionario_id",
                schema: "public",
                table: "dependente",
                column: "funcionario_id");

            migrationBuilder.CreateIndex(
                name: "ix_documento_funcionario_id_tipo",
                schema: "public",
                table: "documento",
                columns: new[] { "funcionario_id", "tipo" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_empresa_cnpj",
                schema: "public",
                table: "empresa",
                column: "cnpj",
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_empresa_razao_social",
                schema: "public",
                table: "empresa",
                column: "razao_social");

            migrationBuilder.CreateIndex(
                name: "ix_empresa_tenant_id",
                schema: "public",
                table: "empresa",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_funcionario_cargo_id",
                schema: "public",
                table: "funcionario",
                column: "cargo_id");

            migrationBuilder.CreateIndex(
                name: "ix_funcionario_cpf_hash",
                schema: "public",
                table: "funcionario",
                column: "cpf_hash",
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_funcionario_empresa_id",
                schema: "public",
                table: "funcionario",
                column: "empresa_id");

            migrationBuilder.CreateIndex(
                name: "ix_funcionario_lotacao_id",
                schema: "public",
                table: "funcionario",
                column: "lotacao_id");

            migrationBuilder.CreateIndex(
                name: "ix_funcionario_nome",
                schema: "public",
                table: "funcionario",
                column: "nome")
                .Annotation("Npgsql:IndexMethod", "gin")
                .Annotation("Npgsql:IndexOperators", new[] { "gin_trgm_ops" });

            migrationBuilder.CreateIndex(
                name: "ix_funcionario_status",
                schema: "public",
                table: "funcionario",
                column: "status");

            migrationBuilder.CreateIndex(
                name: "ix_horario_trabalho_empresa_id_codigo",
                schema: "public",
                table: "horario_trabalho",
                columns: new[] { "empresa_id", "codigo" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_lotacao_empresa_id_codigo",
                schema: "public",
                table: "lotacao",
                columns: new[] { "empresa_id", "codigo" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_rubrica_empresa_id_codigo",
                schema: "public",
                table: "rubrica",
                columns: new[] { "empresa_id", "codigo" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_rubrica_natureza",
                schema: "public",
                table: "rubrica",
                column: "natureza");

            migrationBuilder.CreateIndex(
                name: "ix_rubrica_tipo_esocial",
                schema: "public",
                table: "rubrica",
                column: "tipo_esocial");

            migrationBuilder.CreateIndex(
                name: "ix_sindicato_empresa_id_codigo",
                schema: "public",
                table: "sindicato",
                columns: new[] { "empresa_id", "codigo" },
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_tenant_schema_name",
                schema: "public",
                table: "tenant",
                column: "schema_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tenant_tenant_id",
                schema: "public",
                table: "tenant",
                column: "tenant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usuario_email",
                schema: "public",
                table: "usuario",
                column: "email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "audit_log",
                schema: "public");

            migrationBuilder.DropTable(
                name: "cargo",
                schema: "public");

            migrationBuilder.DropTable(
                name: "convenio",
                schema: "public");

            migrationBuilder.DropTable(
                name: "dependente",
                schema: "public");

            migrationBuilder.DropTable(
                name: "documento",
                schema: "public");

            migrationBuilder.DropTable(
                name: "empresa",
                schema: "public");

            migrationBuilder.DropTable(
                name: "funcionario",
                schema: "public");

            migrationBuilder.DropTable(
                name: "horario_trabalho",
                schema: "public");

            migrationBuilder.DropTable(
                name: "lotacao",
                schema: "public");

            migrationBuilder.DropTable(
                name: "rubrica",
                schema: "public");

            migrationBuilder.DropTable(
                name: "sindicato",
                schema: "public");

            migrationBuilder.DropTable(
                name: "tenant",
                schema: "public");

            migrationBuilder.DropTable(
                name: "usuario",
                schema: "public");
        }
    }
}
