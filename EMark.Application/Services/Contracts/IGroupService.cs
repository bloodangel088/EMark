using EMark.Api.Models.Responses;
using System.Threading.Tasks;

namespace EMark.Application.Services.Contracts
{
    public interface IGroupService
    {
        Task CreateGroup(GroupModel model);
        Task AddTeacherToGroup(AddGroupModel model, int groupId);
        Task AddStudentToGroup(AddGroupModel model, int groupId);
        Task UpdateGroup(GroupModel model, int groupId);
        Task<GroupModel> GroupInfo(int groupId);
        Task DeleteTeacherFromGroup(int groupId);
        Task DeleteStudentFromGroup(int groupId, int studentId);
    }
}
