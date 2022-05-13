using EMark.Api.Models.Responses;
using FluentValidation;

namespace EMark.Api.Validators
{
    public class AddGroupValidator : AbstractValidator<GroupModel>
    {
        public AddGroupValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty()
                .MaximumLength(32);
        }
    }
}
