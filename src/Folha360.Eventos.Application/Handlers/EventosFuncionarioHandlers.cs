using Folha360.Eventos.Application.DTOs;
using Folha360.Eventos.Application.Queries;
using Folha360.Eventos.Domain.Abstractions;
using MediatR;

namespace Folha360.Eventos.Application.Handlers;

public class ListarEventosFuncionarioHandler : IRequestHandler<ListarEventosFuncionarioQuery, Result<EventosFuncionarioDto>>
{
    private readonly IAdmissaoRepository _admissaoRepo;
    private readonly IFeriasRepository _feriasRepo;
    private readonly IAfastamentoRepository _afastamentoRepo;
    private readonly IDesligamentoRepository _desligamentoRepo;
    private readonly IAlteracaoContratualRepository _alteracaoRepo;

    public ListarEventosFuncionarioHandler(
        IAdmissaoRepository admissaoRepo,
        IFeriasRepository feriasRepo,
        IAfastamentoRepository afastamentoRepo,
        IDesligamentoRepository desligamentoRepo,
        IAlteracaoContratualRepository alteracaoRepo)
    {
        _admissaoRepo = admissaoRepo;
        _feriasRepo = feriasRepo;
        _afastamentoRepo = afastamentoRepo;
        _desligamentoRepo = desligamentoRepo;
        _alteracaoRepo = alteracaoRepo;
    }

    public async Task<Result<EventosFuncionarioDto>> Handle(ListarEventosFuncionarioQuery query, CancellationToken ct)
    {
        var eventos = new List<EventoFuncionarioItemDto>();

        var admissoes = await _admissaoRepo.GetPagedAsync(1, 100, query.FuncionarioId, ct);
        eventos.AddRange(admissoes.Items.Select(a => new EventoFuncionarioItemDto
        {
            TipoEvento = "Admissao",
            Id = a.Id,
            DataEvento = a.DataAdmissao,
            Descricao = $"Admissão em {a.DataAdmissao:dd/MM/yyyy}",
        }));

        var ferias = await _feriasRepo.GetPagedAsync(1, 100, query.FuncionarioId, ct);
        eventos.AddRange(ferias.Items.Select(f => new EventoFuncionarioItemDto
        {
            TipoEvento = "Ferias",
            Id = f.Id,
            DataEvento = f.DataInicio,
            Descricao = $"Férias de {f.DiasGozo} dias a partir de {f.DataInicio:dd/MM/yyyy}",
        }));

        var afastamentos = await _afastamentoRepo.GetPagedAsync(1, 100, query.FuncionarioId, ct);
        eventos.AddRange(afastamentos.Items.Select(a => new EventoFuncionarioItemDto
        {
            TipoEvento = "Afastamento",
            Id = a.Id,
            DataEvento = a.DataInicio,
            Descricao = $"Afastamento por {a.TipoAfastamento} de {a.DataInicio:dd/MM/yyyy} até {a.DataFimPrevista:dd/MM/yyyy}",
        }));

        var desligamentos = await _desligamentoRepo.GetPagedAsync(1, 100, query.FuncionarioId, ct);
        eventos.AddRange(desligamentos.Items.Select(d => new EventoFuncionarioItemDto
        {
            TipoEvento = "Desligamento",
            Id = d.Id,
            DataEvento = d.DataDesligamento,
            Descricao = $"Desligamento em {d.DataDesligamento:dd/MM/yyyy} - {d.MotivoDesligamento}",
        }));

        var alteracoes = await _alteracaoRepo.GetPagedAsync(1, 100, query.FuncionarioId, ct);
        eventos.AddRange(alteracoes.Items.Select(a => new EventoFuncionarioItemDto
        {
            TipoEvento = "AlteracaoContratual",
            Id = a.Id,
            DataEvento = a.DataAlteracao,
            Descricao = $"Alteração contratual em {a.DataAlteracao:dd/MM/yyyy}",
        }));

        var eventosOrdenados = eventos.OrderByDescending(e => e.DataEvento).ToList();

        return Result<EventosFuncionarioDto>.Success(new EventosFuncionarioDto
        {
            FuncionarioId = query.FuncionarioId,
            Eventos = eventosOrdenados,
        });
    }
}
