using AutoMapper;
using ControleGastos.Application.DTOs.Transacao;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Interfaces;
using ControleGastos.Domain.Models;

namespace ControleGastos.Application.Services
{
    public class TransacaoService : BaseService<Transacao, TransacaoDto, CreateTransacaoDto, TransacaoDto>, ITransacaoService
    {
        private readonly IPessoaRepository _pessoaRepository;
        private readonly ICategoriaRepository _categoriaRepository;

        public TransacaoService(
            ITransacaoRepository repository,
            IPessoaRepository pessoaRepo,
            ICategoriaRepository catRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper) : base(repository, unitOfWork, mapper)
        {
            _pessoaRepository = pessoaRepo;
            _categoriaRepository = catRepo;
        }

        public override async Task<TransacaoDto> CreateAsync(CreateTransacaoDto dto)
        {
            var pessoa = await _pessoaRepository.GetByIdAsync(dto.PessoaId) ?? throw new Exception("Pessoa não encontrada.");
            var categoria = await _categoriaRepository.GetByIdAsync(dto.CategoriaId) ?? throw new Exception("Categoria não encontrada.");

            var transacao = new Transacao(dto.Descricao, dto.Valor, dto.Tipo, categoria, pessoa, dto.Data);

            await _repository.AddAsync(transacao);
            await _unitOfWork.CommitAsync();

            return _mapper.Map<TransacaoDto>(transacao);
        }
    }
}
