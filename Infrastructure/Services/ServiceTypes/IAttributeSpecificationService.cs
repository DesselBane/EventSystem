using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.DataModel.ServiceAttributes;

namespace Infrastructure.Services.ServiceTypes
{
    public interface IAttributeSpecificationService
    {
        Task<ServiceAttributeSpecification> CreateAttributeSpecificationAsync(ServiceAttributeSpecification spec);
        Task<IEnumerable<ServiceAttributeSpecification>> GetAllSpecificationsForTypeAsync(int typeId);
        Task<ServiceAttributeSpecification> UpdateAttributeSpecificationAsync(ServiceAttributeSpecification spec);
        Task DeleteSpecificationAsync(int specId, int typeId);
        Task<ServiceAttributeSpecification> GetSingleSpecificationAsync(int specId, int typeId);
    }
}