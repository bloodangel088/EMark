using EMark.Api.Models.Requests;
using FluentValidation;

namespace EMark.Api.Validators
{
    public class SignInRequestValidator : AbstractValidator<UserSignInModel>
    {
        public SignInRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty();
            
            RuleFor(x => x.Password)
                .NotEmpty();
        }
    }
}
