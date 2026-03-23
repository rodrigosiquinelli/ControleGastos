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

    [HttpGet("pessoas")]
    public async Task<IActionResult> GetTotaisPorPessoa()
    {
        var result = await _totalService.GetRelatorioPessoasAsync();
        return Ok(result);
    }

    [HttpGet("categorias")]
    public async Task<IActionResult> GetTotaisPorCategoria()
    {
        var result = await _totalService.GetRelatorioCategoriasAsync();
        return Ok(result);
    }
}
