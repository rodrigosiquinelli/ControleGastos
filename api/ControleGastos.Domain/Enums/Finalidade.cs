namespace ControleGastos.Domain.Enums
{
    [Flags]
    public enum Finalidade
    {
        Despesa = TipoTransacao.Despesa,
        Receita = TipoTransacao.Receita,
        Ambas = TipoTransacao.Despesa | TipoTransacao.Receita
    }
}
