namespace ControleGastos.Application.Interfaces
{
    /* Interface genérica que define o contrato padrão para operações de CRUD, 
     permitindo a reutilização da mesma estrutura em diferentes serviços da aplicação.*/
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
