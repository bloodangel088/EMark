namespace EMark.DataAccess.Entities
{
    public abstract class EntityBase<TId>
    {
        public TId Id { get; set; }
    }
}