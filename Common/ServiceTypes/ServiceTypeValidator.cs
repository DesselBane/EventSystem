using System;
using System.Linq;
using System.Security.Claims;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Security.Claims;
using Infrastructure.DataModel.Service;
using Infrastructure.ErrorCodes;
using JetBrains.Annotations;

namespace Common.ServiceTypes
{
    public class ServiceTypeValidator
    {
        #region Vars

        private readonly ClaimsIdentity _identity;
        private readonly DataContext _modelContext;

        #endregion

        #region Constructors

        public ServiceTypeValidator(DataContext modelContext, ClaimsIdentity identity)
        {
            _modelContext = modelContext;
            _identity = identity;
        }

        #endregion

        public void ValidateTypeExists(int id)
        {
            if (!_modelContext.ServiceTypes.Any(x => x.Id == id))
                throw new NotFoundException(id, nameof(ServiceType), Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND));
        }

        public void ValidateTypeExists(ServiceType type)
        {
            ValidateTypeExists(type.Id);
        }

        [AssertionMethod]
        public static void ValidateNameNotEmpty(ServiceType type)
        {
            if (string.IsNullOrWhiteSpace(type.Name))
                throw new UnprocessableEntityException("Name must be set", Guid.Parse(ServiceTypeErrorCodes.TYPE_NAME_MUST_BE_SET));
        }

        [AssertionMethod]
        public static void ValidateNameNotTooLong(ServiceType type)
        {
            if (type.Name.Length > 50)
                throw new UnprocessableEntityException("Length of Name field must not exceed 50 chars", Guid.Parse(ServiceTypeErrorCodes.TYPE_NAME_TOO_LONG));
        }

        [AssertionMethod]
        public void ValidateNameDoesntExist(ServiceType type)
        {
            if (_modelContext.ServiceTypes.Any(x => x.Name == type.Name))
                throw new ConflictException("This Type already exists", Guid.Parse(ServiceTypeErrorCodes.SERVICE_TYPE_ALREADY_EXISTS));
        }

        public void CanUpdateServiceType()
        {
            if (!_identity.HasClaim(RoleClaim.ROLE_CLAIM_TYPE, ((int) RoleClaimTypes.ServiceProviderTyp_Administrator).ToString()))
                throw new ForbiddenException("User doesnt have permission to create an EventServiceProviderType", Guid.Parse(ServiceTypeErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE_TYPE));
        }

        public void CanCreateServiceType()
        {
            if (!_identity.HasClaim(RoleClaim.ROLE_CLAIM_TYPE, ((int) RoleClaimTypes.ServiceProviderTyp_Administrator).ToString()))
                throw new ForbiddenException("User doesnt have permission to create an EventServiceProviderType", Guid.Parse(ServiceTypeErrorCodes.NO_PERMISSION_TO_CREATE_SERVICE_TYPE));
        }

        public void CanDeleteServiceType()
        {
            if (!_identity.HasClaim(RoleClaim.ROLE_CLAIM_TYPE, ((int) RoleClaimTypes.ServiceProviderTyp_Administrator).ToString()))
                throw new ForbiddenException("User doesnt have permission to create an EventServiceProviderType", Guid.Parse(ServiceTypeErrorCodes.NO_PERMISSION_TO_DELETE_SERVICE_TYPE));
        }
    }
}