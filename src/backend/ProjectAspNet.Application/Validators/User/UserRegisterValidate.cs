﻿using FluentValidation;
using ProjectAspNet.Application.SharedValidators;
using ProjectAspNet.Communication.Requests;
using ProjectAspNet.Exceptions.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAspNet.Application.Validators.User
{
    public class UserRegisterValidate : AbstractValidator<RegisterUserRequest>
    {
        public UserRegisterValidate()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(ResourceExceptMessages.NAME_EMPTY);
            RuleFor(x => x.Email).NotEmpty().WithMessage(ResourceExceptMessages.NAME_EMPTY);
            RuleFor(x => x.Password).SetValidator(new PasswordValidator<RegisterUserRequest>());
            When(user => string.IsNullOrEmpty(user.Email) == false, () =>
            {
                RuleFor(x => x.Email).EmailAddress().WithMessage(ResourceExceptMessages.EMAIL_FORMAT);
            });
        }
    }
}
