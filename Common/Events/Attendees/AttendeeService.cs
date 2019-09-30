using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.DataModel;
using Infrastructure.DataModel.MapperEntities;
using Infrastructure.Services.Events;
using Microsoft.EntityFrameworkCore;

namespace Common.Events.Attendees
{
    public class AttendeeService : IAttendeeService
    {
        #region Vars

        private readonly DataContext _dataContext;

        #endregion

        #region Constructors

        public AttendeeService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        #endregion

        public async Task<AttendeeRelationship> CreateAttendeeRelationshipAsync(AttendeeRelationship relationship)
        {
            var rel = new AttendeeRelationship {EventId = relationship.EventId, PersonId = relationship.PersonId, Type = relationship.Type};
            _dataContext.AttendeeRelationships.Add(rel);
            await _dataContext.SaveChangesAsync();
            return rel;
        }

        public async Task<AttendeeRelationship> UpdateAttendeeRelationshipAsync(AttendeeRelationship relationship)
        {
            var dbRelationship = await _dataContext.AttendeeRelationships.FirstOrDefaultAsync(x => x.EventId == relationship.EventId && x.PersonId == relationship.PersonId);

            dbRelationship.Type = relationship.Type;
            await _dataContext.SaveChangesAsync();
            return dbRelationship;
        }

        public async Task<IEnumerable<AttendeeRelationship>> GetAllRelationshipsForEventAsync(int eventId)
        {
            return await _dataContext.AttendeeRelationships.Where(x => x.EventId == eventId).ToListAsync();
        }

        public Task<AttendeeRelationship> GetSingleRelationshipAsync(int eventId, int attendeeId)
        {
            return _dataContext.AttendeeRelationships.FirstOrDefaultAsync(x => x.EventId == eventId && x.PersonId == attendeeId);
        }

        public async Task DeleteAttendeeRelationshipAsync(int eventId, int attendeeId)
        {
            var rel = await _dataContext.AttendeeRelationships.FirstOrDefaultAsync(x => x.EventId == eventId && x.PersonId == attendeeId);
            _dataContext.AttendeeRelationships.Remove(rel);
            await _dataContext.SaveChangesAsync();
        }
    }
}