using EMark.Api.Models.Responses;
using EMark.Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading.Tasks;

namespace EMark.Api.Controllers
{
    public class MarksController : ApiControllerBase
    {
        private readonly IMarksService _marksService;

        public MarksController(IMarksService marksService)
        {
            _marksService = marksService;
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("{subjectId}/create-mark-column")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMarkColumn([FromBody] MarkColumnModel request, [FromRoute] int subjectId)
        {
            await _marksService.CreateMarkColumn(request, subjectId);
            return NoContent();
        }

        [Authorize(Roles = "Teacher")]
        [HttpPost("create-mark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateMark([FromBody] MarkModel request, [BindRequired] int markColumnId, [BindRequired] int studentId)
        {
            await _marksService.CreateMark(request, markColumnId,studentId);
            return NoContent();
        }

        [Authorize(Roles = "Teacher")]
        [HttpPut("update-mark-column")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMarkColumn([FromBody] ColumnUpdateModel request, [BindRequired] int subjectId, [BindRequired] int columnId)
        {
            await _marksService.UpdateMarkColumn(request, subjectId, columnId);
            return NoContent();
        }

        [Authorize(Roles = "Teacher")]
        [HttpPut("update-mark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateMark([FromBody] UpdateMarkModel request, [BindRequired] int markId)
        {
            await _marksService.UpdateMark(request, markId);
            return NoContent();
        }
    }
}
