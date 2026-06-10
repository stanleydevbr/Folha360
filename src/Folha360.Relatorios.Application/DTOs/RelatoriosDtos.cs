namespace Folha360.Relatorios.Application.DTOs;

public class HoleriteDto
{
    public Guid EmpresaId { get; set; }
    public string NomeEmpresa { get; set; } = string.Empty;
    public string CnpjEmpresa { get; set; } = string.Empty;
    public string? LogoEmpresaUrl { get; set; }
    public Guid FuncionarioId { get; set; }
    public string NomeFuncionario { get; set; } = string.Empty;
    public string Cargo { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public List<RubricaItemDto> Vencimentos { get; set; } = new();
    public List<RubricaItemDto> Descontos { get; set; } = new();
    public decimal TotalVencimentos { get; set; }
    public decimal TotalDescontos { get; set; }
    public decimal BaseInss { get; set; }
    public decimal BaseFgts { get; set; }
    public decimal BaseIrrf { get; set; }
    public decimal Liquido { get; set; }
}

public class RubricaItemDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

public class FolhaAnaliticaDto
{
    public Guid EmpresaId { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public List<FuncionarioFolhaDto> Funcionarios { get; set; } = new();
}

public class FuncionarioFolhaDto
{
    public Guid FuncionarioId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Departamento { get; set; } = string.Empty;
    public List<RubricaItemDto> Vencimentos { get; set; } = new();
    public List<RubricaItemDto> Descontos { get; set; } = new();
    public decimal TotalVencimentos { get; set; }
    public decimal TotalDescontos { get; set; }
    public decimal Liquido { get; set; }
}

public class FolhaSinteticaDto
{
    public Guid EmpresaId { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public List<RubricaTotalDto> TotaisPorRubrica { get; set; } = new();
    public List<DepartamentoTotalDto> TotaisPorDepartamento { get; set; } = new();
}

public class RubricaTotalDto
{
    public string Codigo { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Natureza { get; set; } = string.Empty;
    public decimal Valor { get; set; }
}

public class DepartamentoTotalDto
{
    public Guid DepartamentoId { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal TotalVencimentos { get; set; }
    public decimal TotalDescontos { get; set; }
    public decimal Liquido { get; set; }
}

public class ResumoMensalDto
{
    public Guid EmpresaId { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public int TotalFuncionarios { get; set; }
    public decimal TotalVencimentos { get; set; }
    public decimal TotalDescontos { get; set; }
    public decimal TotalLiquido { get; set; }
    public decimal TotalIrrf { get; set; }
    public decimal TotalInss { get; set; }
    public decimal TotalFgts { get; set; }
    public decimal VariacaoVencimentos { get; set; }
    public decimal VariacaoPercentual { get; set; }
}

public class ResumoAnualDto
{
    public Guid EmpresaId { get; set; }
    public int Ano { get; set; }
    public List<ResumoMensalDto> Meses { get; set; } = new();
    public decimal TotalAnualVencimentos { get; set; }
    public decimal TotalAnualDescontos { get; set; }
    public decimal TotalAnualLiquido { get; set; }
    public decimal MediaMensalVencimentos { get; set; }
}

public class DirfDto
{
    public Guid FuncionarioId { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public decimal RendimentosTributaveis { get; set; }
    public decimal RendimentosIsentos { get; set; }
    public decimal IrrfRetido { get; set; }
    public decimal DecimoTerceiro { get; set; }
    public decimal Ferias { get; set; }
}

public class RaisDto
{
    public Guid FuncionarioId { get; set; }
    public string Cpf { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string PisPasep { get; set; } = string.Empty;
    public DateTime? DataAdmissao { get; set; }
    public DateTime? DataDesligamento { get; set; }
    public string MotivoDesligamento { get; set; } = string.Empty;
    public decimal RemuneracaoJaneiro { get; set; }
    public decimal RemuneracaoFevereiro { get; set; }
    public decimal RemuneracaoMarco { get; set; }
    public decimal RemuneracaoAbril { get; set; }
    public decimal RemuneracaoMaio { get; set; }
    public decimal RemuneracaoJunho { get; set; }
    public decimal RemuneracaoJulho { get; set; }
    public decimal RemuneracaoAgosto { get; set; }
    public decimal RemuneracaoSetembro { get; set; }
    public decimal RemuneracaoOutubro { get; set; }
    public decimal RemuneracaoNovembro { get; set; }
    public decimal RemuneracaoDezembro { get; set; }
    public decimal RemuneracaoTotal { get; set; }
    public decimal DecimoTerceiro { get; set; }
}

public class LoteStatusDto
{
    public Guid LoteId { get; set; }
    public string Status { get; set; } = string.Empty;
    public int ProgressoPercentual { get; set; }
    public int EstimativaSegundos { get; set; }
    public List<string>? Erros { get; set; }
}

public class EmailDestinoDto
{
    public Guid EmpresaId { get; set; }
    public List<string> Destinatarios { get; set; } = new();
    public string? Assunto { get; set; }
    public string? Mensagem { get; set; }
}

public class RelatorioDto
{
    public string Titulo { get; set; } = string.Empty;
    public Guid EmpresaId { get; set; }
    public string NomeEmpresa { get; set; } = string.Empty;
    public string Periodo { get; set; } = string.Empty;
    public List<string> Cabecalhos { get; set; } = new();
    public List<List<string>> Linhas { get; set; } = new();
}
