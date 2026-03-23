namespace ControleGastos.Application.Interfaces
{
    public interface IBaseService<TDto, TCreateDto, TUpdateDto>
        where TDto : class
    {
        Task<IEnumerable<TDto>> GetAllAsync(string? search = null);
        Task<TDto?> GetByIdAsync(Guid id);
        Task<TDto> CreateAsync(TCreateDto dto);
        Task UpdateAsync(Guid id, TUpdateDto dto);
        Task DeleteAsync(Guid id);
    }
}
