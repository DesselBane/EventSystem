using System.Threading.Tasks;
using Infrastructure.DataModel.Misc;

namespace Infrastructure.Services.Events
{
    public interface IEventLocationService
    {
        Task<Location> GetLocationForEventAsync(int eventId);
        Task<Location> UpdateLocationForEventAsync(int eventId, Location location);
    }
}