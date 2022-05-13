using EMark.Api.Models.Responses;
using System.Threading.Tasks;

namespace EMark.Application.Services.Contracts
{
    public interface IMarksService
    {
        Task CreateMarkColumn(MarkColumnModel model, int subjectId);
        Task CreateMark(MarkModel model, int markColumnId, int studentId);
        Task UpdateMarkColumn(ColumnUpdateModel model, int subjectId, int columnId);
        Task UpdateMark(UpdateMarkModel model, int markId);
    }
}
