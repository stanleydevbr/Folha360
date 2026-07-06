using Folha360.Cadastros.Domain.Entities;

namespace Folha360.Cadastros.Domain.Abstractions;

// ============================
// Company Expansion (T12)
// ============================
public interface IConfiguracaoBancariaEmpresaRepository
{
    Task<ConfiguracaoBancariaEmpresa?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ConfiguracaoBancariaEmpresa>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct = default);
    Task AddAsync(ConfiguracaoBancariaEmpresa entity, CancellationToken ct = default);
    Task UpdateAsync(ConfiguracaoBancariaEmpresa entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IEnderecoEmpresaRepository
{
    Task<EnderecoEmpresa?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<EnderecoEmpresa>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct = default);
    Task AddAsync(EnderecoEmpresa entity, CancellationToken ct = default);
    Task UpdateAsync(EnderecoEmpresa entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IContatoEmpresaRepository
{
    Task<ContatoEmpresa?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<ContatoEmpresa>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct = default);
    Task<ContatoEmpresa?> GetContatoPrincipalAsync(Guid empresaId, CancellationToken ct = default);
    Task AddAsync(ContatoEmpresa entity, CancellationToken ct = default);
    Task UpdateAsync(ContatoEmpresa entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IConfiguracaoGeralRepository
{
    Task<ConfiguracaoGeral?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ConfiguracaoGeral?> GetByChaveAsync(Guid empresaId, string chave, CancellationToken ct = default);
    Task<IEnumerable<ConfiguracaoGeral>> ListarPorEmpresaAsync(Guid empresaId, CancellationToken ct = default);
    Task AddAsync(ConfiguracaoGeral entity, CancellationToken ct = default);
    Task UpdateAsync(ConfiguracaoGeral entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IConfiguracaoESocialRepository
{
    Task<ConfiguracaoESocial?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ConfiguracaoESocial?> GetByEmpresaAsync(Guid empresaId, CancellationToken ct = default);
    Task AddAsync(ConfiguracaoESocial entity, CancellationToken ct = default);
    Task UpdateAsync(ConfiguracaoESocial entity, CancellationToken ct = default);
}

// ============================
// Employee Expansion (T13)
// ============================
public interface IDadosBancariosFuncionarioRepository
{
    Task<DadosBancariosFuncionario?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<DadosBancariosFuncionario>> ListarPorFuncionarioAsync(Guid funcionarioId, CancellationToken ct = default);
    Task<DadosBancariosFuncionario?> GetContaPrincipalAsync(Guid funcionarioId, CancellationToken ct = default);
    Task AddAsync(DadosBancariosFuncionario entity, CancellationToken ct = default);
    Task UpdateAsync(DadosBancariosFuncionario entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IContratoTrabalhoRepository
{
    Task<ContratoTrabalho?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ContratoTrabalho?> GetByFuncionarioAsync(Guid funcionarioId, CancellationToken ct = default);
    Task AddAsync(ContratoTrabalho entity, CancellationToken ct = default);
    Task UpdateAsync(ContratoTrabalho entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IRemuneracaoBeneficioRepository
{
    Task<RemuneracaoBeneficio?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<RemuneracaoBeneficio?> GetByFuncionarioAsync(Guid funcionarioId, CancellationToken ct = default);
    Task AddAsync(RemuneracaoBeneficio entity, CancellationToken ct = default);
    Task UpdateAsync(RemuneracaoBeneficio entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IAfastamentoFuncionarioRepository
{
    Task<AfastamentoFuncionario?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<AfastamentoFuncionario>> ListarPorFuncionarioAsync(Guid funcionarioId, CancellationToken ct = default);
    Task AddAsync(AfastamentoFuncionario entity, CancellationToken ct = default);
    Task UpdateAsync(AfastamentoFuncionario entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IInfoESocialFuncionarioRepository
{
    Task<InfoESocialFuncionario?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<InfoESocialFuncionario?> GetByFuncionarioAsync(Guid funcionarioId, CancellationToken ct = default);
    Task AddAsync(InfoESocialFuncionario entity, CancellationToken ct = default);
    Task UpdateAsync(InfoESocialFuncionario entity, CancellationToken ct = default);
}

public interface IMovimentacaoFixaRepository
{
    Task<MovimentacaoFixa?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<MovimentacaoFixa>> ListarPorFuncionarioAsync(Guid funcionarioId, CancellationToken ct = default);
    Task AddAsync(MovimentacaoFixa entity, CancellationToken ct = default);
    Task UpdateAsync(MovimentacaoFixa entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}

public interface IMovimentacaoMensalRepository
{
    Task<MovimentacaoMensal?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<MovimentacaoMensal>> ListarPorFuncionarioMesAsync(Guid funcionarioId, string mesAno, CancellationToken ct = default);
    Task<MovimentacaoMensal?> GetByFuncionarioRubricaMesAsync(Guid funcionarioId, Guid rubricaId, string mesAno, CancellationToken ct = default);
    Task AddAsync(MovimentacaoMensal entity, CancellationToken ct = default);
    Task UpdateAsync(MovimentacaoMensal entity, CancellationToken ct = default);
    Task SoftDeleteAsync(Guid id, CancellationToken ct = default);
}
