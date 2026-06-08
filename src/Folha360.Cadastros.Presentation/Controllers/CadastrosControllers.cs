using Folha360.Cadastros.Application;
using Folha360.Cadastros.Application.Commands;
using Folha360.Cadastros.Application.DTOs;
using Folha360.Cadastros.Application.Queries;
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
