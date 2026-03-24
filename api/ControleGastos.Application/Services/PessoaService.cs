using AutoMapper;
using ControleGastos.Application.DTOs.Pessoa;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Interfaces;
using ControleGastos.Domain.Models;

namespace ControleGastos.Application.Services
{
    // Implementa a lógica específica para pessoas, estendendo a base genérica
    public class PessoaService : BaseService<Pessoa, PessoaDto, CreatePessoaDto, UpdatePessoaDto>, IPessoaService
    {
        public PessoaService(
            IPessoaRepository repository, 
            IUnitOfWork unitOfWork, 
            IMapper mapper) 
            : base(repository, unitOfWork, mapper) { }

        // Sobrescreve o método da base para realizar a criação da entidade Pessoa e persistência
        public override async Task<PessoaDto> CreateAsync(CreatePessoaDto dto)
        {
            var pessoa = new Pessoa(dto.Nome, dto.DataNascimento);
            await _repository.AddAsync(pessoa);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<PessoaDto>(pessoa);
        }

        // Sobrescreve o método de atualização para buscar a pessoa existente e aplicar as mudanças de estado
        public override async Task UpdateAsync(Guid id, UpdatePessoaDto dto)
        {
            // Valida a existência da pessoa antes de tentar qualquer alteração
            var pessoa = await _repository.GetByIdAsync(id) ?? throw new Exception("Pessoa não encontrada.");

            // Aplica as novas informações utilizando os métodos de domínio da entidade
            pessoa.SetNome(dto.Nome);
            pessoa.SetDataNascimento(dto.DataNascimento);

            await _repository.UpdateAsync(pessoa);
            await _unitOfWork.CommitAsync();
        }
    }
}
