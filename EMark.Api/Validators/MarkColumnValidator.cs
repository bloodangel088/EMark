using EMark.Api.Models.Responses;
using FluentValidation;

namespace EMark.Api.Validators
{
    public class MarkColumnValidator : AbstractValidator<MarkColumnModel>
    {
        public MarkColumnValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(256);
        }
    }
}
