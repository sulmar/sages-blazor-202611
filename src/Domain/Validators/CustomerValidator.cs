using Domain.Models;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Validators;

public class CustomerValidator : AbstractValidator<Customer>
{
    public CustomerValidator()
    {
        RuleFor(p => p.Name).MinimumLength(3).MaximumLength(100).NotEmpty();
        RuleFor(p => p.Email).EmailAddress().NotEmpty();
    }
}
