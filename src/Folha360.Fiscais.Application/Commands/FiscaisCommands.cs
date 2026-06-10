using Folha360.Application;
using Folha360.Fiscais.Application.DTOs;
using MediatR;

namespace Folha360.Fiscais.Application.Commands;

public record ApurarObrigacoesCommand(
    Guid EmpresaId,
    string Periodo,
    Guid ProcessamentoId) : IRequest<Result<ResumoApuracaoDto>>;

public record GerarGuiasCommand(
    Guid ApuracaoFiscalId) : IRequest<Result<List<GuiaRecolhimentoDto>>>;

public record ExportarLancamentosCommand(
    Guid EmpresaId,
    string Periodo) : IRequest<Result<List<ExportacaoDto>>>;

public record ReverterObrigacoesCommand(
    Guid EmpresaId,
    string Periodo) : IRequest<Result<bool>>;

public record CriarRegraFiscalCommand(
    string Tributo,
    int Versao,
    string VigenciaInicio,
    string? VigenciaFim,
    string Parametros,
    string CodigoReceita) : IRequest<Result<RegraFiscalDto>>;

public record EnviarExportacaoSftpCommand(
    Guid EmpresaId,
    string Periodo) : IRequest<Result<bool>>;

public record RegistrarPagamentoGuiaCommand(
    Guid GuiaId,
    decimal ValorPago,
    DateTime DataPagamento) : IRequest<Result<bool>>;
