using ControleGastos.Application.DTOs.Transacao;
using ControleGastos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastos.API.Controllers;

public class TransacoesController : BaseController
{
    private readonly ITransacaoService _transacaoService;

    public TransacoesController(ITransacaoService transacaoService)
    {
        _transacaoService = transacaoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null)
    {
        var result = await _transacaoService.GetAllAsync(search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var transacao = await _transacaoService.GetByIdAsync(id);
        return transacao == null ? NotFound() : Ok(transacao);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransacaoDto dto)
    {
        var transacao = await _transacaoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = transacao.Id }, transacao);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _transacaoService.DeleteAsync(id);
        return NoContent();
    }
}
