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

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<DadosBancariosFuncionario>> Obter(Guid funcionarioId, Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<DadosBancariosFuncionario>> Criar(
        Guid funcionarioId, [FromBody] DadosBancariosFuncionario entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Obter), new { funcionarioId, id = entity.Id }, entity);
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

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<AfastamentoFuncionario>> Obter(Guid funcionarioId, Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<AfastamentoFuncionario>> Criar(
        Guid funcionarioId, [FromBody] AfastamentoFuncionario entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Obter), new { funcionarioId, id = entity.Id }, entity);
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

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<MovimentacaoFixa>> Obter(Guid funcionarioId, Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<MovimentacaoFixa>> Criar(
        Guid funcionarioId, [FromBody] MovimentacaoFixa entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Obter), new { funcionarioId, id = entity.Id }, entity);
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
        if (string.IsNullOrWhiteSpace(entity.MesAno))
        {
            return BadRequest("MesAno is required.");
        }

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
