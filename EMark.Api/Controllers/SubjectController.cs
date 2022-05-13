using EMark.Api.Models.Responses;
using EMark.Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;
using EMark.Api.Models.Responses.Journal;

namespace EMark.Api.Controllers
{
    public class SubjectController : ApiControllerBase
    {
        private readonly ISubjectService _subjectService;

        public SubjectController(ISubjectService subjectService)
        {
            _subjectService = subjectService;
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("{groupId}/create-subject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSubject([FromBody] SubjectModel request,[FromRoute] int groupId)
        {
            await _subjectService.CreateSubject(request, groupId);
            return NoContent();
        }

        [Authorize(Roles = "Teacher")]
        [HttpPut("update-subject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSubject([FromBody] SubjectModel request, [BindRequired] int subjectId, [BindRequired] int groupId)
        {
            await _subjectService.UpdateSubject(request, subjectId, groupId);
            return NoContent();
        }

        [AllowAnonymous]
        [HttpGet("subject-info")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<SubjectModel> SubjectInfo([BindRequired] int subjectId, [BindRequired] int groupId)
        {
            return await _subjectService.SubjectInfo(subjectId, groupId);
        }

        [Authorize(Roles = "Teacher")]
        [HttpDelete("delete-subject")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteSubject( [BindRequired] int subjectId, [BindRequired] int groupId)
        {
            await _subjectService.DeleteSubject(subjectId, groupId);
            return NoContent();
        }

        [HttpGet("{subjectId}/journal")]
        [ProducesResponseType(typeof(JournalModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<JournalModel> GetJournal([FromRoute] int subjectId)
        {
            return await _subjectService.GetJournal(subjectId);
        }
    }
}
