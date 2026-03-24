using ControleGastos.Application.DTOs.Pessoa;
using ControleGastos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastos.API.Controllers;

public class PessoasController : BaseController
{
    private readonly IPessoaService _pessoaService;

    public PessoasController(IPessoaService pessoaService)
    {
        _pessoaService = pessoaService;
    }

    // Retorna a lista de todas as pessoas cadastradas.
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null)
    {
        var result = await _pessoaService.GetAllAsync(search);
        return Ok(result);
    }

    // Busca uma pessoa específica através do seu identificador único.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var pessoa = await _pessoaService.GetByIdAsync(id);
        return pessoa == null ? NotFound() : Ok(pessoa);
    }

    // Realiza o cadastro de uma nova pessoa no sistema.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePessoaDto dto)
    {
        var pessoa = await _pessoaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = pessoa.Id }, pessoa);
    }

    // Atualiza as informações de uma pessoa já existente.
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePessoaDto dto)
    {
        await _pessoaService.UpdateAsync(id, dto);
        return NoContent();
    }

    // Remove uma pessoa do sistema junto com suas transações.
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _pessoaService.DeleteAsync(id);
        return NoContent();
    }
}
