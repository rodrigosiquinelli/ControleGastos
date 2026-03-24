using AutoMapper;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Interfaces;

namespace ControleGastos.Application.Services
{
    // Serviço base que fornece a lógica comum para os serviços, reduzindo a duplicação de código CRUD
    public abstract class BaseService<TEntity, TDto, TCreateDto, TUpdateDto> : IBaseService<TDto, TCreateDto, TUpdateDto>
        where TEntity : class
        where TDto : class
    {
        protected readonly IRepository<TEntity> _repository;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;

        protected BaseService(IRepository<TEntity> repository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // Obtém todos os registros do repositório e os converte para o formato DTO de saída
        public virtual async Task<IEnumerable<TDto>> GetAllAsync(string? search = null)
        {
            var entities = await _repository.GetAllAsync(search);
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }

        // Busca um registro específico por ID; retorna nulo se não encontrado ou o DTO mapeado
        public virtual async Task<TDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<TDto>(entity);
        }

        // Método não implementado na base; deve ser obrigatoriamente sobrescrito nas classes filhas para tratar a criação
        public virtual Task<TDto> CreateAsync(TCreateDto dto)
        {
            throw new NotImplementedException();
        }

        // Método não implementado na base; deve ser obrigatoriamente sobrescrito nas classes filhas para tratar a atualização
        public virtual Task UpdateAsync(Guid id, TUpdateDto dto)
        {
            throw new NotImplementedException();
        }

        // Remove um registro através do repositório e efetiva a transação no banco de dados via Unit of Work
        public virtual async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }
    }
}
