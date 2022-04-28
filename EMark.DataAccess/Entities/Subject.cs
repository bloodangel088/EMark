using System.Collections.Generic;

namespace EMark.DataAccess.Entities
{
    public class Subject : EntityBase<int>
    {
        public string Name { get; set; }
        public int GroupId { get; set; }
        public int TeacherId { get; set; }
        public Group Group { get; set; }
        public Teacher Teacher { get; set; }
        public ICollection<MarkColumn> MarkColumns { get; set; }
    }
}