using EMark.DataAccess.Entities.Enums;

namespace EMark.DataAccess.Entities
{
    public class User : EntityBase<int>
    {
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Patronymic { get; set; }
        public Role Role { get; set; }
    }
}