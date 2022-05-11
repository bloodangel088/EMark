namespace EMark.DataAccess.Entities
{
    public class StudentGroup : EntityBase<int>
    {
        public int StudentId { get; set; }
        public int GroupId { get; set; }
        public Student Student { get; set; }

    }
}