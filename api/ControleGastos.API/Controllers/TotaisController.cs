using ControleGastos.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ControleGastos.API.Controllers;

public class TotaisController : BaseController
{
    private readonly ITotaisService _totalService;

    public TotaisController(ITotaisService totalService)
    {
        _totalService = totalService;
    }

    // Endpoint para obter o somatório de gastos agrupados por cada pessoa cadastrada
    [HttpGet("pessoas")]
    public async Task<IActionResult> GetTotaisPorPessoa()
    {
        var result = await _totalService.GetRelatorioPessoasAsync();
        return Ok(result);
    }

    // Endpoint para obter o somatório de gastos agrupados por categorias
    [HttpGet("categorias")]
    public async Task<IActionResult> GetTotaisPorCategoria()
    {
        var result = await _totalService.GetRelatorioCategoriasAsync();
        return Ok(result);
    }
}
