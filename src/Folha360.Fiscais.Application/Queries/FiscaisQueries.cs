using Folha360.Application;
using Folha360.Fiscais.Application.DTOs;
using MediatR;

namespace Folha360.Fiscais.Application.Queries;

public record ObterApuracaoQuery(Guid EmpresaId, string Periodo) : IRequest<Result<ResumoApuracaoDto>>;
public record ObterApuracaoDetalhadaQuery(Guid EmpresaId, string Periodo) : IRequest<Result<List<ApuracaoFiscalDto>>>;
public record ObterStatusFiscalQuery(Guid EmpresaId) : IRequest<Result<StatusFiscalDto>>;
public record ObterCalendarioFiscalQuery(Guid EmpresaId, int Ano) : IRequest<Result<List<DateTime>>>;
public record ListarGuiasQuery(Guid EmpresaId, string Periodo) : IRequest<Result<List<GuiaRecolhimentoDto>>>;
public record ObterGuiaQuery(Guid EmpresaId, string Periodo, string Tributo) : IRequest<Result<Stream>>;
public record ObterDadosGuiaQuery(Guid EmpresaId, string Periodo, string Tributo) : IRequest<Result<GuiaRecolhimentoDto>>;
public record ListarExportacoesQuery(Guid EmpresaId, string Periodo) : IRequest<Result<List<ExportacaoDto>>>;
public record ListarRegrasFiscaisQuery(string? Tributo = null) : IRequest<Result<List<RegraFiscalDto>>>;
