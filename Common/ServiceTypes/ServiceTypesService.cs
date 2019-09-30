using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Service;
using Infrastructure.Services.ServiceTypes;
using Microsoft.EntityFrameworkCore;

namespace Common.ServiceTypes
{
    public class ServiceTypesService : IServiceTypeService
    {
        #region Vars

        private readonly DataContext _modelContext;

        #endregion

        #region Constructors

        public ServiceTypesService(DataContext modelContext)
        {
            _modelContext = modelContext;
        }

        #endregion

        public async Task<IEnumerable<ServiceType>> GetServiceAllTypesAsync()
        {
            return await _modelContext.ServiceTypes.ToListAsync();
        }

        public Task<ServiceType> GetSingleServiceTypeAsync(int id)
        {
            return _modelContext.ServiceTypes.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ServiceType> CreateServiceTypeAsync(ServiceType type)
        {
            var newType = new ServiceType {Name = type.Name};
            _modelContext.ServiceTypes.Add(newType);
            await _modelContext.SaveChangesAsync();
            return newType;
        }

        public async Task<ServiceType> UpdateServiceTypeAsync(ServiceType type)
        {
            var dbType = await _modelContext.ServiceTypes.FirstOrDefaultAsync(x => x.Id == type.Id);
            dbType.Name = type.Name;
            await _modelContext.SaveChangesAsync();
            return dbType;
        }

        public async Task DeleteServiceTypeAsync(int id)
        {
            var type = await _modelContext.ServiceTypes.FirstOrDefaultAsync(x => x.Id == id);
            _modelContext.ServiceTypes.Remove(type);
            await _modelContext.SaveChangesAsync();
        }
    }
}