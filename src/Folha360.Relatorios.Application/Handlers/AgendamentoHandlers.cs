using Folha360.Application;
using Folha360.Relatorios.Application.Services;
using Folha360.Relatorios.Domain.Abstractions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Folha360.Relatorios.Application.Handlers;

public class CriarAgendamentoHandler : IRequestHandler<CriarAgendamentoCommand, Result<Guid>>
{
    private readonly IAgendamentoService _agendamentoService;

    public CriarAgendamentoHandler(IAgendamentoService agendamentoService)
    {
        _agendamentoService = agendamentoService;
    }

    public async Task<Result<Guid>> Handle(CriarAgendamentoCommand request, CancellationToken ct)
    {
        var id = await _agendamentoService.CriarAsync(request, ct);
        return Result<Guid>.Success(id);
    }
}

public class AtualizarAgendamentoHandler : IRequestHandler<AtualizarAgendamentoCommand, Result<bool>>
{
    private readonly IAgendamentoService _agendamentoService;

    public AtualizarAgendamentoHandler(IAgendamentoService agendamentoService)
    {
        _agendamentoService = agendamentoService;
    }

    public async Task<Result<bool>> Handle(AtualizarAgendamentoCommand request, CancellationToken ct)
    {
        await _agendamentoService.AtualizarAsync(request, ct);
        return Result<bool>.Success(true);
    }
}

public class CancelarAgendamentoHandler : IRequestHandler<CancelarAgendamentoCommand, Result<bool>>
{
    private readonly IAgendamentoService _agendamentoService;

    public CancelarAgendamentoHandler(IAgendamentoService agendamentoService)
    {
        _agendamentoService = agendamentoService;
    }

    public async Task<Result<bool>> Handle(CancelarAgendamentoCommand request, CancellationToken ct)
    {
        await _agendamentoService.CancelarAsync(request.AgendamentoId, ct);
        return Result<bool>.Success(true);
    }
}

public class EnviarEmailHandler : IRequestHandler<EnviarEmailCommand, Result<bool>>
{
    private readonly IRelatorioStorageService _storageService;
    private readonly IRelatorioEmailService _emailService;

    public EnviarEmailHandler(
        IRelatorioStorageService storageService,
        IRelatorioEmailService emailService)
    {
        _storageService = storageService;
        _emailService = emailService;
    }

    public async Task<Result<bool>> Handle(EnviarEmailCommand request, CancellationToken ct)
    {
        var chave = $"{request.EmpresaId}/{request.TipoRelatorio}/{request.Periodo}/{request.Formato.ToString().ToLower()}";
        var bucket = request.TipoRelatorio == TipoRelatorio.Holerite ? "folha360-holerites" : "folha360-relatorios";

        Stream? anexo = null;
        string? nomeArquivo = null;
        string? mensagem = request.Mensagem;

        if (await _storageService.ExisteAsync(bucket, chave, ct))
        {
            var stream = await _storageService.RecuperarAsync(bucket, chave, ct);
            if (stream.Length <= 10 * 1024 * 1024)
            {
                anexo = stream;
                nomeArquivo = $"{request.TipoRelatorio}_{request.Periodo}.{request.Formato.ToString().ToLower()}";
            }
            else
            {
                var url = await _storageService.GerarUrlAssinadaAsync(bucket, chave, TimeSpan.FromDays(7), ct);
                mensagem = (mensagem ?? string.Empty) + $"\n\nLink para download: {url}";
            }
        }

        var destino = new DTOs.EmailDestinoDto
        {
            EmpresaId = request.EmpresaId,
            Destinatarios = request.Destinatarios,
            Assunto = request.Assunto,
            Mensagem = mensagem,
        };

        await _emailService.EnviarAsync(destino, anexo, nomeArquivo, ct);
        return Result<bool>.Success(true);
    }
}

public class RefreshViewsHandler : IRequestHandler<RefreshViewsCommand, Result<bool>>
{
    private readonly Folha360.Infrastructure.Data.Folha360DbContext _context;
    private readonly IRedisCacheService _cacheService;
    private readonly Microsoft.Extensions.Logging.ILogger<RefreshViewsHandler> _logger;

    public RefreshViewsHandler(
        Folha360.Infrastructure.Data.Folha360DbContext context,
        IRedisCacheService cacheService,
        Microsoft.Extensions.Logging.ILogger<RefreshViewsHandler> logger)
    {
        _context = context;
        _cacheService = cacheService;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(RefreshViewsCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Refresh de materialized views iniciado para empresa {EmpresaId}, período {Periodo}", request.EmpresaId, request.Periodo);

        await _context.Database.ExecuteSqlRawAsync("REFRESH MATERIALIZED VIEW CONCURRENTLY vw_resumo_folha_mensal", ct);
        await _context.Database.ExecuteSqlRawAsync("REFRESH MATERIALIZED VIEW CONCURRENTLY vw_dirf_anual", ct);
        await _context.Database.ExecuteSqlRawAsync("REFRESH MATERIALIZED VIEW CONCURRENTLY vw_rais_anual", ct);

        // Invalidate cache
        await _cacheService.InvalidarAsync($"relatorios:resumo:{request.EmpresaId}:{request.Periodo}", ct);

        _logger.LogInformation("Refresh de materialized views concluído");
        return Result<bool>.Success(true);
    }
}

public class InvalidarRelatoriosHandler : IRequestHandler<InvalidarRelatoriosCommand, Result<bool>>
{
    private readonly IAgendamentoRepository _agendamentoRepository;
    private readonly Microsoft.Extensions.Logging.ILogger<InvalidarRelatoriosHandler> _logger;

    public InvalidarRelatoriosHandler(
        IAgendamentoRepository agendamentoRepository,
        Microsoft.Extensions.Logging.ILogger<InvalidarRelatoriosHandler> logger)
    {
        _agendamentoRepository = agendamentoRepository;
        _logger = logger;
    }

    public async Task<Result<bool>> Handle(InvalidarRelatoriosCommand request, CancellationToken ct)
    {
        _logger.LogInformation("Invalidando relatórios para empresa {EmpresaId}, período {Periodo}", request.EmpresaId, request.Periodo);
        await _agendamentoRepository.InvalidarArquivosAsync(request.EmpresaId, request.Periodo, ct);
        return Result<bool>.Success(true);
    }
}
