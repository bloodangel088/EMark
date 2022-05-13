using EMark.Api.Models.Responses;
using FluentValidation;

namespace EMark.Api.Validators
{
    public class ColumnUpdateValidator : AbstractValidator<ColumnUpdateModel>
    {
        public ColumnUpdateValidator()
        {
            RuleFor(c => c.Name)
                .NotEmpty();
        }
    }
}
