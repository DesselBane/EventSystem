using System;
using System.Linq;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel;
using Infrastructure.DataModel.MapperEntities;
using Infrastructure.ErrorCodes;
using Infrastructure.Services;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Common.Events.Attendees
{
    public class AttendeeValidator
    {
        #region Vars

        private readonly DataContext _dataContext;
        private readonly IPersonService _personService;

        #endregion

        #region Constructors

        public AttendeeValidator(IPersonService personService, DataContext dataContext)
        {
            _personService = personService;
            _dataContext = dataContext;
        }

        #endregion

        public void CanWriteRelationship(AttendeeRelationship relationship)
        {
            var person = _personService.GetPersonForUserAsync().Result;
            var dbevent = _dataContext.Events
                                      .Include(x => x.AttendeeRelationships)
                                      .FirstOrDefault(x => x.Id == relationship.EventId);

            if (dbevent.HostId == person.Id)
                return;

            if (relationship.Type == AttendeeTypes.Helper)
                throw new ForbiddenException("User doent have permission to add a Helper", Guid.Parse(AttendeeErrorCodes.NO_PERMISSION_TO_ADD_HELPER));

            if (dbevent.AttendeeRelationships.FirstOrDefault(x => x.PersonId == person.Id && x.Type == AttendeeTypes.Helper) == null)
                throw new ForbiddenException("User doent have permission to add a Guest", Guid.Parse(AttendeeErrorCodes.NO_PERMISSION_TO_ADD_GUEST));
        }

        public void CanReadRelationship(int eventId)
        {
            var person = _personService.GetPersonForUserAsync().Result;

            if (!_dataContext.Events.Any(x => x.Id == eventId && (x.HostId == person.Id || x.AttendeeRelationships.Any(y => y.PersonId == person.Id))))
                throw new ForbiddenException("User doesnt have permission to see Relationship", Guid.Parse(AttendeeErrorCodes.NO_PERMISSION_TO_GET_RELATIONSHIP));
        }

        public void CanDeleteRelationship(int eventId, int personId)
        {
            var relationship = _dataContext.AttendeeRelationships.FirstOrDefault(x => x.EventId == eventId && x.PersonId == personId);
            var person = _personService.GetPersonForUserAsync().Result;
            var dbevent = _dataContext.Events
                                      .Include(x => x.AttendeeRelationships)
                                      .FirstOrDefault(x => x.Id == relationship.EventId);

            if (dbevent.HostId == person.Id)
                return;

            if (relationship.Type == AttendeeTypes.Helper)
                throw new ForbiddenException("User doent have permission to add a Helper", Guid.Parse(AttendeeErrorCodes.NO_PERMISSION_TO_DELETE_RELATIONSHIP));

            if (dbevent.AttendeeRelationships.FirstOrDefault(x => x.PersonId == person.Id && x.Type == AttendeeTypes.Helper) == null)
                throw new ForbiddenException("User doent have permission to add a Guest", Guid.Parse(AttendeeErrorCodes.NO_PERMISSION_TO_DELETE_RELATIONSHIP));
        }

        [AssertionMethod]
        public void ValidateAttendeeRelationshipDoesNotExist(int personId, int eventId)
        {
            if (_dataContext.AttendeeRelationships.Any(x => x.EventId == eventId && x.PersonId == personId))
                throw new ConflictException("Person is already mapped to this event", Guid.Parse(AttendeeErrorCodes.PERSON_IS_ALREADY_MAPPED_TO_EVENT));
        }

        [AssertionMethod]
        public void ValidateAttendeeRelationshipDoesNotExist(AttendeeRelationship relationship)
        {
            ValidateAttendeeRelationshipDoesNotExist(relationship.PersonId, relationship.EventId);
        }

        [AssertionMethod]
        public void ValidateAttendeeRelationshipExists(int eventId, int personId)
        {
            if (!_dataContext.AttendeeRelationships.Any(x => x.EventId == eventId && x.PersonId == personId))
                throw new NotFoundException($"{{EventId: {eventId}, PersonId: {personId}}} ", nameof(AttendeeRelationship), Guid.Parse(AttendeeErrorCodes.RELATIONSHIP_NOT_FOUND));
        }

        [AssertionMethod]
        public void ValidateAttendeeRelationshipExists(AttendeeRelationship relationship)
        {
            ValidateAttendeeRelationshipExists(relationship.EventId, relationship.PersonId);
        }

        public void ValidateAttendeeRelationshipDoesntTargetHost(AttendeeRelationship relationship)
        {
            if (_dataContext.Events.Any(x => x.Id == relationship.EventId && x.HostId == relationship.PersonId))
                throw new ConflictException("Host cannot be added as Guest or Helper", Guid.Parse(AttendeeErrorCodes.CANNOT_ADD_HOST_AS_GUEST));
        }
    }
}