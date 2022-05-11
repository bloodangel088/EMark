namespace EMark.Api.Models.Responses.Journal
{
    public class JournalModel
    {
        public string Name { get; set; }
        public string GroupName { get; set; }
        public string TeacherFullname { get; set; }
        public JournalMarkColumn[] Columns { get; set; }
    }
}
