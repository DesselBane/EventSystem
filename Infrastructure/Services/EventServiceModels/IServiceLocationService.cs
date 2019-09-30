using System.Threading.Tasks;
using Infrastructure.DataModel.Misc;

namespace Infrastructure.Services.EventServiceModels
{
    public interface IServiceLocationService
    {
        Task<Location> GetLocationForServiceAsync(int serviceId);
        Task<Location> UpdateLocationForServiceAsync(int serviceId, Location location);
    }
}