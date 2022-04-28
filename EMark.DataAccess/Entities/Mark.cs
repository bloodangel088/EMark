namespace EMark.DataAccess.Entities
{
    public class Mark : EntityBase<int>
    {
        public int Value { get; set; }
        public int MarkColumnId { get; set; }
        public int StudentId { get; set; }
        public MarkColumn MarkColumn { get; set; }
        public Student Student { get; set; }
    }
}