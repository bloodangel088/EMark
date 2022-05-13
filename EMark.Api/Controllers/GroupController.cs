using EMark.Api.Models.Responses;
using EMark.Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace EMark.Api.Controllers
{
    public class GroupController : ApiControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("create-group")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateGroup([FromBody] GroupModel request)
        {
            await _groupService.CreateGroup(request);
            return NoContent();
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("{id}/add-teacher-to-group")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddTeacherToGroup([FromBody] AddGroupModel request, [FromRoute(Name = "id")] int groupId)
        {
            await _groupService.AddTeacherToGroup(request, groupId);
            return NoContent();
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("{id}/add-student-to-group")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddStudentToGroup([FromBody] AddGroupModel request, [FromRoute(Name = "id")] int groupId)
        {
            await _groupService.AddStudentToGroup(request, groupId);
            return NoContent();
        }

        [Authorize(Roles = "Teacher")]
        [HttpPut("{id}/update-group")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateGroup([FromBody] GroupModel request, [FromRoute(Name = "id")] int groupId)
        {
            await _groupService.UpdateGroup(request, groupId);
            return NoContent();
        }

        [HttpGet("{id}/group-info")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<GroupModel> GroupInfo([FromRoute(Name = "id")] int groupId)
        {
            return await _groupService.GroupInfo(groupId);
        }

        [Authorize(Roles = "Teacher")]
        [HttpDelete("{id}/teacher-leave-from-group")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteTeacherFromGroup([FromRoute(Name = "id")] int groupId)
        {
            await _groupService.DeleteTeacherFromGroup(groupId);
            return NoContent();
        }

        [Authorize(Roles = "Teacher")]
        [HttpDelete("{id}/remove-student-from-group")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteStudentFromGroup([FromRoute(Name = "id")] int groupId, [BindRequired] int studentId)
        {
            await _groupService.DeleteStudentFromGroup(groupId, studentId);
            return NoContent();
        }
    }
}
