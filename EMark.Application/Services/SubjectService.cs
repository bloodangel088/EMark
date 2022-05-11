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
    public class SubjectService : ISubjectService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;
        private readonly IJwtTokenReader _jwtTokenReader;

        public SubjectService(DatabaseContext databaseContext, IMapper mapper, IJwtTokenReader jwtTokenReader)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
            _jwtTokenReader = jwtTokenReader;
        }

        public async Task CreateSubject(SubjectModel model)
        {
            var teacherId = int.Parse(_jwtTokenReader.UserId);

            var teacher = await _databaseContext.Users.AsNoTracking().OfType<Teacher>().SingleOrDefaultAsync(teacher => teacher.Id == teacherId);
            if (teacher is null)
            {
                throw new NotFoundException("Teacher not found");
            }

            var group = await _databaseContext.Groups.AsNoTracking().SingleOrDefaultAsync(group => group.Id == model.GroupId);
            if (group is null)
            {
                throw new NotFoundException("Group is not found");
            }

            bool isSubjectAlreadyExist = await _databaseContext.Subjects.AnyAsync(subject => subject.Name == model.Name && subject.GroupId == model.GroupId);
            if (isSubjectAlreadyExist == true)
            {
                throw new ValidationException("Subject already exists in group");
            }

            bool isTeacherInGroup = await _databaseContext.Groups.AnyAsync(group => group.Id == model.GroupId && group.TeacherGroups.Any(teacher => teacher.Id == model.TeacherId));
            if (!isTeacherInGroup)
            {
                throw new ValidationException("Teacher must be in group");
            }

            _databaseContext.Subjects.Add(new Subject
            {
                Name = model.Name,
                TeacherId = model.TeacherId,
                GroupId = model.GroupId,
            });

            await _databaseContext.SaveChangesAsync();
        }

        public async Task UpdateSubject(SubjectModel model, int subjectId, int groupId)
        {
            var group = await _databaseContext.Groups.AsNoTracking().Include(group => group.Subjects).FirstOrDefaultAsync(group => group.Id == groupId);
            if (group is null)
            {
                throw new NotFoundException("Group is not found");
            }
            var subject = group.Subjects.FirstOrDefault(subject => subject.Id == subjectId);
            if (subject is null)
            {
                throw new NotFoundException("Subject is not found");
            }

            _mapper.Map(model, subject);
            _databaseContext.Subjects.Update(subject);

            await _databaseContext.SaveChangesAsync();
        }

        public async Task<SubjectModel> SubjectInfo(int subjectId, int groupId)
        {
            var group = await _databaseContext.Groups.AsNoTracking().Include(group => group.Subjects).FirstOrDefaultAsync(group => group.Id == groupId);
            if (group is null)
            {
                throw new NotFoundException("Group is not found");
            }
            var subject = group.Subjects.FirstOrDefault(subject => subject.Id == subjectId);
            if (subject is null)
            {
                throw new NotFoundException("Subject is not found");
            }

            return _mapper.Map<SubjectModel>(subject);
        }

        public async Task DeleteSubject(int subjectId, int groupId)
        {
            var teacherId = int.Parse(_jwtTokenReader.UserId);
            var subject = await _databaseContext.Subjects.FirstOrDefaultAsync(subject => subject.Id == subjectId);
            var teacherGroup = await _databaseContext.TeacherGroups
                .Include(teacherGroup => teacherGroup.Teacher)
                .ThenInclude(teacher => teacher.Subjects)
                .SingleOrDefaultAsync(teacherGroup => teacherGroup.GroupId == groupId && teacherGroup.TeacherId == teacherId);
            
            if (teacherGroup is null)
            {
                throw new NotFoundException("Teacher need be in group");
            }

            if (subject is null)
            {
                throw new NotFoundException("Subject is not found");
            }

            if (subject.TeacherId != teacherId)
            {
                throw new UnauthorizedAccessException();
            }

            _databaseContext.Subjects.Remove(subject);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<JournalModel> GetJournal(int subjectId)
        {
            var subject = await _databaseContext.Subjects
                .Include(subject =>subject.MarkColumns)
                .ThenInclude(markColumn =>markColumn.Marks)
                .AsNoTracking().FirstOrDefaultAsync(subject => subject.Id == subjectId);
            if (subject is null)
            {
                throw new NotFoundException("Subject is not found");
            }

        }
    }
}
