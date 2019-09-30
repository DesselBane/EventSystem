using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.DataModel.Events;
using Infrastructure.DataModel.Service;

namespace Infrastructure.Services.EventServiceModels
{
    public interface IEventServiceModelService
    {
        Task<EventServiceModel> GetServiceByIdAsync(int serviceId);
        Task<EventServiceModel> CreateEventServiceModelAsync(EventServiceModel serviceModel);
        Task<EventServiceModel> UpdateEventServiceModelAsync(EventServiceModel serviceModel);
        Task DeleteEventServiceModelAsync(int serviceModelId);
        Task<IEnumerable<EventServiceModel>> GetServiceForCurrentUser();
        Task<IEnumerable<EventServiceModel>> GetServiceByTypeIdAsync(int typeId);
        Task<IEnumerable<ServiceAgreement>> GetAgreementsForCurrentUser();
    }
}