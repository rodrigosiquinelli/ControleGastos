using AutoMapper;
using ControleGastos.Application.DTOs.Categoria;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Interfaces;
using ControleGastos.Domain.Models;

namespace ControleGastos.Application.Services
{
    // Implementa a lógica específica para categorias, estendendo a base genérica
    public class CategoriaService : BaseService<Categoria, CategoriaDto, CreateCategoriaDto, CategoriaDto>, ICategoriaService
    {
        private readonly ITransacaoRepository _transacaoRepository;

        public CategoriaService(
            ICategoriaRepository repository,
            ITransacaoRepository transacaoRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper)
            : base(repository, unitOfWork, mapper)
        {
            _transacaoRepository = transacaoRepository;
        }

        // Sobrescreve o método da base para realizar a criação da entidade Categoria e persistência
        public override async Task<CategoriaDto> CreateAsync(CreateCategoriaDto dto)
        {
            var categoria = new Categoria(dto.Descricao, dto.Finalidade);
            await _repository.AddAsync(categoria);
            await _unitOfWork.CommitAsync();
            return _mapper.Map<CategoriaDto>(categoria);
        }

        // Sobrescreve o método da base para incluir validações de negócio antes de atualizar a categoria
        public override async Task UpdateAsync(Guid id, CategoriaDto dto)
        {
            var categoria = await _repository.GetByIdAsync(id) ?? throw new Exception("Categoria não encontrada.");

            // Verifica se houve alteração de finalidade e se o registro possui vínculos que impeçam a mudança
            if (categoria.Finalidade != dto.Finalidade)
            {
                var temTransacoes = await _transacaoRepository.ExisteTransacaoComCategoriaAsync(id);

                if (temTransacoes)
                {
                    throw new InvalidOperationException("Não é possível alterar a finalidade de categorias que possuem transações vinculadas.");
                }
            }

            // Aplica as novas informações utilizando os métodos de domínio da entidade
            categoria.SetDescricao(dto.Descricao);
            categoria.SetFinalidade(dto.Finalidade);

            await _repository.UpdateAsync(categoria);
            await _unitOfWork.CommitAsync();
        }
    }
}
