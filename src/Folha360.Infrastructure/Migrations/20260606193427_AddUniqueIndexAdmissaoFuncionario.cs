using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Folha360.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueIndexAdmissaoFuncionario : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_admissao_funcionario_id",
                schema: "public",
                table: "admissao");

            migrationBuilder.CreateIndex(
                name: "ix_admissao_funcionario_id",
                schema: "public",
                table: "admissao",
                column: "funcionario_id",
                unique: true,
                filter: "deleted_at IS NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_admissao_funcionario_id",
                schema: "public",
                table: "admissao");

            migrationBuilder.CreateIndex(
                name: "ix_admissao_funcionario_id",
                schema: "public",
                table: "admissao",
                column: "funcionario_id",
                filter: "deleted_at IS NULL");
        }
    }
}
