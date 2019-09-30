using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.DataModel.MapperEntities;

namespace Infrastructure.Services.Events
{
    public interface IAttendeeService
    {
        Task<AttendeeRelationship> CreateAttendeeRelationshipAsync(AttendeeRelationship relationship);
        Task DeleteAttendeeRelationshipAsync(int eventId, int attendeeId);
        Task<IEnumerable<AttendeeRelationship>> GetAllRelationshipsForEventAsync(int eventId);
        Task<AttendeeRelationship> GetSingleRelationshipAsync(int eventId, int attendeeId);
        Task<AttendeeRelationship> UpdateAttendeeRelationshipAsync(AttendeeRelationship relationship);
    }
}