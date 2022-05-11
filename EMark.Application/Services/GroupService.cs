using AutoMapper;
using EMark.Api.Models.Responses;
using EMark.Application.Services.Contracts;
using EMark.DataAccess.Connection;
using EMark.DataAccess.Entities;
using Kirpichyov.FriendlyJwt.Contracts;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using EMark.Application.Exeptions;

namespace EMark.Application.Services
{
    public class GroupService : IGroupService
    {
        private readonly DatabaseContext _databaseContext;
        private readonly IMapper _mapper;
        private readonly IJwtTokenReader _jwtTokenReader;

        public GroupService(DatabaseContext databaseContext, IMapper mapper, IJwtTokenReader jwtTokenReader)
        {
            _databaseContext = databaseContext;
            _mapper = mapper;
            _jwtTokenReader = jwtTokenReader;
        }

        public async Task CreateGroup(GroupModel groupModel)
        {
            bool isGroupAlredyExist = await _databaseContext.Groups.AsNoTracking().AnyAsync(group => group.Name == groupModel.Name);
            if (!isGroupAlredyExist)
            {
                throw new ValidationException("Group is already exist");
            }

            Group newGroup = _mapper.Map<GroupModel, Group>(groupModel);
            var teacherID = int.Parse(_jwtTokenReader.UserId);

            var teacherGroup = new TeacherGroup()
            {
                TeacherId = teacherID
            };
            newGroup.TeacherGroups.Add(teacherGroup);
            _databaseContext.Groups.Add(newGroup);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task AddTeacherToGroup(AddGroupModel model, int groupId)
        {
            var teacher = await _databaseContext.Users.AsNoTracking().OfType<Teacher>().SingleOrDefaultAsync(teacher => teacher.Email == model.Email);
            if (teacher is null)
            {
                throw new NotFoundException("Teacher not found");
            }

            var group = await _databaseContext.Groups.Include(group => group.TeacherGroups).SingleOrDefaultAsync(group => group.Id == groupId);
            if (group is null)
            {
                throw new NotFoundException("Group is not found");
            }

            bool isTeacherAlreadyInGroup = group.TeacherGroups.Any(teacherInDb=> teacherInDb.TeacherId == teacher.Id);
            if (isTeacherAlreadyInGroup)
            {
                throw new ValidationException("Teacher already in group");
            }

            group.TeacherGroups.Add(new TeacherGroup
            {
                TeacherId = teacher.Id,
            });
            
            await _databaseContext.SaveChangesAsync();
        }

        public async Task AddStudentToGroup(AddGroupModel model, int groupId)
        {
            var student = await _databaseContext.Users.AsNoTracking().OfType<Student>().SingleOrDefaultAsync(student => student.Email == model.Email);
            if (student is null)
            {
                throw new NotFoundException("Student not found");
            }

            var group = await _databaseContext.Groups.Include(group => group.TeacherGroups).SingleOrDefaultAsync(group => group.Id == groupId);
            if (group is null)
            {
                throw new NotFoundException("Group is not found");
            }

            bool isStudentAlreadyInGroup = group.StudentGroups.Any(studentInDb => studentInDb.StudentId == student.Id);
            if (!isStudentAlreadyInGroup)
            {
                throw new ValidationException("Student already in group");
            }
            
            group.StudentGroups.Add(new StudentGroup
            {
                StudentId = student.Id,
            });
           
            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteTeacherFromGroup(int groupId)
        {
            var teacherId = int.Parse(_jwtTokenReader.UserId);

            var teacherGroup = await _databaseContext.TeacherGroups
                .Include(teacherGroup => teacherGroup.Teacher)
                .ThenInclude(teacher => teacher.Subjects)
                .SingleOrDefaultAsync(teacherGroup => teacherGroup.GroupId == groupId && teacherGroup.TeacherId == teacherId);
            if (teacherGroup is null)
            {
                throw new NotFoundException("Teaher is not found");
            }

            teacherGroup.Teacher.Subjects.Clear();
            _databaseContext.TeacherGroups.Remove(teacherGroup);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteStudentFromGroup(int groupId, int studentId)
        {
            var studentGroup = await _databaseContext.StudentGroups
                .Include(student => student.Student)
                .ThenInclude(student => student.Marks)
                .SingleOrDefaultAsync(studentGroup => studentGroup.GroupId == groupId && studentGroup.StudentId == studentId);
            if (studentGroup is null)
            {
                throw new NotFoundException("Student is not found");
            }

            studentGroup.Student.Marks.Clear();
            _databaseContext.StudentGroups.Remove(studentGroup);
            await _databaseContext.SaveChangesAsync();
        }

        public async Task DeleteGroup(int groupId)
        {

        }

        public async Task UpdateGroup(GroupModel model, int groupId)
        {
            Group group = await _databaseContext.Groups.SingleAsync(group => group.Id == groupId);
            if (group is null)
            {
                throw new NotFoundException("Group is not found");
            }

            _mapper.Map(model, group);
            _databaseContext.Groups.Update(group);
            
            await _databaseContext.SaveChangesAsync();
        }

        public async Task<GroupModel> GroupInfo(int groupId)
        {
            Group group = await _databaseContext.Groups.AsNoTracking().SingleAsync(group => group.Id == groupId);
            if (group is null)
            {
                throw new NotFoundException("Group is not found");
            }

            return _mapper.Map<GroupModel>(group);
        }
    }
}
