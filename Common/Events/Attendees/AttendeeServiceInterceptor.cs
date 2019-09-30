using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Common.People;
using Infrastructure.DataModel.MapperEntities;
using Infrastructure.Interception;
using Infrastructure.Services.Events;

namespace Common.Events.Attendees
{
    public class AttendeeServiceInterceptor : InterceptingMappingBase, IAttendeeService
    {
        #region Vars

        private readonly AttendeeValidator _attendeeValidator;
        private readonly EventValidator _eventValidator;
        private readonly PersonValidator _personValidator;

        #endregion

        #region Constructors

        public AttendeeServiceInterceptor(AttendeeValidator attendeeValidator, EventValidator eventValidator, PersonValidator personValidator)
        {
            _attendeeValidator = attendeeValidator;
            _eventValidator = eventValidator;
            _personValidator = personValidator;
            var mappings = new Dictionary<string, Action<IInvocation>>
            {
                {
                    nameof(CreateAttendeeRelationshipAsync),
                    x => CreateAttendeeRelationshipAsync((AttendeeRelationship) x.Arguments[0])
                },
                {
                    nameof(DeleteAttendeeRelationshipAsync),
                    x => DeleteAttendeeRelationshipAsync((int) x.Arguments[0], (int) x.Arguments[1])
                },
                {
                    nameof(GetAllRelationshipsForEventAsync),
                    x => GetAllRelationshipsForEventAsync((int) x.Arguments[0])
                },
                {
                    nameof(GetSingleRelationshipAsync),
                    x => GetSingleRelationshipAsync((int) x.Arguments[0], (int) x.Arguments[1])
                },
                {
                    nameof(UpdateAttendeeRelationshipAsync),
                    x => UpdateAttendeeRelationshipAsync((AttendeeRelationship) x.Arguments[0])
                }
            };

            BuildUp(mappings);
        }

        #endregion

        public Task<AttendeeRelationship> CreateAttendeeRelationshipAsync(AttendeeRelationship relationship)
        {
            _eventValidator.ValidateEventExists(relationship.EventId);
            _personValidator.ValidatePersonExists(relationship.PersonId);
            _attendeeValidator.ValidateAttendeeRelationshipDoesNotExist(relationship);
            _attendeeValidator.CanWriteRelationship(relationship);
            _attendeeValidator.ValidateAttendeeRelationshipDoesntTargetHost(relationship);
            return null;
        }

        public Task DeleteAttendeeRelationshipAsync(int eventId, int attendeeId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _personValidator.ValidatePersonExists(attendeeId);
            _attendeeValidator.ValidateAttendeeRelationshipExists(eventId, attendeeId);
            _attendeeValidator.CanDeleteRelationship(eventId, attendeeId);
            return null;
        }

        public Task<IEnumerable<AttendeeRelationship>> GetAllRelationshipsForEventAsync(int eventId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _attendeeValidator.CanReadRelationship(eventId);
            return null;
        }

        public Task<AttendeeRelationship> GetSingleRelationshipAsync(int eventId, int attendeeId)
        {
            _eventValidator.ValidateEventExists(eventId);
            _personValidator.ValidatePersonExists(attendeeId);
            _attendeeValidator.ValidateAttendeeRelationshipExists(eventId, attendeeId);
            _attendeeValidator.CanReadRelationship(eventId);
            return null;
        }

        public Task<AttendeeRelationship> UpdateAttendeeRelationshipAsync(AttendeeRelationship relationship)
        {
            _eventValidator.ValidateEventExists(relationship.EventId);
            _personValidator.ValidatePersonExists(relationship.PersonId);
            _attendeeValidator.ValidateAttendeeRelationshipExists(relationship);
            _attendeeValidator.CanWriteRelationship(relationship);
            return null;
        }
    }
}