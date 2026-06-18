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
        if (entity.Id != id)
        {
            return BadRequest("Entity ID does not match route ID.");
        }

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
        if (entity.Id != id)
        {
            return BadRequest("Entity ID does not match route ID.");
        }

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
        if (entity.Id != id)
        {
            return BadRequest("Entity ID does not match route ID.");
        }

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
        if (entity.Id != id)
        {
            return BadRequest("Entity ID does not match route ID.");
        }

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
        if (entity.Id != id)
        {
            return BadRequest("Entity ID does not match route ID.");
        }

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
// Support Registries (Sindicatos, Convênios, Horários, Dependentes, Documentos)
// ============================
