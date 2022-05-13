using EMark.Api.Models.Requests;
using FluentValidation;
using System.Net.Mail;

namespace EMark.Api.Validators
{
    public class RegisterRequestValidator : AbstractValidator<UserRegisterModel>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .Must(email => MailAddress.TryCreate(email, out _)).WithMessage("Email format is invalid")
                .MaximumLength(320);
            
            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(32);
            
            RuleFor(x => x.Firstname)
                .NotEmpty();
            
            RuleFor(x => x.Lastname)
                .NotEmpty();
            
            RuleFor(x => x.Patronymic)
                .NotEmpty();

        }
    }
}
