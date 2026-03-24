namespace ControleGastos.Domain.Models
{
    // Classe base que fornece a identificação única (GUID) para todas as entidades do sistema
    public abstract class EntityBase
    {
        // Identificador único da entidade, gerado automaticamente na criação do objeto.
        public Guid Id { get; private set; } = Guid.NewGuid();

        protected void SetId(Guid id) => Id = id;
    }
}
