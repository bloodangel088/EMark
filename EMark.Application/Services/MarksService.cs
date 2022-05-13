using AutoMapper;
using EMark.Api.Models.Responses;
using EMark.Application.Exeptions;
using EMark.Application.Services.Contracts;
using EMark.DataAccess.Connection;
using EMark.DataAccess.Entities;
using Kirpichyov.FriendlyJwt.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace EMark.Application.Services
{
    public class MarksService : IMarksService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IJwtTokenReader _jwtTokenReader;

        public MarksService(DatabaseContext databaseContext, IMapper mapper, IJwtTokenReader jwtTokenReader)
        {
            _databaseContext = databaseContext;
            _jwtTokenReader = jwtTokenReader;
        }

        public async Task CreateMarkColumn(MarkColumnModel model, int subjectId)
        {
            var teacherId = int.Parse(_jwtTokenReader.UserId);
            var teacherGroups = _databaseContext.Groups.Where(group => group.TeacherGroups.Any(teacher => teacher.Id == teacherId)).Select(group => group.Id).ToArray();

            var subject = await _databaseContext.Subjects.AsNoTracking().SingleOrDefaultAsync(subject => subject.Id == subjectId && teacherGroups.Contains(subject.GroupId));
            if (subject is null)
            {
                throw new NotFoundException("Subject is not found");
            }


            bool isMarkColumnAlreadyExist = await _databaseContext.MarkColumns.AnyAsync(markColumn => markColumn.Name == model.Name && markColumn.SubjectId == subjectId);
            if (isMarkColumnAlreadyExist == true)
            {
                throw new ValidationException("MarkColumn already exists in Subject");
            }

            _databaseContext.MarkColumns.Add(new MarkColumn
            {
                Name = model.Name,
                SubjectId = subjectId,
            });

            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateMarkColumn(ColumnUpdateModel model, int subjectId, int columnId)
        {
            var teacherId = int.Parse(_jwtTokenReader.UserId);
            var teacherGroups = _databaseContext.Groups.Where(group => group.TeacherGroups.Any(teacher => teacher.Id == teacherId)).Select(group => group.Id).ToArray();

            var subject = await _databaseContext.Subjects.Include(subject => subject.MarkColumns).AsNoTracking().SingleOrDefaultAsync(subject => subject.Id == subjectId && teacherGroups.Contains(subject.GroupId));
            if (subject is null)
            {
                throw new NotFoundException("Subject is not found");
            }

            var column = subject.MarkColumns.FirstOrDefault(column => column.Id == columnId);
            if (column is null)
            {
                throw new NotFoundException("Column is not found");
            }

            column.Name = model.Name;
            _databaseContext.Update(column);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task CreateMark(MarkModel model, int studentId, int markColumnId)
        {

            var teacherId = int.Parse(_jwtTokenReader.UserId);
            var teacherGroups = _databaseContext.Groups.Where(group => group.TeacherGroups.Any(teacher => teacher.Id == teacherId)).Select(group => group.Id).ToArray();

            var markColumn = await _databaseContext.MarkColumns.AsNoTracking().SingleOrDefaultAsync(markColumn => markColumn.Id == markColumnId && teacherGroups.Contains(markColumn.Subject.GroupId));
            if (markColumn is null)
            {
                throw new NotFoundException("MarkColumn is not found");
            }

            var student = await _databaseContext.Users.OfType<Student>().AsNoTracking().SingleOrDefaultAsync(student => student.Id == studentId);
            if (student is null)
            {
                throw new NotFoundException("Student is not found");
            }

            _databaseContext.Marks.Add(new Mark
            {
                Value = model.Value,
                MarkColumnId = markColumnId,
                StudentId = studentId,
            });

            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateMark(UpdateMarkModel model, int markId)
        {
            var teacherId = int.Parse(_jwtTokenReader.UserId);
            var mark = await _databaseContext.Marks.Include(mark => mark.MarkColumn).ThenInclude(mark => mark.Subject).SingleOrDefaultAsync(mark => mark.Id == markId);

            if (mark is null)
            {
                throw new NotFoundException("Mark is not found");
            }

            if (mark.MarkColumn.Subject.TeacherId != teacherId)
            {
                throw new UnauthorizedAccessException();
            }

            mark.Value = model.Value;
            _databaseContext.Update(mark);
            await _databaseContext.SaveChangesAsync();

        }

    }
}
