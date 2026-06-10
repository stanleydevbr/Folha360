using Folha360.Application;

namespace Folha360.Relatorios.Application.Queries;

public class ObterDirfQuery : IRequest<Result<IReadOnlyList<DirfDto>>>
{
    public Guid EmpresaId { get; set; }
    public int Ano { get; set; }
}

public class ObterRaisQuery : IRequest<Result<IReadOnlyList<RaisDto>>>
{
    public Guid EmpresaId { get; set; }
    public int Ano { get; set; }
}

public class ObterHoleriteQuery : IRequest<Result<HoleriteDto>>
{
    public Guid EmpresaId { get; set; }
    public Guid FuncionarioId { get; set; }
    public string Periodo { get; set; } = string.Empty;
}

public class ListarHoleritesQuery : IRequest<Result<IReadOnlyList<HoleriteDto>>>
{
    public Guid EmpresaId { get; set; }
    public string Periodo { get; set; } = string.Empty;
}

public class ObterFolhaAnaliticaQuery : IRequest<Result<FolhaAnaliticaDto>>
{
    public Guid EmpresaId { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public Guid? DepartamentoId { get; set; }
    public string? TipoCalculo { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class ObterFolhaSinteticaQuery : IRequest<Result<FolhaSinteticaDto>>
{
    public Guid EmpresaId { get; set; }
    public string Periodo { get; set; } = string.Empty;
    public Guid? DepartamentoId { get; set; }
}

public class ObterResumoMensalQuery : IRequest<Result<ResumoMensalDto>>
{
    public Guid EmpresaId { get; set; }
    public string Periodo { get; set; } = string.Empty;
}

public class ObterResumoAnualQuery : IRequest<Result<ResumoAnualDto>>
{
    public Guid EmpresaId { get; set; }
    public int Ano { get; set; }
}

public class ObterStatusLoteQuery : IRequest<Result<LoteStatusDto>>
{
    public Guid LoteId { get; set; }
}

public class ListarAgendamentosQuery : IRequest<Result<IReadOnlyList<object>>>
{
    public Guid EmpresaId { get; set; }
}

public class ObterHistoricoAgendamentoQuery : IRequest<Result<IReadOnlyList<object>>>
{
    public Guid AgendamentoId { get; set; }
}
