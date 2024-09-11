﻿using Moq;
using ProjectAspNet.Domain.Repositories.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectAspNet.Communication.Responses;
using ProjectAspNet.Domain.Entities;

namespace CommonTestUtilities.Repositories
{
    public class UserEmailExistsBuild
    {
        private readonly Mock<IUserEmailExists> _repository;

        public UserEmailExistsBuild() => _repository = new Mock<IUserEmailExists>();

        public void EmailExists(string email, bool result)
        {
            _repository.Setup(rep => rep.EmailExists(email)).ReturnsAsync(result);
        }

        public void Password_Email_Exists(UserEntitie user)
        {
            _repository.Setup(rep => rep.LoginByEmailAndPassword(user.Email, user.Password)).ReturnsAsync(user);
        }

        public IUserEmailExists Build() => _repository.Object;
    }
}
