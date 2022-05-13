using EMark.Api.Models.Responses;
using FluentValidation;

namespace EMark.Api.Validators
{
    public class UpdateMarkValidator : AbstractValidator<UpdateMarkModel>
    {
        public UpdateMarkValidator()
        {
            RuleFor(x => x.Value)
                .NotEmpty();
        }
    }
}
