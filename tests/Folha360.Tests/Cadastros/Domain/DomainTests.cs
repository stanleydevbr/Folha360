using Folha360.Cadastros.Domain.Entities;
using Folha360.Cadastros.Domain.Events;
using Folha360.Cadastros.Domain.ValueObjects;

namespace Folha360.Tests.Cadastros.Domain;

public class ValueObjectsTests
{
    [Fact]
    public void Cnpj_Valido_DeveCriar()
    {
        var cnpj = new Cnpj("11222333000181");
        Assert.Equal("11222333000181", cnpj.Numero);
        Assert.Equal("11.222.333/0001-81", cnpj.Formatado);
    }

    [Fact]
    public void Cnpj_Invalido_DigitosErrados_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() => new Cnpj("11222333000182"));
    }

    [Fact]
    public void Cnpj_TamanhoInvalido_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() => new Cnpj("123"));
    }

    [Fact]
    public void Cnpj_TodosDigitosIguais_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() => new Cnpj("11111111111111"));
    }

    [Fact]
    public void Cnpj_ComFormatacao_DeveRemoverCaracteres()
    {
        var cnpj = new Cnpj("11.222.333/0001-81");
        Assert.Equal("11222333000181", cnpj.Numero);
    }

    [Fact]
    public void Cpf_Valido_DeveCriar()
    {
        var cpf = new Cpf("52998224725");
        Assert.Equal("52998224725", cpf.Numero);
        Assert.Equal("529.982.247-25", cpf.Formatado);
    }

    [Fact]
    public void Cpf_Invalido_DigitosErrados_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() => new Cpf("52998224726"));
    }

    [Fact]
    public void Cpf_TodosDigitosIguais_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() => new Cpf("11111111111"));
    }

    [Fact]
    public void Cpf_ComputeHash_DeveSerDeterministico()
    {
        var cpf = new Cpf("52998224725");
        var hash1 = cpf.ComputeHash();
        var hash2 = cpf.ComputeHash();
        Assert.Equal(hash1, hash2);
        Assert.Equal(64, hash1.Length); // SHA-256 hex = 64 chars
    }

    [Fact]
    public void Cbo_Valido_DeveCriar()
    {
        var cbo = new Cbo("123456");
        Assert.Equal("123456", cbo.Codigo);
    }

    [Fact]
    public void Cbo_MenosDe6Digitos_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() => new Cbo("12345"));
    }

    [Fact]
    public void Cbo_MaisDe6Digitos_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() => new Cbo("1234567"));
    }

    [Fact]
    public void Endereco_Valido_DeveCriar()
    {
        var endereco = new Endereco("Rua A", "100", "Centro", "01001000", "São Paulo", "SP", "Apto 1");
        Assert.Equal("Rua A", endereco.Logradouro);
        Assert.Equal("100", endereco.Numero);
        Assert.Equal("SP", endereco.Uf);
    }

    [Fact]
    public void Endereco_UfInvalida_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            new Endereco("Rua A", "100", "Centro", "01001000", "São Paulo", "S"));
    }

    [Fact]
    public void Endereco_SemComplemento_DeveCriar()
    {
        var endereco = new Endereco("Rua A", "100", "Centro", "01001000", "São Paulo", "SP");
        Assert.Null(endereco.Complemento);
    }
}

public class EntidadesTests
{
    [Fact]
    public void Empresa_DeveCriarComSucesso()
    {
        var empresa = new Empresa(
            Guid.NewGuid(), "11222333000181", "Empresa Teste", "Lucro Real");

        Assert.NotEqual(Guid.Empty, empresa.Id);
        Assert.Equal("11222333000181", empresa.Cnpj);
        Assert.Equal("Empresa Teste", empresa.RazaoSocial);
        Assert.Equal("Lucro Real", empresa.RegimeTributario);
        Assert.Null(empresa.DeletedAt);
    }

    [Fact]
    public void Funcionario_DeveCriarComSucesso()
    {
        var funcionario = new Funcionario(
            Guid.NewGuid(), "João Silva", "52998224725",
            "abc123hash", new DateOnly(2024, 1, 15),
            Guid.NewGuid(), Guid.NewGuid(), 5000m);

        Assert.Equal("João Silva", funcionario.Nome);
        Assert.Equal("Ativo", funcionario.Status);
        Assert.Equal(5000m, funcionario.SalarioBase);
    }

    [Fact]
    public void Funcionario_Desligar_DeveAtualizarStatus()
    {
        var funcionario = new Funcionario(
            Guid.NewGuid(), "João Silva", "52998224725",
            "abc123hash", new DateOnly(2024, 1, 15),
            Guid.NewGuid(), Guid.NewGuid(), 5000m);

        funcionario.Desligar(new DateOnly(2025, 6, 1));

        Assert.Equal("Desligado", funcionario.Status);
        Assert.Equal(new DateOnly(2025, 6, 1), funcionario.DataDesligamento);
    }

