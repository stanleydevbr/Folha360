using Folha360.Application;

namespace Folha360.Relatorios.Application.Commands;

public class GerarHoleritesLoteCommand : IRequest<Result<Guid>>
{
    public Guid EmpresaId { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public List<Guid>? FuncionarioIds { get; set; }
}

public class CriarAgendamentoCommand : IRequest<Result<Guid>>
{
    public Guid EmpresaId { get; set; }
    public TipoRelatorio TipoRelatorio { get; set; }
    public FormatoExportacao Formato { get; set; }
    public string Recorrencia { get; set; } = string.Empty;
    public List<string> Destinatarios { get; set; } = new();
}

public class AtualizarAgendamentoCommand : IRequest<Result<bool>>
{
    public Guid AgendamentoId { get; set; }
    public string? Recorrencia { get; set; }
    public FormatoExportacao? Formato { get; set; }
    public List<string>? Destinatarios { get; set; }
    public bool? Ativo { get; set; }
}

public class CancelarAgendamentoCommand : IRequest<Result<bool>>
{
    public Guid AgendamentoId { get; set; }
}

public class EnviarEmailCommand : IRequest<Result<bool>>
{
    public Guid EmpresaId { get; set; }
    public TipoRelatorio TipoRelatorio { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public FormatoExportacao Formato { get; set; }
    public List<string> Destinatarios { get; set; } = new();
    public string? Assunto { get; set; }
    public string? Mensagem { get; set; }
}

public class RefreshViewsCommand : IRequest<Result<bool>>
{
    public Guid EmpresaId { get; set; }
    public string Periodo { get; set; } = string.Empty;
}

public class InvalidarRelatoriosCommand : IRequest<Result<bool>>
{
    public Guid EmpresaId { get; set; }
    public string Periodo { get; set; } = string.Empty;
}
