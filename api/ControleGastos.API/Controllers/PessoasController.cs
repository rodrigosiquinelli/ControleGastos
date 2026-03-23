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

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null)
    {
        var result = await _pessoaService.GetAllAsync(search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var pessoa = await _pessoaService.GetByIdAsync(id);
        return pessoa == null ? NotFound() : Ok(pessoa);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePessoaDto dto)
    {
        var pessoa = await _pessoaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = pessoa.Id }, pessoa);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePessoaDto dto)
    {
        await _pessoaService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _pessoaService.DeleteAsync(id);
        return NoContent();
    }
}
