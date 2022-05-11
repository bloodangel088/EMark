namespace EMark.Api.Models.Responses.Journal
{
    public class JournalMarkColumn
    {
        public string Name { get; set; }
        public JournalMarkModel[] Marks { get; set; }
    }
}