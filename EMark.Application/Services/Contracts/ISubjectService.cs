using EMark.Api.Models.Responses;
using System.Threading.Tasks;
using EMark.Api.Models.Responses.Journal;

namespace EMark.Application.Services.Contracts
{
    public interface ISubjectService
    {
        Task CreateSubject(SubjectModel model, int groupId);
        Task UpdateSubject(SubjectModel model, int subjectId, int groupId);
        Task<SubjectModel> SubjectInfo(int subjectId, int groupId);
        Task DeleteSubject(int subjectId, int groupId);
        Task<JournalModel> GetJournal(int subjectId);
    }
}
