namespace EMark.Api.Models.Requests
{
    public class UserRegisterModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Patronymic { get; set; }
    }
}