using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.DataModel;
using Infrastructure.DataModel.ServiceAttributes;
using Infrastructure.Services.ServiceTypes;
using Microsoft.EntityFrameworkCore;

namespace Common.ServiceTypes.ServiceAttributes
{
    public class AttributeSpecificationService : IAttributeSpecificationService
    {
        private readonly DataContext _dataContext;

        public AttributeSpecificationService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<ServiceAttributeSpecification> CreateAttributeSpecificationAsync(ServiceAttributeSpecification spec)
        {
            var dbSpec = new ServiceAttributeSpecification
            {
                AttributeType = spec.AttributeType,
                Description = spec.Description,
                Name = spec.Name,
                ServiceTypeId = spec.ServiceTypeId
            };

            _dataContext.ServiceAttributeSpecifications.Add(dbSpec);
            await _dataContext.SaveChangesAsync();
            return dbSpec;
        }

        public async Task<IEnumerable<ServiceAttributeSpecification>> GetAllSpecificationsForTypeAsync(int typeId)
        {
            return await _dataContext.ServiceAttributeSpecifications
                .Where(x => x.ServiceTypeId == typeId)
                .ToListAsync();
        }

        public async Task<ServiceAttributeSpecification> UpdateAttributeSpecificationAsync(ServiceAttributeSpecification spec)
        {
            var dbSpec = await _dataContext.ServiceAttributeSpecifications.FirstAsync(x => x.Id == spec.Id && x.ServiceTypeId == spec.ServiceTypeId);

            dbSpec.AttributeType = spec.AttributeType;
            dbSpec.Description = spec.Description;
            dbSpec.Name = spec.Name;

            await _dataContext.SaveChangesAsync();
            return dbSpec;
        }

        public async Task DeleteSpecificationAsync(int specId, int typeId)
        {
            var spec = await _dataContext.ServiceAttributeSpecifications.FirstAsync(x => x.Id == specId && x.ServiceTypeId == typeId);
            _dataContext.ServiceAttributeSpecifications.Remove(spec);
            await _dataContext.SaveChangesAsync();
        }

        public Task<ServiceAttributeSpecification> GetSingleSpecificationAsync(int specId, int typeId)
        {
            return _dataContext.ServiceAttributeSpecifications.FirstOrDefaultAsync(x => x.Id == specId && x.ServiceTypeId == typeId);
        }
    }
}