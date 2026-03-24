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

    // Retorna a lista de todas as transações.
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null)
    {
        var result = await _transacaoService.GetAllAsync(search);
        return Ok(result);
    }

    // Busca uma transação específica pelo seu identificador único.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var transacao = await _transacaoService.GetByIdAsync(id);
        return transacao == null ? NotFound() : Ok(transacao);
    }

    // Registra uma nova transação financeira vinculando pessoa e categoria.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransacaoDto dto)
    {
        var transacao = await _transacaoService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = transacao.Id }, transacao);
    }

    // Remove um registro de transação do sistema.
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _transacaoService.DeleteAsync(id);
        return NoContent();
    }
}
