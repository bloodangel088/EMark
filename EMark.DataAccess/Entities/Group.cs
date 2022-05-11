using System.Collections.Generic;

namespace EMark.DataAccess.Entities
{
    public class Group : EntityBase<int>
    {
        public string Name { get; set; }
        public ICollection<TeacherGroup> TeacherGroups { get; set; }
        public ICollection<StudentGroup> StudentGroups { get; set; }
        public ICollection<Subject> Subjects { get; set; }
        public Group()
        {
            TeacherGroups = new List<TeacherGroup>();
            StudentGroups = new List<StudentGroup>();
            Subjects = new List<Subject>();
        }
    }
}