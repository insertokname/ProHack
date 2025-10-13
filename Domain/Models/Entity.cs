namespace Domain.Models
{
    public class Entity
    {
        public Guid Id { get; set; }

        public Entity() { Id = Guid.NewGuid(); }
        public Entity(Guid id) { Id = id; }
    }
}