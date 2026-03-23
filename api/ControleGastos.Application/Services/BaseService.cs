using AutoMapper;
using ControleGastos.Application.Interfaces;
using ControleGastos.Domain.Interfaces;

namespace ControleGastos.Application.Services
{
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

        public virtual async Task<IEnumerable<TDto>> GetAllAsync(string? search = null)
        {
            var entities = await _repository.GetAllAsync(search);
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }

        public virtual async Task<TDto?> GetByIdAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<TDto>(entity);
        }

        public virtual Task<TDto> CreateAsync(TCreateDto dto)
        {
            throw new NotImplementedException();
        }

        public virtual Task UpdateAsync(Guid id, TUpdateDto dto)
        {
            throw new NotImplementedException();
        }

        public virtual async Task DeleteAsync(Guid id)
        {
            await _repository.DeleteAsync(id);
            await _unitOfWork.CommitAsync();
        }
    }
}
