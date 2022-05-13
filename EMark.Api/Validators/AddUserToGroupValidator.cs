using EMark.Api.Models.Responses;
using FluentValidation;
using System.Net.Mail;

namespace EMark.Api.Validators
{
    public class AddUserToGroupValidator : AbstractValidator<AddGroupModel>
    {
        public AddUserToGroupValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .Must(email => MailAddress.TryCreate(email, out _)).WithMessage("Email format is invalid")
                .MaximumLength(320);
        }
    }
}
