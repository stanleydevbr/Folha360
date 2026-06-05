using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using Folha360.Domain.Abstractions;
using Folha360.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Folha360.Cadastros.Infrastructure.Data;

/// <summary>
/// DbContext do módulo de Cadastros (F02).
/// Herda de Folha360DbContext e adiciona os 10 DbSets das entidades de cadastro.
/// Aplica query filter global de soft delete e criptografia de campos sensíveis.
/// </summary>
public class CadastrosDbContext : Folha360DbContext
{
    public CadastrosDbContext(DbContextOptions<Folha360DbContext> options)
        : base(options)
    {
    }

    // Schema public
    public DbSet<Empresa> Empresas => Set<Empresa>();

    // Schema tenant
    public DbSet<Funcionario> Funcionarios => Set<Funcionario>();
    public DbSet<Cargo> Cargos => Set<Cargo>();
    public DbSet<Rubrica> Rubricas => Set<Rubrica>();
    public DbSet<Lotacao> Lotacoes => Set<Lotacao>();
    public DbSet<Dependente> Dependentes => Set<Dependente>();
    public DbSet<Documento> Documentos => Set<Documento>();
    public DbSet<Sindicato> Sindicatos => Set<Sindicato>();
    public DbSet<Convenio> Convenios => Set<Convenio>();
    public DbSet<HorarioTrabalho> HorariosTrabalho => Set<HorarioTrabalho>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ============================
        // Schema: public
        // ============================
        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.ToTable("empresa", "public");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Cnpj).IsRequired().HasMaxLength(14);
            entity.Property(e => e.RazaoSocial).IsRequired().HasMaxLength(200);
            entity.Property(e => e.NomeFantasia).HasMaxLength(200);
            entity.Property(e => e.Cnae).HasMaxLength(7);
            entity.Property(e => e.RegimeTributario).IsRequired().HasMaxLength(30);
            entity.Property(e => e.Fpas).HasMaxLength(10);
            entity.Property(e => e.CodigoTerceiros).HasMaxLength(50);
            entity.Property(e => e.ClassificacaoTributaria).HasMaxLength(30);
            entity.Property(e => e.MatrizFilial).HasMaxLength(10);
            entity.Property(e => e.CnpjMatriz).HasMaxLength(14);
            entity.Property(e => e.EnderecoLogradouro).HasMaxLength(200);
            entity.Property(e => e.EnderecoNumero).HasMaxLength(20);
            entity.Property(e => e.EnderecoComplemento).HasMaxLength(100);
            entity.Property(e => e.EnderecoBairro).HasMaxLength(100);
            entity.Property(e => e.EnderecoCep).HasMaxLength(8);
            entity.Property(e => e.EnderecoMunicipio).HasMaxLength(100);
            entity.Property(e => e.EnderecoUf).HasMaxLength(2);
            entity.Property(e => e.Telefone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.DeletedAt);

            // Índices P1
            entity.HasIndex(e => e.Cnpj).IsUnique().HasFilter("deleted_at IS NULL");
            entity.HasIndex(e => e.TenantId);
            entity.HasIndex(e => e.RazaoSocial);
        });

        // ============================
        // Schema: tenant (template_tenant / tenant_XXX)
        // ============================
        modelBuilder.Entity<Funcionario>(entity =>
        {
            entity.ToTable("funcionario");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Cpf).IsRequired().HasConversion<EncryptionConverter>();
            entity.Property(e => e.CpfHash).IsRequired().HasMaxLength(64);
            entity.Property(e => e.DataAdmissao).IsRequired();
            entity.Property(e => e.Status).IsRequired().HasMaxLength(20).HasDefaultValue("Ativo");
            entity.Property(e => e.SalarioBase).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.Sexo).HasMaxLength(20);
            entity.Property(e => e.EstadoCivil).HasMaxLength(20);
            entity.Property(e => e.Nacionalidade).HasMaxLength(50);
            entity.Property(e => e.NomeMae).HasMaxLength(200);
            entity.Property(e => e.NomePai).HasMaxLength(200);
            entity.Property(e => e.EnderecoLogradouro).HasMaxLength(200);
            entity.Property(e => e.EnderecoNumero).HasMaxLength(20);
            entity.Property(e => e.EnderecoComplemento).HasMaxLength(100);
            entity.Property(e => e.EnderecoBairro).HasMaxLength(100);
            entity.Property(e => e.EnderecoCep).HasMaxLength(8);
            entity.Property(e => e.EnderecoMunicipio).HasMaxLength(100);
            entity.Property(e => e.EnderecoUf).HasMaxLength(2);
            entity.Property(e => e.Telefone).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.TipoContrato).HasMaxLength(30);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.DeletedAt);

            // Índices P1
            entity.HasIndex(e => e.CpfHash).IsUnique().HasFilter("deleted_at IS NULL");
            entity.HasIndex(e => e.EmpresaId);
            entity.HasIndex(e => e.CargoId);
            entity.HasIndex(e => e.LotacaoId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Nome).HasMethod("gin").HasOperators("gin_trgm_ops");
        });

        modelBuilder.Entity<Cargo>(entity =>
        {
            entity.ToTable("cargo");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Cbo).IsRequired().HasMaxLength(6);
            entity.Property(e => e.Descricao).HasMaxLength(500);
            entity.Property(e => e.SalarioBaseMinimo).HasColumnType("decimal(18,2)");
            entity.Property(e => e.SalarioBaseMaximo).HasColumnType("decimal(18,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.DeletedAt);

            // Índices P1
            entity.HasIndex(e => e.EmpresaId);
            entity.HasIndex(e => e.Cbo);
        });

        modelBuilder.Entity<Rubrica>(entity =>
        {
            entity.ToTable("rubrica");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Natureza).IsRequired().HasMaxLength(20);
            entity.Property(e => e.TipoEsocial).HasMaxLength(10);
            entity.Property(e => e.FormulaCalculo).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.DeletedAt);

            // Índices P1
            entity.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique().HasFilter("deleted_at IS NULL");
            entity.HasIndex(e => e.Natureza);
            entity.HasIndex(e => e.TipoEsocial);
        });

        modelBuilder.Entity<Lotacao>(entity =>
        {
            entity.ToTable("lotacao");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TipoEsocial).HasMaxLength(10);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.DeletedAt);

            // Índices P1
            entity.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique().HasFilter("deleted_at IS NULL");
        });

        modelBuilder.Entity<Dependente>(entity =>
        {
            entity.ToTable("dependente");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Cpf).IsRequired().HasConversion<EncryptionConverter>();
            entity.Property(e => e.DataNascimento).IsRequired();
            entity.Property(e => e.Tipo).IsRequired().HasMaxLength(30);
            entity.Property(e => e.GrauParentesco).HasMaxLength(50);
            entity.Property(e => e.PensaoAlimenticiaValor).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PensaoAlimenticiaPercentual).HasColumnType("decimal(5,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.DeletedAt);

            // Índices P1
            entity.HasIndex(e => e.FuncionarioId);
        });

        modelBuilder.Entity<Documento>(entity =>
        {
            entity.ToTable("documento");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Tipo).IsRequired().HasMaxLength(30);
            entity.Property(e => e.Numero).IsRequired().HasConversion<EncryptionConverter>();
            entity.Property(e => e.OrgaoEmissor).HasMaxLength(50);
            entity.Property(e => e.UfEmissor).HasMaxLength(2);
            entity.Property(e => e.ArquivoPath).HasMaxLength(500);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.DeletedAt);

            // Índices P1 + UNIQUE constraint
            entity.HasIndex(e => new { e.FuncionarioId, e.Tipo }).IsUnique().HasFilter("deleted_at IS NULL");
        });

        modelBuilder.Entity<Sindicato>(entity =>
        {
            entity.ToTable("sindicato");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Cnpj).HasMaxLength(14);
            entity.Property(e => e.Tipo).HasMaxLength(30);
            entity.Property(e => e.ContribuicaoSindicalPercentual).HasColumnType("decimal(5,2)");
            entity.Property(e => e.ContribuicaoAssistencialPercentual).HasColumnType("decimal(5,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.DeletedAt);

            // Índices P1
            entity.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique().HasFilter("deleted_at IS NULL");
        });

        modelBuilder.Entity<Convenio>(entity =>
        {
            entity.ToTable("convenio");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Tipo).IsRequired().HasMaxLength(30);
            entity.Property(e => e.Operadora).HasMaxLength(100);
            entity.Property(e => e.ValorMensal).HasColumnType("decimal(18,2)");
            entity.Property(e => e.PercentualEmpresa).HasColumnType("decimal(5,2)");
            entity.Property(e => e.PercentualFuncionario).HasColumnType("decimal(5,2)");
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.DeletedAt);

            // Índices P1
            entity.HasIndex(e => e.EmpresaId);
        });

        modelBuilder.Entity<HorarioTrabalho>(entity =>
        {
            entity.ToTable("horario_trabalho");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Codigo).IsRequired().HasMaxLength(20);
            entity.Property(e => e.Descricao).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Tipo).IsRequired().HasMaxLength(20);
            entity.Property(e => e.CargaHorariaDiaria).IsRequired();
            entity.Property(e => e.CargaHorariaSemanal).IsRequired();
            entity.Property(e => e.ToleranciaAtrasoMinutos).HasDefaultValue(0);
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.UpdatedAt).IsRequired();
            entity.Property(e => e.DeletedAt);

            // Índices P1
            entity.HasIndex(e => new { e.EmpresaId, e.Codigo }).IsUnique().HasFilter("deleted_at IS NULL");
        });

        // ============================
        // Query Filter Global: Soft Delete
        // ============================
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                var parameter = System.Linq.Expressions.Expression.Parameter(entityType.ClrType, "e");
                var property = System.Linq.Expressions.Expression.Property(parameter, nameof(ISoftDeletable.DeletedAt));
                var nullCheck = System.Linq.Expressions.Expression.Equal(property, System.Linq.Expressions.Expression.Constant(null, typeof(DateTime?)));
                var lambda = System.Linq.Expressions.Expression.Lambda(nullCheck, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }

    /// <summary>
    /// Intercepta operações DELETE e as converte em soft delete (preenche DeletedAt).
    /// </summary>
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<ISoftDeletable>()
                     .Where(e => e.State == EntityState.Deleted))
        {
            entry.State = EntityState.Modified;
            entry.Entity.DeletedAt = DateTime.UtcNow;
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        foreach (var entry in ChangeTracker.Entries<ISoftDeletable>()
                     .Where(e => e.State == EntityState.Deleted))
        {
            entry.State = EntityState.Modified;
            entry.Entity.DeletedAt = DateTime.UtcNow;
        }

        return base.SaveChanges();
    }
}
