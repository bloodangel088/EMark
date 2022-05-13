using EMark.Api.Models.Responses;
using FluentValidation;
using System.Net.Mail;

namespace EMark.Api.Validators
{
    public class UserUpdateValidator : AbstractValidator<UserUpdateModel>
    {
        public UserUpdateValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .Must(email => MailAddress.TryCreate(email, out _)).WithMessage("Email format is invalid")
                .MaximumLength(320);
            
            RuleFor(x => x.Firstname)
                .NotEmpty();
            
            RuleFor(x => x.Lastname)
                .NotEmpty();
            
            RuleFor(x => x.Patronymic)
                .NotEmpty();
        }
    }
}
