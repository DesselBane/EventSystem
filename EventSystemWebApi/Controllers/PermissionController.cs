using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Security;
using Infrastructure.ErrorCodes;
using Infrastructure.Services.Permissions;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace EventSystemWebApi.Controllers
{
    [Route("api/user/{userId}/[controller]")]
    public class PermissionController : Controller
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        
        
        
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK,typeof(IEnumerable<UserClaim>))]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = UserErrorCodes.USER_NOT_FOUND + "\n\nUser could not be found")]
        [SwaggerResponse(HttpStatusCode.Forbidden,typeof(ExceptionDTO), Description = PermissionErrorCodes.NO_GET_PERMISSION + "\n\nUser doesnt have permission to see permissions of other user")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public Task<IEnumerable<UserClaim>> GetPermissionsForUser(int userId)
        {
            return _permissionService.GetPermissionsForUserAsync(userId);
        }

        [HttpGet("{claimId}")]
        [SwaggerResponse(HttpStatusCode.OK,typeof(UserClaim))]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = UserErrorCodes.USER_NOT_FOUND + "\n\nUser could not be found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = PermissionErrorCodes.CLAIM_NOT_FOUND + "\n\nClaim could not be found")]
        [SwaggerResponse(HttpStatusCode.Forbidden,typeof(ExceptionDTO), Description = PermissionErrorCodes.NO_GET_PERMISSION + "\n\nUser doesnt have permission to see permissions of other user")]
        public Task<UserClaim> GetSingleClaim(int userId, int claimId)
        {
            return _permissionService.GetSingleClaimAsync(userId, claimId);
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.OK,typeof(UserClaim))]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = UserErrorCodes.USER_NOT_FOUND + "\n\nUser could not be found")]
        [SwaggerResponse(HttpStatusCode.Forbidden,typeof(ExceptionDTO), Description = PermissionErrorCodes.NO_UPDATE_PERMISSION + "\n\nUser doesnt have permission to update the permissions of the other user")]
        [SwaggerResponse(HttpStatusCode.Conflict,typeof(ExceptionDTO), Description = PermissionErrorCodes.CLAIM_ALREADY_CREATED + "\n\nA Claim with this Type Value combination already exists")]
        [SwaggerResponse(422,typeof(ExceptionDTO),Description = PermissionErrorCodes.INVALID_CLAIM_TYPE + "\n\nInvalid claim type")]
        [SwaggerResponse(422,typeof(ExceptionDTO),Description = PermissionErrorCodes.INVALID_CLAIM_VALUE + "\n\nInvalid claim value")]
        public Task<UserClaim> CreateClaimForUser(int userId, [FromBody] UserClaim claim)
        {
            return _permissionService.CreateClaimForUserAsync(userId, claim);
        }


        [HttpDelete("{claimId}")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = UserErrorCodes.USER_NOT_FOUND + "\n\nUser could not be found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = PermissionErrorCodes.CLAIM_NOT_FOUND + "\n\nClaim could not be found")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = PermissionErrorCodes.NO_DELETE_PERMISSION + "\n\nUser doesnt have permission to delete the permissions of the other user")]
        public Task DeleteClaim(int userId, int claimId)
        {
            return _permissionService.DeleteClaimForUserAsync(userId, claimId);
        }
        
    }
}