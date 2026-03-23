using ControleGastos.Domain.Models;
using ControleGastos.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Infrastructure.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(ControleGastosDbContext context)
        {
            if (await context.Pessoas.AnyAsync())
            {
                return;
            }

            var categoria1 = new Categoria("Supermercado", Finalidade.Despesa);
            var categoria2 = new Categoria("Salário", Finalidade.Receita);
            var categoria3 = new Categoria("Transferências", Finalidade.Ambas);

            await context.Categorias.AddRangeAsync(categoria1, categoria2, categoria3);

            var pessoa1 = new Pessoa("João da Silva", new DateTime(1985, 5, 15));
            var pessoa2 = new Pessoa("Pedro Santos", new DateTime(2000, 10, 20));

            await context.Pessoas.AddRangeAsync(pessoa1, pessoa2);

            var transacao1 = new Transacao("Compra no supermercado", 150.00m, TipoTransacao.Despesa, categoria1, pessoa1, DateTime.Today.AddDays(-2));
            var transacao2 = new Transacao("Salário mensal", 3000.00m, TipoTransacao.Receita, categoria2, pessoa1, DateTime.Today.AddDays(-10));
            var transacao3 = new Transacao("Salário mensal", 1000.00m, TipoTransacao.Receita, categoria3, pessoa1, DateTime.Today.AddDays(-12));
            var transacao4 = new Transacao("Salário mensal", 800.00m, TipoTransacao.Despesa, categoria3, pessoa1, DateTime.Today.AddDays(-15));

            var transacao5 = new Transacao("Compra no supermercado", 250.00m, TipoTransacao.Despesa, categoria1, pessoa2, DateTime.Today.AddDays(-3));
            var transacao6 = new Transacao("Salário mensal", 3500.00m, TipoTransacao.Receita, categoria2, pessoa2, DateTime.Today.AddDays(-8));
            var transacao7 = new Transacao("Salário mensal", 1500.00m, TipoTransacao.Receita, categoria3, pessoa2, DateTime.Today.AddDays(-11));
            var transacao8 = new Transacao("Salário mensal", 850.00m, TipoTransacao.Despesa, categoria3, pessoa2, DateTime.Today.AddDays(-13));

            await context.Transacoes.AddRangeAsync(transacao1, transacao2, transacao3, transacao4, transacao5, transacao6, transacao7, transacao8);

            await context.SaveChangesAsync();
        }
    }
}
