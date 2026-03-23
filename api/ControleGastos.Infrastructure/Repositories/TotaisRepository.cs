using ControleGastos.Domain.DTOs;
using ControleGastos.Domain.Enums;
using ControleGastos.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Infrastructure.Repositories
{
    public class TotaisRepository : ITotaisRepository
    {
        private readonly ControleGastosDbContext _context;

        public TotaisRepository(ControleGastosDbContext context)
        {
            _context = context;
        }

        public async Task<RelatorioPessoasGeralDto> GetRelatorioPessoasCompletoAsync()
        {
            var pessoas = await _context.Pessoas
                .Select(p => new
                {
                    p.Id,
                    p.Nome,
                    Receitas = p.Transacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => (decimal?)t.Valor) ?? 0,
                    Despesas = p.Transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => (decimal?)t.Valor) ?? 0
                })
                .Select(res => new TotaisPessoaDto
                {
                    PessoaId = res.Id,
                    Nome = res.Nome,
                    TotalReceitas = res.Receitas,
                    TotalDespesas = res.Despesas,
                    Saldo = res.Receitas - res.Despesas
                })
                .AsNoTracking()
                .ToListAsync();

            return new RelatorioPessoasGeralDto
            {
                Itens = pessoas,
                TotalGeralReceitas = pessoas.Sum(x => x.TotalReceitas),
                TotalGeralDespesas = pessoas.Sum(x => x.TotalDespesas),
                SaldoLiquidoGeral = pessoas.Sum(x => x.Saldo)
            };
        }

        public async Task<RelatorioCategoriaGeralDto> GetRelatorioCategoriasCompletoAsync()
        {
            var categorias = await _context.Categorias
                .Select(c => new
                {
                    c.Id,
                    c.Descricao,
                    Receitas = c.Transacoes.Where(t => t.Tipo == TipoTransacao.Receita).Sum(t => (decimal?)t.Valor) ?? 0,
                    Despesas = c.Transacoes.Where(t => t.Tipo == TipoTransacao.Despesa).Sum(t => (decimal?)t.Valor) ?? 0
                })
                .Select(res => new TotaisCategoriaDto
                {
                    CategoriaId = res.Id,
                    Descricao = res.Descricao,
                    TotalReceitas = res.Receitas,
                    TotalDespesas = res.Despesas,
                    Saldo = res.Receitas - res.Despesas
                })
                .AsNoTracking()
                .ToListAsync();

            return new RelatorioCategoriaGeralDto
            {
                Categorias = categorias,
                TotalGeralReceitas = categorias.Sum(x => x.TotalReceitas),
                TotalGeralDespesas = categorias.Sum(x => x.TotalDespesas),
                SaldoLiquidoGeral = categorias.Sum(x => x.Saldo)
            };
        }
    }
}
