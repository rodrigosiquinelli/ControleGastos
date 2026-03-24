using AutoMapper;
using ControleGastos.Application.DTOs.Transacao;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Interfaces;
using ControleGastos.Domain.Models;

namespace ControleGastos.Application.Services
{
    // Gerencia o ciclo de vida das transações financeiras, integrando dados de pessoas e categorias
    public class TransacaoService : BaseService<Transacao, TransacaoDto, CreateTransacaoDto, TransacaoDto>, ITransacaoService
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public TransacaoService(
            ITransacaoRepository repository,
            IPessoaRepository pessoaRepo,
            ICategoriaRepository catRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper) 
            : base(repository, unitOfWork, mapper)
        {
            _pessoaRepository = pessoaRepo;
            _categoriaRepository = catRepo;
        }

        // Sobrescreve o método de criação para validar a existência das entidades relacionadas antes de registrar a transação
        public override async Task<TransacaoDto> CreateAsync(CreateTransacaoDto dto)
        {
            // Garante a integridade buscando Pessoa e Categoria no banco de dados
            var pessoa = await _pessoaRepository.GetByIdAsync(dto.PessoaId) ?? throw new Exception("Pessoa não encontrada.");
            var categoria = await _categoriaRepository.GetByIdAsync(dto.CategoriaId) ?? throw new Exception("Categoria não encontrada.");

            // Instancia a transação vinculando os objetos completos (Pessoa e Categoria)
            var transacao = new Transacao(dto.Descricao, dto.Valor, dto.Tipo, categoria, pessoa, dto.Data);

            await _repository.AddAsync(transacao);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<TransacaoDto>(transacao);
        }
    }
}
