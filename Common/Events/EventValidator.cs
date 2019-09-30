using System;
using System.Linq;
using Common.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Events;
using Infrastructure.DataModel.MapperEntities;
using Infrastructure.ErrorCodes;
using Infrastructure.Services;

namespace Common.Events
{
    public class EventValidator
    {
        #region Vars

        private readonly DataContext _eventContext;
        private readonly IPersonService _personService;

        #endregion

        #region Constructors

        public EventValidator(DataContext eventContext, IPersonService personService)
        {
            _eventContext = eventContext;
            _personService = personService;
        }

        #endregion

        public void ValidateEventExists(int eventId)
        {
            if (!_eventContext.Events.Any(x => x.Id == eventId))
                throw new NotFoundException(eventId, "Event", Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND));
        }

        public void CanReadEvent(int id)
        {
            var person = _personService.GetPersonForUserAsync().Result;

            if (!_eventContext.Events.Any(x => x.Id == id && (x.HostId == person.Id || x.AttendeeRelationships.Any(y => y.PersonId == person.Id))))
                throw new ForbiddenException("User doesnt have permission to see Event", Guid.Parse(EventErrorCodes.NO_GET_PERMISSIONS));
        }

        public void CanUpdateEvent(int id)
        {
            var person = _personService.GetPersonForUserAsync().Result;

            if (!_eventContext.Events.Any(x => x.Id == id && (x.HostId == person.Id || x.AttendeeRelationships.Any(y => y.PersonId == person.Id && y.Type == AttendeeTypes.Helper))))
                throw new ForbiddenException("User doesnt have permission to update Event", Guid.Parse(EventErrorCodes.NO_UPDATE_PERMISSIONS));
        }

        public void CanDeleteEvent(int eventId)
        {
            var person = _personService.GetPersonForUserAsync().Result;

            if (!_eventContext.Events.Any(x => x.Id == eventId && x.HostId == person.Id))
                throw new ForbiddenException("User doesnt have permission to delete Event", Guid.Parse(EventErrorCodes.NO_DELETE_PERMISSIONS));
        }

        public void ValidateEventProperties(Event eventDTO)
        {
            if (string.IsNullOrWhiteSpace(eventDTO.Name))
                throw new UnprocessableEntityException($"The '{nameof(Event.Name)}' Property is required", Guid.Parse(EventErrorCodes.NAME_REQUIRED));

            if (!eventDTO.End.IsValidSqlDate())
                throw new UnprocessableEntityException($"The '{nameof(Event.End)}' Property is no valid SqlDate", Guid.Parse(EventErrorCodes.END_DATE_INVALID));

            if (!eventDTO.Start.IsValidSqlDate())
                throw new UnprocessableEntityException($"The '{nameof(Event.Start)}' Property is no valid SqlDate", Guid.Parse(EventErrorCodes.START_DATE_INVALID));

            if(eventDTO.End < eventDTO.Start)
                throw new UnprocessableEntityException($"The '{nameof(Event.Start)}' Property must be less or equal to the '{nameof(Event.End)}' Property.",Guid.Parse(EventErrorCodes.START_MUST_BE_BEFORE_END));

            if (eventDTO.Budget.HasValue && eventDTO.Budget.Value < 0)
                throw new UnprocessableEntityException($"The '{nameof(Event.Budget)}' Property must be greater or equal to 0", Guid.Parse(EventErrorCodes.BUDGET_GREATER_OR_EQUAL_ZERO));
        }

        public void CanUpdateHost(int eventId)
        {
            var person = _personService.GetPersonForUserAsync().Result;

            if (!_eventContext.Events.Any(x => x.Id == eventId && x.HostId == person.Id))
                throw new ForbiddenException("User doesnt have permission to update Host", Guid.Parse(EventErrorCodes.NO_UPDATE_HOST_PERMISSIONS));
        }
    }
}