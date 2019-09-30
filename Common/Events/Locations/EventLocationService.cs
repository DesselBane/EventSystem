using System.Threading.Tasks;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Misc;
using Infrastructure.Services.Events;
using Microsoft.EntityFrameworkCore;

namespace Common.Events.Locations
{
    public class EventLocationService : IEventLocationService
    {
        #region Vars

        private readonly DataContext _dataContext;

        #endregion

        #region Constructors

        public EventLocationService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        public async Task<Location> GetLocationForEventAsync(int eventId)
        {
            var dbEvent = await _dataContext.Events.FirstOrDefaultAsync(x => x.Id == eventId);
            return await _dataContext.Locations.FirstOrDefaultAsync(x => x.Id == dbEvent.LocationId);
        }

        public async Task<Location> UpdateLocationForEventAsync(int eventId, Location location)
        {
            var dbEvent = await _dataContext.Events
                                            .Include(x => x.Location)
                                            .FirstOrDefaultAsync(x => x.Id == eventId);

            UpdateLocation(dbEvent.Location, location);
            await _dataContext.SaveChangesAsync();
            return dbEvent.Location;
        }

        public static void UpdateLocation(Location target, Location source)
        {
            target.Country = source.Country;
            target.State = source.State;
            target.Street = source.Street;
            target.ZipCode = source.ZipCode;
            target.City = source.City;
        }
    }
}