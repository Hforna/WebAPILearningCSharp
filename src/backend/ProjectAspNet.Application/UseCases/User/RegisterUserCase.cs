﻿using AutoMapper;
using ProjectAspNet.Application.UseCases.RepositoriesUseCases.User;
using ProjectAspNet.Application.Validators.User;
using ProjectAspNet.Communication.Requests;
using ProjectAspNet.Communication.Responses;
using ProjectAspNet.Domain.Entities;
using ProjectAspNet.Domain.Entities.Tokens;
using ProjectAspNet.Domain.Repositories;
using ProjectAspNet.Domain.Repositories.Security;
using ProjectAspNet.Domain.Repositories.Security.Tokens;
using ProjectAspNet.Domain.Repositories.Users;
using ProjectAspNet.Exceptions.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectAspNet.Application.UseCases.User
{
    public class RegisterUserCase : IUserCase
    {
        private readonly IMapper _mapper;
        private readonly ICryptography _password;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserAdd _userAdd;
        private readonly IUserEmailExists _userEmailExists;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public RegisterUserCase(IMapper mapper, ICryptography password, IUnitOfWork unitOfWork, IUserAdd userAdd, 
            IUserEmailExists userEmailExists, ITokenGenerator tokenGenerator, IRefreshTokenRepository refreshTokenRepository)
        {
            _mapper = mapper;
            _password = password;
            _unitOfWork = unitOfWork;
            _userAdd = userAdd;
            _userEmailExists = userEmailExists;
            _tokenGenerator = tokenGenerator;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<RegisterUserResponse> Execute(RegisterUserRequest request)
        {
            await Validate(request);

            var user = _mapper.Map<UserEntitie>(request);
            user.Password = _password.Encrypt(request.Password);
            user.UserIdentifier = Guid.NewGuid();

            await _userAdd.Add(user);
            await _unitOfWork.Commit();

            var refreshToken = new RefreshTokenEntitie()
            {
                UserId = user.Id,
            };

            if (await _refreshTokenRepository.TokenExists(user))
            {
                await _refreshTokenRepository.SaveRefreshToken(refreshToken, user);
            } else
            {
                await _refreshTokenRepository.AddRefreshToken(refreshToken);
            }

            await _unitOfWork.Commit();

            return new RegisterUserResponse() {Name = request.Name, 
                Token = new TokenResponse() { AccessToken = _tokenGenerator.Generate(user.UserIdentifier), 
                    RefreshToken = refreshToken.Value}               
            };
        }

        public async Task Validate(RegisterUserRequest request)
        {
            var validate = new UserRegisterValidate();
            var result = validate.Validate(request);
            if(await _userEmailExists.EmailExists(request.Email))
            {
                result.Errors.Add(new FluentValidation.Results.ValidationFailure(string.Empty, "This e-mail already exists"));
            }

            if(!result.IsValid)
            {
                var errorMessages = result.Errors.Select(x => x.ErrorMessage).ToList();
                throw new RegisterUserError(errorMessages);
            }
        }
    }
}
