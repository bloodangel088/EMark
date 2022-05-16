using EMark.Api.Models.Responses;
using System.Collections.Generic;
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
        Task<IReadOnlyCollection<UserUpdateModel>> GetAllTeachersFromgroup(int groupId);
        Task<IReadOnlyCollection<UserUpdateModel>> GetAllStudentsFromgroup(int groupId);
        Task DeleteGroup(int groupId);
    }
}
