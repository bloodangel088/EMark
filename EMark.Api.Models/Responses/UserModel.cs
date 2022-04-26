using EMark.Api.Models.Enums;

namespace EMark.Api.Models.Responses
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Patronymic { get; set; }
        public RoleModel Role { get; set; }
    }
}