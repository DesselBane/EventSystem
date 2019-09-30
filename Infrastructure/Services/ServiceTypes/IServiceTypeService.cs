using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.DataModel.Service;

namespace Infrastructure.Services.ServiceTypes
{
    public interface IServiceTypeService
    {
        Task<IEnumerable<ServiceType>> GetServiceAllTypesAsync();
        Task<ServiceType> GetSingleServiceTypeAsync(int id);
        Task<ServiceType> CreateServiceTypeAsync(ServiceType type);
        Task<ServiceType> UpdateServiceTypeAsync(ServiceType type);
        Task DeleteServiceTypeAsync(int id);
    }
}