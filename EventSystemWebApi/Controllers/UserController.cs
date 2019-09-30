using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Security;
using Infrastructure.ErrorCodes;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace EventSystemWebApi.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,typeof(IEnumerable<User>), Description = "Returns all users this user has permission to manage")]
        public Task<IEnumerable<User>> GetManageableUsers()
        {
            return _userService.GetManageableUsersAsync();
        }

        [HttpGet("{userId}")]
        [SwaggerResponse(HttpStatusCode.OK,typeof(User))]
        [SwaggerResponse(HttpStatusCode.Forbidden,typeof(ExceptionDTO), Description = UserErrorCodes.NO_GET_PERMISSIONS + "\n\nUser doesnt have permission to see User details")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = UserErrorCodes.USER_NOT_FOUND + "\n\nUser not found")]
        public Task<User> GetSingleUser(int userId)
        {
            return _userService.GetSingleUserAsync(userId);
        }
    }
}