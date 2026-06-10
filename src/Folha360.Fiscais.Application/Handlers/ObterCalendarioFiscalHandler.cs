using Folha360.Application;
using Folha360.Fiscais.Application.Queries;
using MediatR;

namespace Folha360.Fiscais.Application.Handlers;

public class ObterCalendarioFiscalHandler : IRequestHandler<ObterCalendarioFiscalQuery, Result<List<DateTime>>>
{
    public Task<Result<List<DateTime>>> Handle(ObterCalendarioFiscalQuery request, CancellationToken ct)
    {
        var vencimentos = new List<DateTime>();
        for (int mes = 1; mes <= 12; mes++)
        {
            vencimentos.Add(new DateTime(request.Ano, mes, 20)); // GPS
            vencimentos.Add(new DateTime(request.Ano, mes, 7));  // GRF
        }

        return Task.FromResult(Result<List<DateTime>>.Success(vencimentos));
    }
}
