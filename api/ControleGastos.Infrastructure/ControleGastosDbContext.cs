using ControleGastos.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Infrastructure
{
    // Classe de contexto do Entity Framework Core que coordena a funcionalidade de banco de dados para os modelos de domínio.
    public class ControleGastosDbContext : DbContext
    {
        public ControleGastosDbContext(DbContextOptions<ControleGastosDbContext> options) : base(options) { }

        public DbSet<Pessoa> Pessoas { get; set; } = null!;
        public DbSet<Categoria> Categorias { get; set; } = null!;
        public DbSet<Transacao> Transacoes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Chaves Primárias
            modelBuilder.Entity<Pessoa>().HasKey(p => p.Id);
            modelBuilder.Entity<Categoria>().HasKey(c => c.Id);
            modelBuilder.Entity<Transacao>().HasKey(t => t.Id);

            // Configurações de Tamanho
            modelBuilder.Entity<Pessoa>().Property(p => p.Nome).HasMaxLength(200).IsRequired();
            modelBuilder.Entity<Categoria>().Property(c => c.Descricao).HasMaxLength(400).IsRequired();
            modelBuilder.Entity<Transacao>().Property(t => t.Descricao).HasMaxLength(400).IsRequired();

            // Relacionamento Pessoa -> Transacoes (Cascade Delete)
            modelBuilder.Entity<Transacao>()
                .HasOne(t => t.Pessoa)
                .WithMany(p => p.Transacoes)
                .HasForeignKey(t => t.PessoaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento Categoria -> Transacoes (Restrict)
            modelBuilder.Entity<Transacao>()
                .HasOne(t => t.Categoria)
                .WithMany(c => c.Transacoes)
                .HasForeignKey(t => t.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração de Data
            modelBuilder.Entity<Transacao>()
                .Property(t => t.Data)
                .HasColumnType("date")
                .IsRequired();

            // Índices para busca rápida
            modelBuilder.Entity<Pessoa>().HasIndex(p => p.Nome);
            modelBuilder.Entity<Categoria>().HasIndex(c => c.Descricao);
        }
    }
}
