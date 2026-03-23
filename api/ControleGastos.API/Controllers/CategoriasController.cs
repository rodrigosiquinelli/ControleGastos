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

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? search = null)
    {
        var result = await _categoriaService.GetAllAsync(search);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var categoria = await _categoriaService.GetByIdAsync(id);
        return categoria == null ? NotFound() : Ok(categoria);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoriaDto dto)
    {
        var categoria = await _categoriaService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = categoria.Id }, categoria);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] CategoriaDto dto)
    {
        await _categoriaService.UpdateAsync(id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _categoriaService.DeleteAsync(id);
        return NoContent();
    }
}
