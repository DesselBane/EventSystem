using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Infrastructure.DataModel.Service;
using Infrastructure.Interception;
using Infrastructure.Services.ServiceTypes;

namespace Common.ServiceTypes
{
    public class ServiceTypeServiceInterceptor : InterceptingMappingBase, IServiceTypeService
    {
        #region Vars

        private readonly ServiceTypeValidator _serviceTypeValidator;

        #endregion

        #region Constructors

        public ServiceTypeServiceInterceptor(ServiceTypeValidator serviceTypeValidator)
        {
            _serviceTypeValidator = serviceTypeValidator;

            BuildUp(new Dictionary<string, Action<IInvocation>>
            {
                {
                    nameof(GetServiceAllTypesAsync),
                    x => GetServiceAllTypesAsync()
                },
                {
                    nameof(GetSingleServiceTypeAsync),
                    x => GetSingleServiceTypeAsync((int) x.Arguments[0])
                },
                {
                    nameof(CreateServiceTypeAsync),
                    x => CreateServiceTypeAsync((ServiceType) x.Arguments[0])
                },
                {
                    nameof(UpdateServiceTypeAsync),
                    x => UpdateServiceTypeAsync((ServiceType) x.Arguments[0])
                },
                {
                    nameof(DeleteServiceTypeAsync),
                    x => DeleteServiceTypeAsync((int) x.Arguments[0])
                }
            });
        }

        #endregion

        public Task<IEnumerable<ServiceType>> GetServiceAllTypesAsync()
        {
            return null;
        }

        public Task<ServiceType> GetSingleServiceTypeAsync(int id)
        {
            _serviceTypeValidator.ValidateTypeExists(id);
            return null;
        }

        public Task<ServiceType> CreateServiceTypeAsync(ServiceType type)
        {
            ServiceTypeValidator.ValidateNameNotEmpty(type);
            ServiceTypeValidator.ValidateNameNotTooLong(type);
            _serviceTypeValidator.ValidateNameDoesntExist(type);

            _serviceTypeValidator.CanCreateServiceType();
            return null;
        }

        public Task<ServiceType> UpdateServiceTypeAsync(ServiceType type)
        {
            _serviceTypeValidator.ValidateTypeExists(type);

            _serviceTypeValidator.CanUpdateServiceType();

            ServiceTypeValidator.ValidateNameNotEmpty(type);
            ServiceTypeValidator.ValidateNameNotTooLong(type);
            _serviceTypeValidator.ValidateNameDoesntExist(type);
            return null;
        }

        public Task DeleteServiceTypeAsync(int id)
        {
            _serviceTypeValidator.ValidateTypeExists(id);
            _serviceTypeValidator.CanDeleteServiceType();
            return null;
        }
    }
}