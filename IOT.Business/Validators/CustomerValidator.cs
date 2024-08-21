using FluentValidation;
using IOT.Business.Validations;
using IOT.Entities.Models;

namespace IOT.Business.Validators
{
    public class CustomerValidator : AbstractValidator<Customer>
    {
        public CustomerValidator()
        {
            RuleFor(c => c.CustomerName).NotEmpty().WithMessage("Customer name is required.");
        }
    }
}
