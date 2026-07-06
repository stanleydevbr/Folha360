using Folha360.Cadastros.Domain.Abstractions;
using Folha360.Cadastros.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Folha360.Cadastros.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "Operador")]
public class ProcessosAdministrativosController : ControllerBase
{
    private readonly IProcessoAdministrativoRepository _repo;
    private readonly IRubricaProcessoRepository _rubricaProcessoRepo;

    public ProcessosAdministrativosController(
        IProcessoAdministrativoRepository repo,
        IRubricaProcessoRepository rubricaProcessoRepo)
    {
        _repo = repo;
        _rubricaProcessoRepo = rubricaProcessoRepo;
    }

    [HttpGet]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<IEnumerable<ProcessoAdministrativo>>> Listar(
        [FromQuery] Guid? empresaId, [FromQuery] string? tipo, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(1, 100, empresaId: empresaId, tipo: tipo, ct: ct);
        return Ok(items);
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<ProcessoAdministrativo>> Obter(Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<ProcessoAdministrativo>> Criar(
        [FromBody] ProcessoAdministrativo entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Obter), new { id = entity.Id }, entity);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult> Atualizar(
        Guid id, [FromBody] ProcessoAdministrativo entity, CancellationToken ct)
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

    // --- N:N Rubrica-Processo ---
    [HttpGet("{id:guid}/rubricas")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<IEnumerable<RubricaProcesso>>> ListarRubricas(
        Guid id, CancellationToken ct)
    {
        var items = await _rubricaProcessoRepo.GetByProcessoIdAsync(id, ct);
        return Ok(items);
    }

    [HttpPost("{id:guid}/rubricas")]
    public async Task<ActionResult<RubricaProcesso>> VincularRubrica(
        Guid id, [FromBody] RubricaProcesso entity, CancellationToken ct)
    {
        entity = new RubricaProcesso(entity.RubricaId, id);
        await _rubricaProcessoRepo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(ListarRubricas), new { id }, entity);
    }

    [HttpDelete("{id:guid}/rubricas/{rubricaProcessoId:guid}")]
    public async Task<IActionResult> DesvincularRubrica(
        Guid id, Guid rubricaProcessoId, CancellationToken ct)
    {
        await _rubricaProcessoRepo.RemoveAsync(rubricaProcessoId, ct);
        return NoContent();
    }
}
