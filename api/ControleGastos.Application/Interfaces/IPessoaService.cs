using ControleGastos.Application.DTOs.Pessoa;

namespace ControleGastos.Application.Interfaces
{
    public interface IPessoaService : IBaseService<PessoaDto, CreatePessoaDto, UpdatePessoaDto>
    {
    }
}
