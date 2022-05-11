using System.Collections.Generic;

namespace EMark.DataAccess.Entities
{
    public class Teacher : User
    {
        public ICollection<Subject> Subjects { get; set; }

    }
}