using EMark.Api.Models.Responses;
using FluentValidation;

namespace EMark.Api.Validators
{
    public class MarkValidator : AbstractValidator<MarkModel>
    {
        public MarkValidator()
        {
            RuleFor(x => x.Value)
                .GreaterThanOrEqualTo(0); 
        }
    }
}