    [Fact]
    public void Cargo_SalarioMinimoMaiorQueMaximo_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            new Cargo(Guid.NewGuid(), "Dev", "123456", salarioBaseMinimo: 5000, salarioBaseMaximo: 3000));
    }

    [Fact]
    public void Cargo_DeveCriarComSucesso()
    {
        var cargo = new Cargo(Guid.NewGuid(), "Desenvolvedor", "123456",
            salarioBaseMinimo: 3000, salarioBaseMaximo: 8000);

        Assert.Equal("Desenvolvedor", cargo.Nome);
        Assert.Equal("123456", cargo.Cbo);
    }

    [Fact]
    public void Rubrica_DeveCriarComSucesso()
    {
        var rubrica = new Rubrica(
            Guid.NewGuid(), "001", "Salário Base", "Vencimento",
            incideInss: true, incideFgts: true);

        Assert.Equal("001", rubrica.Codigo);
        Assert.Equal("Vencimento", rubrica.Natureza);
        Assert.True(rubrica.IncideInss);
    }

    [Fact]
    public void Lotacao_DeveCriarComSucesso()
    {
        var lotacao = new Lotacao(Guid.NewGuid(), "ADM", "Administrativo");

        Assert.Equal("ADM", lotacao.Codigo);
        Assert.Equal("Administrativo", lotacao.Descricao);
    }

    [Fact]
    public void Dependente_SalarioFamilia_IdadeMaior14_DeveLancarExcecao()
    {
        var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-15));

        Assert.Throws<ArgumentException>(() =>
            new Dependente(Guid.NewGuid(), "Filho", "52998224725",
                dataNascimento, "Filho", dependenteSalarioFamilia: true));
    }

    [Fact]
    public void Dependente_DeveCriarComSucesso()
    {
        var dataNascimento = DateOnly.FromDateTime(DateTime.Today.AddYears(-5));

        var dependente = new Dependente(
            Guid.NewGuid(), "Filho", "52998224725",
            dataNascimento, "Filho", dependenteSalarioFamilia: true);

        Assert.Equal("Filho", dependente.Nome);
        Assert.True(dependente.DependenteSalarioFamilia);
    }

    [Fact]
    public void Documento_DeveCriarComSucesso()
    {
        var documento = new Documento(
            Guid.NewGuid(), "CTPS", "123456", new DateOnly(2020, 1, 1));

        Assert.Equal("CTPS", documento.Tipo);
        Assert.Equal("123456", documento.Numero);
    }

    [Fact]
    public void Sindicato_ContribuicaoAcima10_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            new Sindicato(Guid.NewGuid(), "S01", "Sindicato X", contribuicaoSindicalPercentual: 15));
    }

    [Fact]
    public void Sindicato_DeveCriarComSucesso()
    {
        var sindicato = new Sindicato(Guid.NewGuid(), "S01", "Sindicato X",
            contribuicaoSindicalPercentual: 5);

        Assert.Equal("S01", sindicato.Codigo);
        Assert.Equal(5m, sindicato.ContribuicaoSindicalPercentual);
    }

    [Fact]
    public void Convenio_SomaPercentuaisDiferente100_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            new Convenio(Guid.NewGuid(), "Plano A", "PlanoSaude", 500, 60, 30));
    }

    [Fact]
    public void Convenio_DeveCriarComSucesso()
    {
        var convenio = new Convenio(Guid.NewGuid(), "Plano A", "PlanoSaude", 500, 60, 40);

        Assert.Equal("Plano A", convenio.Nome);
        Assert.Equal(60m, convenio.PercentualEmpresa);
        Assert.Equal(40m, convenio.PercentualFuncionario);
    }

    [Fact]
    public void HorarioTrabalho_CargaAcima600_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            new HorarioTrabalho(Guid.NewGuid(), "H01", "Turno A", "Fixo",
                700, 40, new TimeOnly(8, 0), new TimeOnly(18, 0)));
    }

    [Fact]
    public void HorarioTrabalho_JornadaAcima6h_SemIntervalo_DeveLancarExcecao()
    {
        Assert.Throws<ArgumentException>(() =>
            new HorarioTrabalho(Guid.NewGuid(), "H01", "Turno A", "Fixo",
                480, 40, new TimeOnly(8, 0), new TimeOnly(17, 0)));
    }

    [Fact]
    public void HorarioTrabalho_DeveCriarComSucesso()
    {
        var horario = new HorarioTrabalho(
            Guid.NewGuid(), "H01", "Turno A", "Fixo",
            480, 40, new TimeOnly(8, 0), new TimeOnly(17, 0),
            new TimeOnly(12, 0), new TimeOnly(13, 0));

        Assert.Equal("H01", horario.Codigo);
        Assert.Equal(480, horario.CargaHorariaDiaria);
    }
}

public class EventosDomainTests
{
    [Fact]
    public void EmpresaCadastradaEvent_DeveCriarComSucesso()
    {
        var evento = new EmpresaCadastradaEvent(
            Guid.NewGuid(), "11222333000181", "Empresa X", "Lucro Real");

        Assert.Equal("11222333000181", evento.Cnpj);
        Assert.Equal("Empresa X", evento.RazaoSocial);
        Assert.NotEqual(default, evento.OcorridoEm);
    }

    [Fact]
    public void FuncionarioCadastradoEvent_DeveCriarComSucesso()
    {
        var evento = new FuncionarioCadastradoEvent(
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 5000m, new DateOnly(2024, 1, 15));

        Assert.Equal(5000m, evento.SalarioBase);
        Assert.Equal(new DateOnly(2024, 1, 15), evento.DataAdmissao);
    }

    [Fact]
    public void FuncionarioDesligadoEvent_DeveCriarComSucesso()
    {
        var evento = new FuncionarioDesligadoEvent(
            Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2025, 6, 1), "Fim de contrato");

        Assert.Equal(new DateOnly(2025, 6, 1), evento.DataDesligamento);
        Assert.Equal("Fim de contrato", evento.Motivo);
    }

    [Fact]
    public void RubricaAlteradaEvent_DeveCriarComSucesso()
    {
        var incidencias = new RubricaIncidencias(true, true, true, false, true, true, false);
        var evento = new RubricaAlteradaEvent(
            Guid.NewGuid(), Guid.NewGuid(), "001", "Vencimento", incidencias);

        Assert.Equal("001", evento.Codigo);
        Assert.True(evento.Incidencias.IncideInss);
    }
}
