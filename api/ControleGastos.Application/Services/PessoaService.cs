using AutoMapper;
using ControleGastos.Application.DTOs.Pessoa;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Interfaces;
using ControleGastos.Domain.Models;

namespace ControleGastos.Application.Services
{
    public class PessoaService : BaseService<Pessoa, PessoaDto, CreatePessoaDto, UpdatePessoaDto>, IPessoaService
    {
        public PessoaService(IPessoaRepository repository, IUnitOfWork unitOfWork, IMapper mapper)
            : base(repository, unitOfWork, mapper) { }

        public override async Task<PessoaDto> CreateAsync(CreatePessoaDto dto)
        {
            var pessoa = new Pessoa(dto.Nome, dto.DataNascimento);
            await _repository.AddAsync(pessoa);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<PessoaDto>(pessoa);
        }

        public override async Task UpdateAsync(Guid id, UpdatePessoaDto dto)
        {
            var pessoa = await _repository.GetByIdAsync(id) ?? throw new Exception("Pessoa não encontrada.");

            pessoa.SetNome(dto.Nome);
            pessoa.SetDataNascimento(dto.DataNascimento);

            await _repository.UpdateAsync(pessoa);
            await _unitOfWork.CommitAsync();
        }
    }
}
