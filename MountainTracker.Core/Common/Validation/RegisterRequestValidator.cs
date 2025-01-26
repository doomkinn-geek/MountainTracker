using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using MountainTracker.Core.DTO.Auth;

namespace MountainTracker.Core.Common.Validation
{
    public class RegisterRequestValidator : AbstractValidator<RegisterRequestDto>
    {
        public RegisterRequestValidator()
        {
            RuleFor(x => x.Login)
                .NotEmpty()
                .Length(4, 50);

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.Nickname)
                .NotEmpty()
                .Length(2, 50);
        }
    }
}
