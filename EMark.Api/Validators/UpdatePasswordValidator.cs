using EMark.Api.Models.Responses;
using FluentValidation;

namespace EMark.Api.Validators
{
    public class UpdatePasswordValidator : AbstractValidator<UpdatePasswordModel>
    {
        public UpdatePasswordValidator()
        {
            RuleFor(x => x.Password)
                .NotEmpty();
            
            RuleFor(x => x.NewPassword)
                .NotEmpty()
                .MinimumLength(8)
                .MaximumLength(32);
        }
    }
}
