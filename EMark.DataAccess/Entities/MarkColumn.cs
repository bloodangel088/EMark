using System.Collections.Generic;

namespace EMark.DataAccess.Entities
{
    public class MarkColumn : EntityBase<int>
    {
        public string Name { get; set; }
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }
        public ICollection<Mark> Marks { get; set; }
    }
}