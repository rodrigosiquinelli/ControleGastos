using ControleGastos.Application.DTOs.Transacao;

namespace ControleGastos.Application.Interfaces
{
    public interface ITransacaoService : IBaseService<TransacaoDto, CreateTransacaoDto, TransacaoDto>
    {
    }
}
