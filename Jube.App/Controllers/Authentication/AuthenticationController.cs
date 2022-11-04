/* Copyright (C) 2022-present Jube Holdings Limited.
 *
 * This file is part of Jube™ software.
 *
 * Jube™ is free software: you can redistribute it and/or modify it under the terms of the GNU Affero General Public License 
 * as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * Jube™ is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty  
 * of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Affero General Public License for more details.

 * You should have received a copy of the GNU Affero General Public License along with Jube™. If not, 
 * see <https://www.gnu.org/licenses/>.
 */

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using Jube.App.Code;
using Jube.App.Dto.Authentication;
using Jube.App.Validators.Authentication;
using Jube.Data.Context;
using Jube.Data.Poco;
using Jube.Data.Repository;
using Jube.Data.Security;
using Jube.Engine.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Jube.App.Controllers.Authentication
{
    [Route("api/[controller]")]
    [Produces("application/json")]
    [AllowAnonymous]
    public class AuthenticationController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;

        private readonly DbContext _dbContext;
        private readonly DynamicEnvironment.DynamicEnvironment _dynamicEnvironment;

        public AuthenticationController(DynamicEnvironment.DynamicEnvironment dynamicEnvironment,IHttpContextAccessor contextAccessor)
        {
            _dynamicEnvironment = dynamicEnvironment;
            _dbContext = DataConnectionDbContext.GetDbContextDataConnection(_dynamicEnvironment.AppSettings("ConnectionString"));
            _contextAccessor = contextAccessor;
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Close();
                _dbContext.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost("ByUserNamePassword")]
        [ProducesResponseType(typeof(AuthenticationResponseDto), (int) HttpStatusCode.OK)]
        public ActionResult<AuthenticationResponseDto> ExhaustiveSearchInstance([FromBody] AuthenticationRequestDto model)
        {
            var userRegistryRepository = new UserRegistryRepository(_dbContext);
            var validator = new AuthenticationRequestDtoValidator();
            
            var results = validator.Validate(model);
            
            if (!results.IsValid) return BadRequest();
            
            var userLogin = new UserLogin
            {
                RemoteIp = _contextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString(),
                LocalIp = _contextAccessor.HttpContext?.Connection.LocalIpAddress?.ToString()
            };
            
            var userRegistry = userRegistryRepository.GetByUserName(model.UserName);

            if (userRegistry == null)
            {
                LogLoginFailed(userLogin,model.UserName,1);
                return Unauthorized();
            }

            if (userRegistry.Active != 1)
            {
                LogLoginFailed(userLogin,userRegistry.Name,2);
                return Unauthorized();
            }

            if (userRegistry.PasswordLocked == 1)
            {
                LogLoginFailed(userLogin,userRegistry.Name,3);
                return Unauthorized();
            }

            if (!userRegistry.PasswordExpiryDate.HasValue
                || string.IsNullOrEmpty(userRegistry.Password)
                || !userRegistry.PasswordCreatedDate.HasValue)
            {
                LogLoginFailed(userLogin,userRegistry.Name,4);
                return Unauthorized();
            }

            if (!HashPassword.Verify(userRegistry.Password,
                model.Password,
                _dynamicEnvironment.AppSettings("PasswordHashingKey")))
            {
                userRegistryRepository.IncrementFailedPassword(userRegistry.Id);
                
                if (userRegistry.FailedPasswordCount > 8)
                {
                    userRegistryRepository.SetLocked(userRegistry.Id);
                }
                
                LogLoginFailed(userLogin,userRegistry.Name,5);
                
                return Unauthorized();
            }

            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                var hashedPassword = HashPassword.GenerateHash(model.NewPassword,
                    _dynamicEnvironment.AppSettings("PasswordHashingKey"));
                
                userRegistryRepository.SetPassword(userRegistry.Id,hashedPassword,DateTime.Now.AddDays(90));
            }
            else
            {
                if (!(DateTime.Now <= userRegistry.PasswordExpiryDate.Value)) return Forbid();
            }
            
            var token = Jwt.CreateToken(model.UserName,
                _dynamicEnvironment.AppSettings("JWTKey"),
                _dynamicEnvironment.AppSettings("JWTValidIssuer"),
                _dynamicEnvironment.AppSettings("JWTValidAudience")
            );

            var expiration = DateTime.Now.AddMinutes(15);
            
            var authenticationDto = new AuthenticationResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
            
            var cookieOptions = new CookieOptions
            {
                Expires = expiration,
                HttpOnly = true
            };
            
            Response.Cookies.Append("authentication",
                authenticationDto.Token,cookieOptions);
            
            LogLoginSuccess(userLogin,userRegistry.Name);

            if (userRegistry.FailedPasswordCount > 0)
            {
                userRegistryRepository.ResetFailedPasswordCount(userRegistry.Id);
            }
            
            return Ok(authenticationDto);
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        [ProducesResponseType(typeof(AuthenticationResponseDto), (int) HttpStatusCode.OK)]
        public ActionResult ChangePassword([FromBody] ChangePasswordRequestDto model)
        {
            if (User.Identity == null) return Ok();
            var userRegistryRepository = new UserRegistryRepository(_dbContext,User.Identity.Name);
            var validator = new ChangePasswordRequestDtoValidator();
            
            var results = validator.Validate(model);
            
            if (!results.IsValid) return BadRequest();
            
            var userRegistry = userRegistryRepository.GetByUserName(User.Identity.Name);
            
            if (!HashPassword.Verify(userRegistry.Password,
                model.Password,
                _dynamicEnvironment.AppSettings("PasswordHashingKey")))
            {
                return Unauthorized();
            }

            var hashedPassword = HashPassword.GenerateHash(model.NewPassword,
                _dynamicEnvironment.AppSettings("PasswordHashingKey"));
                
            userRegistryRepository.SetPassword(userRegistry.Id,hashedPassword,DateTime.Now.AddDays(90));

            return Ok();
        }

        private void LogLoginFailed(UserLogin userLogin,string createdUser,int failureTypeId)
        {
            var userLoginRepository = new UserLoginRepository(_dbContext,createdUser);
            userLogin.Failed = 1;
            userLogin.FailureTypeId = failureTypeId;
            
            if (_contextAccessor.HttpContext?.Connection.RemoteIpAddress != null)
                userLogin.LocalIp = _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            
            if (_contextAccessor.HttpContext?.Connection.LocalIpAddress != null)
                userLogin.LocalIp = _contextAccessor.HttpContext.Connection.LocalIpAddress.ToString();
            
            userLogin.UserAgent = Request.Headers["User-Agent"].ToString();
            
            userLoginRepository.Insert(userLogin);
        }

        private void LogLoginSuccess(UserLogin userLogin,string createdUser)
        {
            var userLoginRepository = new UserLoginRepository(_dbContext,createdUser);
            userLogin.Failed = 0;
            
            if (_contextAccessor.HttpContext?.Connection.RemoteIpAddress != null)
                userLogin.LocalIp = _contextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            
            if (_contextAccessor.HttpContext?.Connection.LocalIpAddress != null)
                userLogin.LocalIp = _contextAccessor.HttpContext.Connection.LocalIpAddress.ToString();

            userLogin.UserAgent = Request.Headers["User-Agent"].ToString();
            
            userLoginRepository.Insert(userLogin);
        }
    }
}