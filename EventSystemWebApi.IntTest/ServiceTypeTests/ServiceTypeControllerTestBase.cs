using System;
using System.Threading.Tasks;
using Infrastructure.DataModel.Security;
using Infrastructure.DataModel.Security.Claims;
using Infrastructure.DataModel.Service;
using Infrastructure.DataModel.ServiceAttributes;

namespace EventSystemWebApi.IntTest.ServiceTypeTests
{
    public class ServiceTypeControllerTestBase : TestBase
    {
        protected async Task<ServiceType> CreateTypeAsync()
        {
            var type = new ServiceType {Name = Guid.NewGuid().ToString()};
            _Context.ServiceTypes.Add(type);
            await _Context.SaveChangesAsync();
            return type;
        }
        
        protected async Task<User> SetupServiceTypeAdminAsync()
        {
            var user = CreateUser();
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();
            
            var serviceTypeAdminClaim = UserClaim.FromClaim(new RoleClaim(RoleClaimTypes.ServiceProviderTyp_Administrator));
            serviceTypeAdminClaim.User_Id = user.Id;
            _Context.Claims.Add(serviceTypeAdminClaim);
            await _Context.SaveChangesAsync();
            await SetupBasicAuthenticationAsync(_Client, user.EMail);
            return user;
        }

        protected ServiceAttributeSpecification CreateValidSpecification()
        {
            return new ServiceAttributeSpecification
            {
                AttributeType = Guid.NewGuid().ToString(),
                Name = Guid.NewGuid().ToString(),
                Description = Guid.NewGuid().ToString()
            };
        }

        protected async Task<ServiceAttributeSpecification> CreateValidSpecificationAsync(int typeId)
        {
            using (var ctx = CreateDataContext())
            {
                var spec = CreateValidSpecification();
                spec.ServiceTypeId = typeId;
                ctx.ServiceAttributeSpecifications.Add(spec);
                await ctx.SaveChangesAsync();
                return spec;
            }
        }

        
    }
}