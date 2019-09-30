using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Infrastructure.DataModel.Misc;
using Infrastructure.Interception;
using Infrastructure.Services.EventServiceModels;

namespace Common.EventServiceModels.Locations
{
    public class ServiceLocationServiceInterceptor : InterceptingMappingBase, IServiceLocationService
    {
        #region Vars

        private readonly EventServiceModelValidator _eventServiceModelValidator;

        #endregion

        #region Constructors

        public ServiceLocationServiceInterceptor(EventServiceModelValidator eventServiceModelValidator)
        {
            _eventServiceModelValidator = eventServiceModelValidator;

            BuildUp(new Dictionary<string, Action<IInvocation>>
            {
                {
                    nameof(GetLocationForServiceAsync),
                    x => GetLocationForServiceAsync((int) x.Arguments[0])
                },
                {
                    nameof(UpdateLocationForServiceAsync),
                    x => UpdateLocationForServiceAsync((int) x.Arguments[0], (Location) x.Arguments[1])
                }
            });
        }

        #endregion

        public Task<Location> GetLocationForServiceAsync(int serviceId)
        {
            _eventServiceModelValidator.ValidateEventServiceModelExists(serviceId);
            return null;
        }

        public Task<Location> UpdateLocationForServiceAsync(int serviceId, Location location)
        {
            _eventServiceModelValidator.ValidateEventServiceModelExists(serviceId);
            _eventServiceModelValidator.CanUpdateEventServiceModel(serviceId);
            return null;
        }
    }
}