using EMark.DataAccess.Entities;
using EMark.DataAccess.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace EMark.DataAccess.Connection
{
    public class DatabaseContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<StudentGroup> StudentGroups { get; set; }
        public DbSet<TeacherGroup> TeacherGroups { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<MarkColumn> MarkColumns { get; set; }
        public DbSet<Mark> Marks { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        
        public DatabaseContext(DbContextOptions<DatabaseContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasDiscriminator(user => user.Role)
                .HasValue<Student>(Role.Student)
                .HasValue<Teacher>(Role.Teacher);


            // Groups

            modelBuilder.Entity<Group>().ToTable("Groups");

            modelBuilder.Entity<StudentGroup>()
                .HasOne<Group>()
                .WithMany(group => group.StudentGroups);

            modelBuilder.Entity<TeacherGroup>()
                .HasOne<Group>()
                .WithMany(group => group.TeacherGroups);

            modelBuilder.Entity<StudentGroup>()
                .HasOne(studentGroup => studentGroup.Student);

            modelBuilder.Entity<TeacherGroup>()
                .HasOne(teacherGroup => teacherGroup.Teacher);

            modelBuilder.Entity<RefreshToken>().HasOne<User>().WithMany().IsRequired();
        }
    }
}