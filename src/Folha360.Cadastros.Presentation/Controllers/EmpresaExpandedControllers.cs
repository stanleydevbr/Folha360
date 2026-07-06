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

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<ConfiguracaoBancariaEmpresa>> Obter(Guid empresaId, Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<ConfiguracaoBancariaEmpresa>> Criar(
        Guid empresaId, [FromBody] ConfiguracaoBancariaEmpresa entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Obter), new { empresaId, id = entity.Id }, entity);
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

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<EnderecoEmpresa>> Obter(Guid empresaId, Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<EnderecoEmpresa>> Criar(
        Guid empresaId, [FromBody] EnderecoEmpresa entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Obter), new { empresaId, id = entity.Id }, entity);
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

    [HttpGet("{id:guid}")]
    [Authorize(Policy = "Consulta")]
    public async Task<ActionResult<ContatoEmpresa>> Obter(Guid empresaId, Guid id, CancellationToken ct)
    {
        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is not null ? Ok(entity) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<ContatoEmpresa>> Criar(
        Guid empresaId, [FromBody] ContatoEmpresa entity, CancellationToken ct)
    {
        await _repo.AddAsync(entity, ct);
        return CreatedAtAction(nameof(Obter), new { empresaId, id = entity.Id }, entity);
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
