using ControleGastos.Application.DTOs.Categoria;
using ControleGastos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastos.API.Controllers;

public class CategoriasController : BaseController
{
    private readonly ICategoriaService _categoriaService;

    public CategoriasController(ICategoriaService categoriaService)
    {
        _categoriaService = categoriaService;
    }

    // Retorna todas as categorias cadastradas.
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null)
    {
        var result = await _categoriaService.GetAllAsync(search);
        return Ok(result);
    }

    // Busca uma categoria específica através do seu identificador único.
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var categoria = await _categoriaService.GetByIdAsync(id);
        return categoria == null ? NotFound() : Ok(categoria);
    }

    // Realiza o cadastro de uma nova categoria.
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoriaDto dto)
    {
        var categoria = await _categoriaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
    }

    // Atualiza as informações de uma categoria existente.
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoriaDto dto)
    {
        await _categoriaService.UpdateAsync(id, dto);
        return NoContent();
    }

    // Remove uma categoria do sistema.
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _categoriaService.DeleteAsync(id);
        return NoContent();
    }
}
