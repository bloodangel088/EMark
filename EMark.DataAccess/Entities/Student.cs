using System.Collections.Generic;

namespace EMark.DataAccess.Entities
{
    public class Student : User
    {
        public ICollection<Mark> Marks { get; set; }
    }
}