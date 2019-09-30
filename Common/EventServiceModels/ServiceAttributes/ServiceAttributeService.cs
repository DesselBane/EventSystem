using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Infrastructure.DataModel;
using Infrastructure.DataModel.ServiceAttributes;
using Infrastructure.Services.EventServiceModels.ServiceAttributes;
using Microsoft.EntityFrameworkCore;

namespace Common.EventServiceModels.ServiceAttributes
{
    public class ServiceAttributeService : IServiceAttributeService
    {
        private readonly DataContext _dataContext;

        public ServiceAttributeService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public async Task<ServiceAttribute> UpdateAttributeAsync(ServiceAttribute attr)
        {
            var service = await _dataContext.EventService.FirstAsync(x => x.Id == attr.EventServiceModelId);
            var dbAttribute = await _dataContext.ServiceAttributes
                                                .FirstOrDefaultAsync(x => x.EventServiceModelId == attr.EventServiceModelId
                                                                          && x.ServiceAttributeSpecificationId == attr.ServiceAttributeSpecificationId
                                                                          && x.ServiceTypeId == service.TypeId) ?? new ServiceAttribute
                                                                                                                   {
                                                                                                                       EventServiceModelId = service.Id,
                                                                                                                       ServiceAttributeSpecificationId = attr.ServiceAttributeSpecificationId,
                                                                                                                       ServiceTypeId = service.TypeId
                                                                                                                   };

            dbAttribute.Value = attr.Value;
            await _dataContext.SaveChangesAsync();
            return dbAttribute;
        }

        public async Task<IEnumerable<ServiceAttribute>> GetAllAttributesForServiceAsync(int serviceId)
        {
            var service = await _dataContext.EventService.FirstAsync(x => x.Id == serviceId);
            var specs = await _dataContext.ServiceAttributeSpecifications
                                          .Where(x => x.ServiceTypeId == service.TypeId)
                                          .ToListAsync();

            var attributes = await _dataContext.ServiceAttributes
                                               .Where(x => x.EventServiceModelId == serviceId)
                                               .ToListAsync();

            if (attributes.Count == specs.Count)
                return attributes;

            foreach (var spec in specs)
            {
                if (attributes.Any(x => x.ServiceAttributeSpecificationId == spec.Id && x.ServiceTypeId == spec.ServiceTypeId))
                    continue;
                
                attributes.Add(await CreateUndefined(serviceId,spec.Id,service.TypeId));
            }

            return attributes;
        }

        public async Task<ServiceAttribute> GetSingleAttributeAsync(int serviceId, int specId)
        {
            var service = await _dataContext.EventService.FirstAsync(x => x.Id == serviceId);

            var attr = await _dataContext.ServiceAttributes.FirstOrDefaultAsync(x => x.EventServiceModelId == serviceId
                                                                                     && x.ServiceAttributeSpecificationId == specId
                                                                                     && x.ServiceTypeId == service.TypeId);

            return attr ?? await CreateUndefined(serviceId, specId, service.TypeId);
        }


        private async Task<ServiceAttribute> CreateUndefined(int serviceId, int specId, int typeId)
        {
            var attr = new ServiceAttribute
                   {
                       EventServiceModelId = serviceId,
                       ServiceAttributeSpecificationId = specId,
                       ServiceTypeId = typeId,
                       Value = "undefined"
                   };

            _dataContext.ServiceAttributes.Add(attr);
            await _dataContext.SaveChangesAsync();

            return attr;
        }
    }
}