using System.Threading.Tasks;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Misc;
using Infrastructure.Services.EventServiceModels;
using Microsoft.EntityFrameworkCore;

namespace Common.EventServiceModels.Locations
{
    public class ServiceLocationService : IServiceLocationService
    {
        #region Vars

        private readonly DataContext _dataContext;

        #endregion

        #region Constructors

        public ServiceLocationService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        public async Task<Location> GetLocationForServiceAsync(int serviceId)
        {
            var serviceModel = await _dataContext.EventService
                                                 .Include(x => x.Location)
                                                 .FirstOrDefaultAsync(x => x.Id == serviceId);

            return serviceModel.Location;
        }

        public async Task<Location> UpdateLocationForServiceAsync(int serviceId, Location location)
        {
            var dbLocation = (await _dataContext.EventService
                                                .Include(x => x.Location)
                                                .FirstOrDefaultAsync(x => x.Id == serviceId)).Location;

            dbLocation.Country = location.Country;
            dbLocation.State = location.State;
            dbLocation.Street = location.Street;
            dbLocation.ZipCode = location.ZipCode;
            dbLocation.City = location.City;
            
            await _dataContext.SaveChangesAsync();
            return dbLocation;
        }
    }
}