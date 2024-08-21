using FluentValidation;
using IOT.Business.Validations;
using IOT.Entities.Models;

namespace IOT.Business.Validators
{
    public class UserValidator : AbstractValidator<Users>
    {
        public UserValidator()
        {
            RuleFor(u => u.FirstName).NotEmpty().WithMessage("FirstName is required.");
        }
    }
}
