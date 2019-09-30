using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.DataModel.ServiceAttributes;

namespace Infrastructure.Services.EventServiceModels.ServiceAttributes
{
    public interface IServiceAttributeService
    {
        Task<ServiceAttribute> UpdateAttributeAsync(ServiceAttribute attr);
        Task<IEnumerable<ServiceAttribute>> GetAllAttributesForServiceAsync(int serviceId);
        Task<ServiceAttribute> GetSingleAttributeAsync(int serviceId, int specId);
    }
}