using System;
using System.Threading.Tasks;
using Infrastructure.DataModel.Security;
using Infrastructure.DataModel.Security.Claims;
using Infrastructure.DataModel.Service;
using Infrastructure.DataModel.ServiceAttributes;

namespace EventSystemWebApi.IntTest.ServiceTests
{
    public abstract class ServiceControllerTestBase : TestBase
    {
        #region Overrides of TestBase

        protected override async Task<User> SetupAuthenticationAsync()
        {
            var user = CreateUser();
            _Context.Users.Add(user);

            var typeAdminClaim = UserClaim.FromClaim(new RoleClaim(RoleClaimTypes.ServiceProviderTyp_Administrator));

            _Context.Claims.Add(typeAdminClaim);
            user.Claims.Add(typeAdminClaim);
            await _Context.SaveChangesAsync();

            await SetupBasicAuthenticationAsync(_Client, user.EMail);

            return user;
        }

        #endregion

        

        protected async Task<ServiceAttributeSpecification> CreateAttributeSpecificationAsync(int serviceTypeId)
        {
            var spec = new ServiceAttributeSpecification
                       {
                           AttributeType = Guid.NewGuid().ToString(),
                           Name = Guid.NewGuid().ToString(),
                           Description = Guid.NewGuid().ToString(),
                           ServiceTypeId = serviceTypeId
                       };

            using (var ctx = CreateDataContext())
            {
                ctx.ServiceAttributeSpecifications.Add(spec);
                await ctx.SaveChangesAsync();
            }

            return spec;
        }

        protected async Task<ServiceAttribute> CreateSeriveAttributeAsync(int seriveId, int serviceTypeId, int specId)
        {
            var attr = new ServiceAttribute
                       {
                           EventServiceModelId = seriveId,
                           ServiceAttributeSpecificationId = specId,
                           ServiceTypeId = serviceTypeId,
                           Value = Guid.NewGuid().ToString()
                       };

            using (var ctx = CreateDataContext())
            {
                ctx.ServiceAttributes.Add(attr);
                await ctx.SaveChangesAsync();
            }

            return attr;
        }

        protected static ServiceAttribute CreateSeriveAttribute()
        {
            return new ServiceAttribute
                   {
                       Value = Guid.NewGuid().ToString(),
                       EventServiceModelId = -1,
                       ServiceAttributeSpecificationId = -1,
                       ServiceTypeId = -1
                   };
        }
    }
}