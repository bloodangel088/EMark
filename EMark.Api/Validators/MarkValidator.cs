using EMark.Api.Models.Responses;
using FluentValidation;

namespace EMark.Api.Validators
{
    public class MarkValidator : AbstractValidator<MarkModel>
    {
        public MarkValidator()
        {
            RuleFor(x => x.Value)
                .NotEmpty()
                .GreaterThanOrEqualTo(0); 
        }
    }
}
