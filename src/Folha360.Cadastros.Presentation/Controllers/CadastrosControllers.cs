using Folha360.Cadastros.Application;
using Folha360.Cadastros.Application.Commands;
using Folha360.Cadastros.Application.DTOs;
using Folha360.Cadastros.Application.Queries;
using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Cadastros.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class EmpresasController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmpresasController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<EmpresaDto>>> Listar(
        [FromQuery] ListarEmpresasQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<EmpresaDto>> Obter(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterEmpresaQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    [HttpPost]
    public async Task<ActionResult<EmpresaDto>> Criar(
        [FromBody] CriarEmpresaCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return UnprocessableEntity(new { result.Errors });
        return CreatedAtAction(nameof(Obter), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EmpresaDto>> Atualizar(
        Guid id, [FromBody] AtualizarEmpresaCommand command, CancellationToken ct)
    {
        command = command with { Id = id };
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ExcluirEmpresaCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { result.Errors });
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class FuncionariosController : ControllerBase
{
    private readonly IMediator _mediator;

    public FuncionariosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<FuncionarioDto>>> Listar(
        [FromQuery] ListarFuncionariosQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<FuncionarioDto>> Obter(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterFuncionarioQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    [HttpPost]
    public async Task<ActionResult<FuncionarioDto>> Criar(
        [FromBody] CriarFuncionarioCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return UnprocessableEntity(new { result.Errors });
        return CreatedAtAction(nameof(Obter), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FuncionarioDto>> Atualizar(
        Guid id, [FromBody] AtualizarFuncionarioCommand command, CancellationToken ct)
    {
        command = command with { Id = id };
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ExcluirFuncionarioCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { result.Errors });
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class CargosController : ControllerBase
{
    private readonly IMediator _mediator;

    public CargosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<CargoDto>>> Listar(
        [FromQuery] ListarCargosQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<CargoDto>> Obter(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterCargoQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    [HttpPost]
    public async Task<ActionResult<CargoDto>> Criar(
        [FromBody] CriarCargoCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return UnprocessableEntity(new { result.Errors });
        return CreatedAtAction(nameof(Obter), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<CargoDto>> Atualizar(
        Guid id, [FromBody] AtualizarCargoCommand command, CancellationToken ct)
    {
        command = command with { Id = id };
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ExcluirCargoCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { result.Errors });
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class RubricasController : ControllerBase
{
    private readonly IMediator _mediator;

    public RubricasController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<RubricaDto>>> Listar(
        [FromQuery] ListarRubricasQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<RubricaDto>> Obter(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterRubricaQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    [HttpPost]
    public async Task<ActionResult<RubricaDto>> Criar(
        [FromBody] CriarRubricaCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return UnprocessableEntity(new { result.Errors });
        return CreatedAtAction(nameof(Obter), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RubricaDto>> Atualizar(
        Guid id, [FromBody] AtualizarRubricaCommand command, CancellationToken ct)
    {
        command = command with { Id = id };
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ExcluirRubricaCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { result.Errors });
    }

    [HttpGet("conformidade")]
    [Authorize(Policy = "Contador")]
    public async Task<ActionResult<List<ConformidadeRubricaDto>>> VerificarConformidade(
        [FromQuery] Guid empresaId, CancellationToken ct)
    {
        var result = await _mediator.Send(new VerificarConformidadeQuery { EmpresaId = empresaId }, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    [HttpPost("simular")]
    [Authorize(Policy = "Contador")]
    public async Task<ActionResult<SimulacaoResultadoDto>> Simular(
        [FromBody] SimularRubricaCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    // --- Composição ---
    [HttpGet("{id:guid}/composicao")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<List<RubricaComposicaoDto>>> ListarComposicao(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ListarComposicaoQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    [HttpPost("{id:guid}/composicao")]
    public async Task<ActionResult<RubricaComposicaoDto>> AdicionarComponente(
        Guid id, [FromBody] AdicionarComponenteCommand command, CancellationToken ct)
    {
        command = command with { RubricaPrincipalId = id };
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return UnprocessableEntity(new { result.Errors });
        return CreatedAtAction(nameof(ListarComposicao), new { id }, result.Value);
    }

    [HttpDelete("{id:guid}/composicao/{compId:guid}")]
    public async Task<IActionResult> RemoverComponente(Guid id, Guid compId, CancellationToken ct)
    {
        var result = await _mediator.Send(new RemoverComponenteCommand(id, compId), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { result.Errors });
    }

    // --- Fórmula ---
    [HttpGet("{id:guid}/formula")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<RubricaFormulaDto>> ObterFormula(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterRubricaFormulaQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    [HttpPut("{id:guid}/formula")]
    public async Task<ActionResult<RubricaFormulaDto>> AtualizarFormula(
        Guid id, [FromBody] AtualizarRubricaFormulaCommand command, CancellationToken ct)
    {
        command = command with { RubricaId = id };
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    // --- Histórico ---
    [HttpGet("{id:guid}/historico")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<RubricaHistoricoDto>>> ListarHistorico(
        Guid id, [FromQuery] ListarHistoricoRubricaQuery query, CancellationToken ct)
    {
        query = query with { RubricaId = id };
        var result = await _mediator.Send(query, ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }

    // --- Incidências ---
    [HttpGet("{id:guid}/incidencias")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<List<RubricaIncidenciaDto>>> ListarIncidencias(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ListarIncidenciasQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    [HttpPost("{id:guid}/incidencias")]
    public async Task<ActionResult<RubricaIncidenciaDto>> AdicionarIncidencia(
        Guid id, [FromBody] AdicionarIncidenciaCommand command, CancellationToken ct)
    {
        command = command with { RubricaId = id };
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return UnprocessableEntity(new { result.Errors });
        return CreatedAtAction(nameof(ListarIncidencias), new { id }, result.Value);
    }

    [HttpDelete("{id:guid}/incidencias/{incidenciaId:guid}")]
    public async Task<IActionResult> RemoverIncidencia(Guid id, Guid incidenciaId, CancellationToken ct)
    {
        var result = await _mediator.Send(new RemoverIncidenciaCommand(id, incidenciaId), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { result.Errors });
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class GruposRubricaController : ControllerBase
{
    private readonly IMediator _mediator;

    public GruposRubricaController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<GrupoRubricaDto>>> Listar(
        [FromQuery] ListarGruposRubricaQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<GrupoRubricaDto>> Obter(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterGrupoRubricaQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    [HttpPost]
    public async Task<ActionResult<GrupoRubricaDto>> Criar(
        [FromBody] CriarGrupoRubricaCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return UnprocessableEntity(new { result.Errors });
        return CreatedAtAction(nameof(Obter), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<GrupoRubricaDto>> Atualizar(
        Guid id, [FromBody] AtualizarGrupoRubricaCommand command, CancellationToken ct)
    {
        command = command with { Id = id };
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ExcluirGrupoRubricaCommand(id), ct);
        return result.IsSuccess ? NoContent() : UnprocessableEntity(new { result.Errors });
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class TabelasProgressivasController : ControllerBase
{
    private readonly IMediator _mediator;

    public TabelasProgressivasController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<List<RubricaTabelaProgressivaDto>>> Listar(
        [FromQuery] ListarFaixasProgressivasQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    [HttpPost]
    public async Task<ActionResult<RubricaTabelaProgressivaDto>> Criar(
        [FromBody] CriarFaixaProgressivaCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return UnprocessableEntity(new { result.Errors });
        return CreatedAtAction(nameof(Listar), new { rubricaId = result.Value!.RubricaId }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<RubricaTabelaProgressivaDto>> Atualizar(
        Guid id, [FromBody] AtualizarFaixaProgressivaCommand command, CancellationToken ct)
    {
        command = command with { Id = id };
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ExcluirFaixaProgressivaCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { result.Errors });
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class LotacoesController : ControllerBase
{
    private readonly IMediator _mediator;

    public LotacoesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<LotacaoDto>>> Listar(
        [FromQuery] ListarLotacoesQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<LotacaoDto>> Obter(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ObterLotacaoQuery(id), ct);
        return result.IsSuccess ? Ok(result.Value) : NotFound(new { result.Errors });
    }

    [HttpPost]
    public async Task<ActionResult<LotacaoDto>> Criar(
        [FromBody] CriarLotacaoCommand command, CancellationToken ct)
    {
        var result = await _mediator.Send(command, ct);
        if (!result.IsSuccess)
            return UnprocessableEntity(new { result.Errors });
        return CreatedAtAction(nameof(Obter), new { id = result.Value!.Id }, result.Value);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LotacaoDto>> Atualizar(
        Guid id, [FromBody] AtualizarLotacaoCommand command, CancellationToken ct)
    {
        command = command with { Id = id };
        var result = await _mediator.Send(command, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new ExcluirLotacaoCommand(id), ct);
        return result.IsSuccess ? NoContent() : NotFound(new { result.Errors });
    }
}

// ============================
// Lookup Controllers (T11)
// ============================
[ApiController]
[Route("api/[controller]")]
public class CbosController : ControllerBase
{
    private readonly IMediator _mediator;
    public CbosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<List<CboDto>>> Listar(
        [FromQuery] ListarCbosQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }
}

[ApiController]
[Route("api/[controller]")]
public class NaturezasJuridicasController : ControllerBase
{
    private readonly IMediator _mediator;
    public NaturezasJuridicasController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<List<NaturezaJuridicaDto>>> Listar(
        [FromQuery] ListarNaturezasJuridicasQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }
}

[ApiController]
[Route("api/[controller]")]
public class MunicipiosController : ControllerBase
{
    private readonly IMediator _mediator;
    public MunicipiosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<PaginatedResult<MunicipioIBGEDto>>> Listar(
        [FromQuery] ListarMunicipiosQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(result);
    }
}

[ApiController]
[Route("api/[controller]")]
public class BancosController : ControllerBase
{
    private readonly IMediator _mediator;
    public BancosController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<List<BancoFebrabanDto>>> Listar(
        [FromQuery] ListarBancosQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return result.IsSuccess ? Ok(result.Value) : UnprocessableEntity(new { result.Errors });
    }
}

// ============================
// Cadastros de Apoio (Sindicatos, Convênios, Horários, Dependentes, Documentos, Processos)
// ============================
[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class SindicatosController : ControllerBase
{
    private readonly ISindicatoRepository _repo;
    public SindicatosController(ISindicatoRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<IEnumerable<Sindicato>>> Listar(
        [FromQuery] Guid? empresaId, CancellationToken ct)
    {
        if (empresaId.HasValue)
        {
            var (items, total) = await _repo.GetPagedAsync(1, 100, empresaId: empresaId, ct: ct);
            return Ok(items);
        }

        var todos = await _repo.GetAllAsync(ct);
        return Ok(todos);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<Sindicato>> Obter(Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Sindicato>> Criar([FromBody] Sindicato entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Obter), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Atualizar(Guid id, [FromBody] Sindicato entity, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null)
        {
            return NotFound();
        }
        await _repo.UpdateAsync(entity, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(id, ct);
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class ConveniosController : ControllerBase
{
    private readonly IConvenioRepository _repo;
    public ConveniosController(IConvenioRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<IEnumerable<Convenio>>> Listar(
        [FromQuery] Guid? empresaId, CancellationToken ct)
    {
        if (empresaId.HasValue)
        {
            var (items, total) = await _repo.GetPagedAsync(1, 100, empresaId: empresaId, ct: ct);
            return Ok(items);
        }

        var todos = await _repo.GetAllAsync(ct);
        return Ok(todos);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<Convenio>> Obter(Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Convenio>> Criar([FromBody] Convenio entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Obter), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Atualizar(Guid id, [FromBody] Convenio entity, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null)
        {
            return NotFound();
        }
        await _repo.UpdateAsync(entity, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(id, ct);
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class HorariosTrabalhoController : ControllerBase
{
    private readonly IHorarioTrabalhoRepository _repo;
    public HorariosTrabalhoController(IHorarioTrabalhoRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<IEnumerable<HorarioTrabalho>>> Listar(
        [FromQuery] Guid? empresaId, CancellationToken ct)
    {
        if (empresaId.HasValue)
        {
            var (items, total) = await _repo.GetPagedAsync(1, 100, empresaId: empresaId, ct: ct);
            return Ok(items);
        }

        var todos = await _repo.GetAllAsync(ct);
        return Ok(todos);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<HorarioTrabalho>> Obter(Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<HorarioTrabalho>> Criar([FromBody] HorarioTrabalho entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Obter), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Atualizar(Guid id, [FromBody] HorarioTrabalho entity, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null)
        {
            return NotFound();
        }
        await _repo.UpdateAsync(entity, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(id, ct);
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class DependentesController : ControllerBase
{
    private readonly IDependenteRepository _repo;
    public DependentesController(IDependenteRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<IEnumerable<Dependente>>> Listar(
        [FromQuery] Guid funcionarioId, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(1, 100, funcionarioId: funcionarioId, ct: ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<Dependente>> Obter(Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Dependente>> Criar([FromBody] Dependente entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Obter), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Atualizar(Guid id, [FromBody] Dependente entity, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null)
        {
            return NotFound();
        }
        await _repo.UpdateAsync(entity, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(id, ct);
        return NoContent();
    }
}

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class DocumentosController : ControllerBase
{
    private readonly IDocumentoRepository _repo;
    public DocumentosController(IDocumentoRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<IEnumerable<Documento>>> Listar(
        [FromQuery] Guid funcionarioId, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(1, 100, funcionarioId: funcionarioId, ct: ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<Documento>> Obter(Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Documento>> Criar([FromBody] Documento entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Obter), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Atualizar(Guid id, [FromBody] Documento entity, CancellationToken ct)
    {
        var existing = await _repo.GetByIdAsync(id, ct);
        if (existing is null)
        {
            return NotFound();
        }
        await _repo.UpdateAsync(entity, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid id, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(id, ct);
        return NoContent();
    }
}

// ============================
// Expansão Empresa (T12)
// ============================
[ApiController]
[Route("api/empresas/{empresaId:guid}/configuracoes-bancarias")]
[Authorize(Policy = "Operador")]
public class EmpresasConfiguracoesBancariasController : ControllerBase
{
    private readonly IConfiguracaoBancariaEmpresaRepository _repo;
    public EmpresasConfiguracoesBancariasController(IConfiguracaoBancariaEmpresaRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<IEnumerable<ConfiguracaoBancariaEmpresa>>> Listar(
        Guid empresaId, CancellationToken ct)
    {
        var items = await _repo.ListarPorEmpresaAsync(empresaId, ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<ConfiguracaoBancariaEmpresa>> Criar(
        Guid empresaId, [FromBody] ConfiguracaoBancariaEmpresa entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Listar), new { empresaId }, entity);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid empresaId, Guid id, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(id, ct);
        return NoContent();
    }
}

[ApiController]
[Route("api/empresas/{empresaId:guid}/enderecos")]
[Authorize(Policy = "Operador")]
public class EmpresasEnderecosController : ControllerBase
{
    private readonly IEnderecoEmpresaRepository _repo;
    public EmpresasEnderecosController(IEnderecoEmpresaRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<IEnumerable<EnderecoEmpresa>>> Listar(
        Guid empresaId, CancellationToken ct)
    {
        var items = await _repo.ListarPorEmpresaAsync(empresaId, ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<EnderecoEmpresa>> Criar(
        Guid empresaId, [FromBody] EnderecoEmpresa entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Listar), new { empresaId }, entity);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid empresaId, Guid id, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(id, ct);
        return NoContent();
    }
}

[ApiController]
[Route("api/empresas/{empresaId:guid}/contatos")]
[Authorize(Policy = "Operador")]
public class EmpresasContatosController : ControllerBase
{
    private readonly IContatoEmpresaRepository _repo;
    public EmpresasContatosController(IContatoEmpresaRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<IEnumerable<ContatoEmpresa>>> Listar(
        Guid empresaId, CancellationToken ct)
    {
        var items = await _repo.ListarPorEmpresaAsync(empresaId, ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<ContatoEmpresa>> Criar(
        Guid empresaId, [FromBody] ContatoEmpresa entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Listar), new { empresaId }, entity);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid empresaId, Guid id, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(id, ct);
        return NoContent();
    }
}

[ApiController]
[Route("api/empresas/{empresaId:guid}/configuracoes-gerais")]
[Authorize(Policy = "Operador")]
public class EmpresasConfiguracoesGeraisController : ControllerBase
{
    private readonly IConfiguracaoGeralRepository _repo;
    public EmpresasConfiguracoesGeraisController(IConfiguracaoGeralRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<IEnumerable<ConfiguracaoGeral>>> Listar(
        Guid empresaId, CancellationToken ct)
    {
        var items = await _repo.ListarPorEmpresaAsync(empresaId, ct);
        return Ok(items);
    }

    [HttpPut]
    public async Task<ActionResult> Atualizar(
        Guid empresaId, [FromBody] Dictionary<string, string> configuracoes, CancellationToken ct)
    {
        foreach (var (chave, valor) in configuracoes)
        {
            var existing = await _repo.GetByChaveAsync(empresaId, chave, ct);
            if (existing is not null)
            {
                existing.AtualizarValor(valor);
                await _repo.UpdateAsync(existing, ct);
            }
            else
            {
                await _repo.AddAsync(new ConfiguracaoGeral(empresaId, chave, valor), ct);
            }
        }

        return NoContent();
    }
}

[ApiController]
[Route("api/empresas/{empresaId:guid}/config-esocial")]
[Authorize(Policy = "Admin")]
public class EmpresasConfigESocialController : ControllerBase
{
    private readonly IConfiguracaoESocialRepository _repo;
    public EmpresasConfigESocialController(IConfiguracaoESocialRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Contador")]
    public async Task<ActionResult<ConfiguracaoESocial>> Obter(Guid empresaId, CancellationToken ct)
    {
        var entity = await _repo.GetByEmpresaAsync(empresaId, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPut]
    public async Task<ActionResult> Atualizar(
        Guid empresaId, [FromBody] ConfiguracaoESocial entity, CancellationToken ct)
    {
        var existing = await _repo.GetByEmpresaAsync(empresaId, ct);
        if (existing is not null)
        {
            await _repo.UpdateAsync(entity, ct);
        }
        else
        {
            await _repo.AddAsync(entity, ct);
        }

        return NoContent();
    }
}

// ============================
// Expansão Funcionário (T13)
// ============================
[ApiController]
[Route("api/funcionarios/{funcionarioId:guid}/dados-bancarios")]
[Authorize(Policy = "Operador")]
public class FuncionariosDadosBancariosController : ControllerBase
{
    private readonly IDadosBancariosFuncionarioRepository _repo;
    public FuncionariosDadosBancariosController(IDadosBancariosFuncionarioRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<IEnumerable<DadosBancariosFuncionario>>> Listar(
        Guid funcionarioId, CancellationToken ct)
    {
        var items = await _repo.ListarPorFuncionarioAsync(funcionarioId, ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<DadosBancariosFuncionario>> Criar(
        Guid funcionarioId, [FromBody] DadosBancariosFuncionario entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Listar), new { funcionarioId }, entity);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid funcionarioId, Guid id, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(id, ct);
        return NoContent();
    }
}

[ApiController]
[Route("api/funcionarios/{funcionarioId:guid}/contrato")]
[Authorize(Policy = "Operador")]
public class FuncionariosContratoController : ControllerBase
{
    private readonly IContratoTrabalhoRepository _repo;
    public FuncionariosContratoController(IContratoTrabalhoRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<ContratoTrabalho>> Obter(Guid funcionarioId, CancellationToken ct)
    {
        var entity = await _repo.GetByFuncionarioAsync(funcionarioId, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<ContratoTrabalho>> Criar(
        Guid funcionarioId, [FromBody] ContratoTrabalho entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Obter), new { funcionarioId }, entity);
    }

    [HttpPut]
    public async Task<ActionResult> Atualizar(
        Guid funcionarioId, [FromBody] ContratoTrabalho entity, CancellationToken ct)
    {
        var existing = await _repo.GetByFuncionarioAsync(funcionarioId, ct);
        if (existing is null)
        {
            return NotFound();
        }
        await _repo.UpdateAsync(entity, ct);
        return NoContent();
    }
}

[ApiController]
[Route("api/funcionarios/{funcionarioId:guid}/remuneracao")]
[Authorize(Policy = "Operador")]
public class FuncionariosRemuneracaoController : ControllerBase
{
    private readonly IRemuneracaoBeneficioRepository _repo;
    public FuncionariosRemuneracaoController(IRemuneracaoBeneficioRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<RemuneracaoBeneficio>> Obter(Guid funcionarioId, CancellationToken ct)
    {
        var entity = await _repo.GetByFuncionarioAsync(funcionarioId, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPut]
    public async Task<ActionResult> Atualizar(
        Guid funcionarioId, [FromBody] RemuneracaoBeneficio entity, CancellationToken ct)
    {
        var existing = await _repo.GetByFuncionarioAsync(funcionarioId, ct);
        if (existing is not null)
        {
            await _repo.UpdateAsync(entity, ct);
        }
        else
        {
            await _repo.AddAsync(entity, ct);
        }

        return NoContent();
    }
}

[ApiController]
[Route("api/funcionarios/{funcionarioId:guid}/afastamentos")]
[Authorize(Policy = "Operador")]
public class FuncionariosAfastamentosController : ControllerBase
{
    private readonly IAfastamentoFuncionarioRepository _repo;
    public FuncionariosAfastamentosController(IAfastamentoFuncionarioRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<IEnumerable<AfastamentoFuncionario>>> Listar(
        Guid funcionarioId, CancellationToken ct)
    {
        var items = await _repo.ListarPorFuncionarioAsync(funcionarioId, ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<AfastamentoFuncionario>> Criar(
        Guid funcionarioId, [FromBody] AfastamentoFuncionario entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Listar), new { funcionarioId }, entity);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid funcionarioId, Guid id, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(id, ct);
        return NoContent();
    }
}

[ApiController]
[Route("api/funcionarios/{funcionarioId:guid}/info-esocial")]
[Authorize(Policy = "Operador")]
public class FuncionariosInfoESocialController : ControllerBase
{
    private readonly IInfoESocialFuncionarioRepository _repo;
    public FuncionariosInfoESocialController(IInfoESocialFuncionarioRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<InfoESocialFuncionario>> Obter(Guid funcionarioId, CancellationToken ct)
    {
        var entity = await _repo.GetByFuncionarioAsync(funcionarioId, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPut]
    public async Task<ActionResult> Atualizar(
        Guid funcionarioId, [FromBody] InfoESocialFuncionario entity, CancellationToken ct)
    {
        var existing = await _repo.GetByFuncionarioAsync(funcionarioId, ct);
        if (existing is not null)
        {
            await _repo.UpdateAsync(entity, ct);
        }
        else
        {
            await _repo.AddAsync(entity, ct);
        }

        return NoContent();
    }
}

[ApiController]
[Route("api/funcionarios/{funcionarioId:guid}/movimentacao-fixa")]
[Authorize(Policy = "Operador")]
public class FuncionariosMovimentacaoFixaController : ControllerBase
{
    private readonly IMovimentacaoFixaRepository _repo;
    public FuncionariosMovimentacaoFixaController(IMovimentacaoFixaRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<IEnumerable<MovimentacaoFixa>>> Listar(
        Guid funcionarioId, CancellationToken ct)
    {
        var items = await _repo.ListarPorFuncionarioAsync(funcionarioId, ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<MovimentacaoFixa>> Criar(
        Guid funcionarioId, [FromBody] MovimentacaoFixa entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Listar), new { funcionarioId }, entity);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid funcionarioId, Guid id, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(id, ct);
        return NoContent();
    }
}

[ApiController]
[Route("api/funcionarios/{funcionarioId:guid}/movimentacao-mensal")]
[Authorize(Policy = "Operador")]
public class FuncionariosMovimentacaoMensalController : ControllerBase
{
    private readonly IMovimentacaoMensalRepository _repo;
    public FuncionariosMovimentacaoMensalController(IMovimentacaoMensalRepository repo) => _repo = repo;

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<IEnumerable<MovimentacaoMensal>>> Listar(
        Guid funcionarioId, [FromQuery] string mes, CancellationToken ct)
    {
        var items = await _repo.ListarPorFuncionarioMesAsync(funcionarioId, mes, ct);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<MovimentacaoMensal>> Criar(
        Guid funcionarioId, [FromBody] MovimentacaoMensal entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Listar), new { funcionarioId, mes = entity.MesAno }, entity);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Excluir(Guid funcionarioId, Guid id, CancellationToken ct)
    {
        await _repo.SoftDeleteAsync(id, ct);
        return NoContent();
    }
}
