namespace ControleGastos.Domain.Models
{
    public abstract class EntityBase
    {
        public Guid Id { get; private set; } = Guid.NewGuid();

        protected void SetId(Guid id) => Id = id;
    }
}
