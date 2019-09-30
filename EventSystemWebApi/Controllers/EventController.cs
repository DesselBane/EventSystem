using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Events;
using Infrastructure.DataModel.MapperEntities;
using Infrastructure.DataModel.Misc;
using Infrastructure.ErrorCodes;
using Infrastructure.Services.Events;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace EventSystemWebApi.Controllers
{
    [Route("api/[controller]")]
    public class EventController : Controller
    {
        #region Vars

        private readonly IAttendeeService _attendeeService;
        private readonly IEventLocationService _eventLocationService;
        private readonly IAgreementService _agreementService;

        private readonly IEventService _eventService;
        private readonly IServiceSlotService _serviceSlotService;
        

        #endregion

        #region Constructors

        public EventController(IEventService eventService, IServiceSlotService serviceSlotService, IAttendeeService attendeeService, IEventLocationService eventLocationService, IAgreementService agreementService)
        {
            _eventService = eventService;
            _serviceSlotService = serviceSlotService;
            _attendeeService = attendeeService;
            _eventLocationService = eventLocationService;
            _agreementService = agreementService;
        }

        #endregion

        #region Simple Event

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.Created, typeof(Event))]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = EventErrorCodes.START_DATE_INVALID + "\n\nStart date is invalid")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + EventErrorCodes.END_DATE_INVALID + "\n\nEnd date is invalid")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + EventErrorCodes.BUDGET_GREATER_OR_EQUAL_ZERO + "\n\nBudget must be 0 or greater")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + EventErrorCodes.NAME_REQUIRED + "\n\nName is required")]
        [SwaggerResponse(422,typeof(ExceptionDTO), Description = "\n" + EventErrorCodes.START_MUST_BE_BEFORE_END + "\n\nStart must be before end")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<Event> CreateEvent([FromBody] Event newEvent)
        {
            HttpContext.Response.StatusCode = (int) HttpStatusCode.Created;
            return _eventService.CreateEventAsync(newEvent);
        }

        [HttpPost("{eventId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Event))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent doesnt Exist")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = EventErrorCodes.NO_UPDATE_PERMISSIONS + "\n\nUser doesnt have permission to update Event")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = EventErrorCodes.START_DATE_INVALID + "\n\nStart date is invalid")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + EventErrorCodes.END_DATE_INVALID + "\n\nEnd date is invalid")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + EventErrorCodes.BUDGET_GREATER_OR_EQUAL_ZERO + "\n\nBudget must be 0 or greater")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + EventErrorCodes.NAME_REQUIRED + "\n\nName is required")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + EventErrorCodes.START_MUST_BE_BEFORE_END + "\n\nStart must be before end")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<Event> UpdateEvent([FromBody] Event updatedEvent, int eventId)
        {
            updatedEvent.Id = eventId;
            return _eventService.UpdateEventAsync(updatedEvent);
        }

        [HttpDelete("{eventId}", Name = nameof(DeleteEvent))]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = EventErrorCodes.NO_DELETE_PERMISSIONS + "\n\nUser doesnt have permission to delete Event")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nNo Event with this Id exists")]
        public virtual Task DeleteEvent(int eventId)
        {
            return _eventService.DeleteEventAsync(eventId);
        }

        [HttpGet("{eventId}", Name = nameof(GetEvent))]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Event))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent doesnt Exist")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = EventErrorCodes.NO_GET_PERMISSIONS + "\n\nUser doesnt have permission to view Event Details")]
        public virtual Task<Event> GetEvent(int eventId)
        {
            return _eventService.GetEventAsync(eventId);
        }

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<Event>))]
        public virtual Task<IEnumerable<Event>> GetEventOverview()
        {
            return _eventService.GetAllEventsForCurrentUserAsync();
            
        }

        [HttpPost("{eventId}/updateHost/{newHostId}")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent doesnt Exist")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + PersonErrorCodes.PERSON_NOT_FOUND + "\n\nNew Host doesnt Exist")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = EventErrorCodes.NO_UPDATE_HOST_PERMISSIONS + "\n\nUser doesnt have permission to update Host")]
        public virtual Task UpdateHost(int eventId, int newHostId)
        {
            return _eventService.UpdateHostAsync(eventId, newHostId);
        }

        #endregion

        #region Service Provider Slot

        [HttpPut("{eventId}/sps")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(ServiceSlot))]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = EventErrorCodes.NO_UPDATE_PERMISSIONS + "\n\nUser doesnt have Permission to add a SPS to this event")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nCorresponding Event doesnt exist")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND + "\n\nCorresponding SP Type doesnt exist")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = ServiceSlotErrorCodes.START_MUSE_BE_BEFORE_END + "\n\nStart must be earlier or equal to End")]
        [SwaggerResponse(422,typeof(ExceptionDTO), Description = "\n" + ServiceSlotErrorCodes.START_DATE_INVALID + "\n\nStart is no valid SqlDate")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + ServiceSlotErrorCodes.END_DATE_INVALID + "\n\nEnd is no valid SqlDate")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<ServiceSlot> AddServiceProviderSlot([FromBody] ServiceSlot slot, int eventId)
        {
            HttpContext.Response.StatusCode = (int) HttpStatusCode.Created;
            slot.EventId = eventId;
            
            //TODO test this !!!!
            slot.Id = 0;
            return _serviceSlotService.AddServiceSlotAsync(slot);
        }

        [HttpPost("{eventId}/sps/{spsId}")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = EventErrorCodes.NO_UPDATE_PERMISSIONS + "\n\nUser doesnt have Permission to update this SPS")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nCorresponding Event doesnt exist")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND + "\n\nCorresponding SP Type doesnt exist")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND + "\n\nSPS doesnt exist")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = ServiceSlotErrorCodes.START_MUSE_BE_BEFORE_END + "\n\nStart must be earlier or equal to End")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + ServiceSlotErrorCodes.START_DATE_INVALID + "\n\nStart is no valid SqlDate")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + ServiceSlotErrorCodes.END_DATE_INVALID + "\n\nEnd is no valid SqlDate")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task UpdateServiceProviderSlot(int eventId, int spsId, [FromBody] ServiceSlot slot)
        {
            slot.Id = spsId;
            slot.EventId = eventId;
            return _serviceSlotService.UpdateServiceSlotAsync(slot);
        }

        [HttpDelete("{eventId}/sps/{spsId}", Name = nameof(DeleteServiceProviderSlot))]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = EventErrorCodes.NO_UPDATE_PERMISSIONS + "\n\nUser doesnt have Permission to delete this SPS")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent doesnt exist")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND + "\n\nSPS doesnt exist")]
        public virtual Task DeleteServiceProviderSlot(int spsId, int eventId)
        {
            return _serviceSlotService.DeleteServiceSlotAsync(spsId, eventId);
        }

        [HttpGet("{eventId}/sps/{spsId}")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = EventErrorCodes.NO_GET_PERMISSIONS + "\n\nUser doesnt have Permission to see this SPS")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent doesnt exist")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND + "\n\nSPS doesnt exist")]
        public virtual Task<ServiceSlot> GetServiceProviderSlot(int spsId, int eventId)
        {
            return _serviceSlotService.GetServiceSlotAsync(spsId, eventId);
        }

        [HttpGet("{eventId}/sps")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<ServiceSlot>))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent doesnt exist")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = EventErrorCodes.NO_GET_PERMISSIONS + "\n\nUser doesnt have Permission to see this SPS")]
        public Task<IEnumerable<ServiceSlot>> GetAllServiceProviderSlotForEvent(int eventId)
        {
            return _serviceSlotService.GetAllServiceSlotsForEventAsync(eventId);
        }

        #endregion Service Provider Slot

        #region Service Agreement

        [HttpPut("{eventId}/sps/{spsId}/request/{serviceId}")]
        [SwaggerResponse(HttpStatusCode.OK,typeof(ServiceAgreement))]
        [SwaggerResponse(HttpStatusCode.Forbidden,typeof(ExceptionDTO), Description = EventErrorCodes.NO_UPDATE_PERMISSIONS + "\n\nUser doesnt have Permission to add ServiceAgreement")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent not found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND + "\n\nService Slot not found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceErrorCodes.SERVICE_NOT_FOUND + "\n\nService not found")]
        [SwaggerResponse(HttpStatusCode.Conflict,typeof(ExceptionDTO), Description = ServiceAgreementErrorCodes.AGREEMENT_ALREADY_EXISTS + "\n\nThis Slot already contains an Agreement")]
        public Task<ServiceAgreement> CreateServiceAgreement(int eventId, int spsId,int serviceId)
        {
            return _agreementService.CreateServiceAgreement(eventId, spsId, serviceId);
        }

        [HttpGet("{eventId}/sps/{spsId}/agreement")]
        [SwaggerResponse(HttpStatusCode.OK,typeof(ServiceAgreement))]
        [SwaggerResponse(HttpStatusCode.Forbidden,typeof(ExceptionDTO), Description = EventErrorCodes.NO_GET_PERMISSIONS + "\n\nUser doesnt have permission to see ServiceAgreement")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent not found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND + "\n\nService Slot not found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceAgreementErrorCodes.AGREEMENT_NOT_FOUND + "\n\nService Agreement not found")]
        public Task<ServiceAgreement> GetServiceAgreement(int eventId, int spsId)
        {
            return _agreementService.GetSingleServiceAgreementByEventSlot(eventId, spsId);
        }

        [HttpGet("{eventId}/agreements")]
        [SwaggerResponse(HttpStatusCode.OK,typeof(IEnumerable<ServiceAgreement>))]
        [SwaggerResponse(HttpStatusCode.Forbidden,typeof(ExceptionDTO), Description = EventErrorCodes.NO_GET_PERMISSIONS + "\n\nUser doesnt have permission to see ServiceAgreements")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent not found")]
        public Task<IEnumerable<ServiceAgreement>> GetAllServiceAgreementsForEvent(int eventId)
        {
            return _agreementService.GetAllServiceAgreementsByEvent(eventId);
        }

        [HttpPost("{eventId}/sps/{spsId}/agreement")]
        [SwaggerResponse(HttpStatusCode.OK,typeof(ServiceAgreement))]
        [SwaggerResponse(HttpStatusCode.Forbidden,typeof(ExceptionDTO), Description = ServiceAgreementErrorCodes.NO_UPDATE_PERMISSIONS + "\n\nUser doesnt have Permission to update ServiceAgreement")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent not found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND + "\n\nService Slot not found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceAgreementErrorCodes.AGREEMENT_NOT_FOUND + "\n\nService Agreement not found")]
        [SwaggerResponse(422,typeof(ExceptionDTO), Description = ServiceAgreementErrorCodes.AGREEMENT_START_BEFORE_END + "\n\nStart Date must be earlier then end Date")]
        [SwaggerResponse(HttpStatusCode.Conflict,typeof(ExceptionDTO), Description = ServiceAgreementErrorCodes.CANNOT_UPDATE_VALUES + "\n\nCan only update values while Agreement is in Request State")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<ServiceAgreement> UpdateServiceAgreement(int eventId, int spsId, [FromBody] ServiceAgreement agreement)
        {
            agreement.EventId = eventId;
            agreement.ServiceSlotId = spsId;
            return _agreementService.UpdateServiceAgreement(agreement);
        }

        [HttpPost("{eventId}/sps/{spsId}/agreement/proposal")]
        [SwaggerResponse(HttpStatusCode.Forbidden,typeof(ExceptionDTO), Description = ServiceAgreementErrorCodes.NO_UPDATE_PERMISSIONS + "\n\nUser doesnt have Permission to update ServiceAgreement")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent not found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND + "\n\nService Slot not found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceAgreementErrorCodes.AGREEMENT_NOT_FOUND + "\n\nService Agreement not found")]
        [SwaggerResponse(HttpStatusCode.Conflict,typeof(ExceptionDTO), Description = ServiceAgreementErrorCodes.INVALID_STATE_TRAVERSAL + "\n\nCannot set State to Proposal")]
        public Task<ServiceAgreement> MakeAgreementProposal(int eventId, int spsId)
        {
            return _agreementService.ProposeAgreement(eventId, spsId);
        }

        [HttpPost("{eventId}/sps/{spsId}/agreement/accept")]
        [SwaggerResponse(HttpStatusCode.Forbidden,typeof(ExceptionDTO), Description = ServiceAgreementErrorCodes.NO_UPDATE_PERMISSIONS + "\n\nUser doesnt have Permission to update ServiceAgreement")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent not found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND + "\n\nService Slot not found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceAgreementErrorCodes.AGREEMENT_NOT_FOUND + "\n\nService Agreement not found")]
        [SwaggerResponse(HttpStatusCode.Conflict,typeof(ExceptionDTO), Description = ServiceAgreementErrorCodes.INVALID_STATE_TRAVERSAL + "\n\nCannot set State to Accepted")]
        public Task<ServiceAgreement> AcceptAgreement(int eventId, int spsId)
        {
            return _agreementService.AcceptAgreement(eventId, spsId);
        }

        [HttpPost("{eventId}/sps/{spsId}/agreement/decline")]
        [SwaggerResponse(HttpStatusCode.Forbidden,typeof(ExceptionDTO), Description = ServiceAgreementErrorCodes.NO_UPDATE_PERMISSIONS + "\n\nUser doesnt have Permission to update ServiceAgreement")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent not found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND + "\n\nService Slot not found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceAgreementErrorCodes.AGREEMENT_NOT_FOUND + "\n\nService Agreement not found")]
        public Task<ServiceAgreement> DeclineAgreement(int eventId, int spsId)
        {
            return _agreementService.DeclineAgreement(eventId,spsId);
        }

        [HttpDelete("{eventId}/sps/{spsId}/agreement")]
        [SwaggerResponse(HttpStatusCode.Forbidden,typeof(ExceptionDTO), Description = EventErrorCodes.NO_DELETE_PERMISSIONS + "\n\nUser doesnt have Permission to delete ServiceAgreement")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent not found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceSlotErrorCodes.SERVICE_SLOT_NOT_FOUND + "\n\nService Slot not found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceAgreementErrorCodes.AGREEMENT_NOT_FOUND + "\n\nAgreement not found")]
        public Task DeleteServiceAgreement(int eventId, int spsId)
        {
            return _agreementService.DeleteAgreement(eventId, spsId);
        }

        #endregion Service Agreement
        
        #region Attendees

        [HttpPut("{eventId}/attendees/{attendeeId}")]
        [SwaggerResponse(HttpStatusCode.Created, typeof(AttendeeRelationship))]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = AttendeeErrorCodes.NO_PERMISSION_TO_ADD_GUEST + "\n\nUser dosent have permission to add a Guest")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = "\n" + AttendeeErrorCodes.NO_PERMISSION_TO_ADD_HELPER + "\n\nUser dosent have permission to add a Helper")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent doest exist")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + PersonErrorCodes.PERSON_NOT_FOUND + "\n\nPerson could not be found")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(ExceptionDTO), Description = AttendeeErrorCodes.PERSON_IS_ALREADY_MAPPED_TO_EVENT + "\n\nPerson is already mapped to Event")]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(ExceptionDTO), Description = "\n" + AttendeeErrorCodes.CANNOT_ADD_HOST_AS_GUEST + "\n\nCannot add Host as Guest")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<AttendeeRelationship> CreateGuestRelationship(int eventId, int attendeeId, [FromBody] AttendeeRelationship attendee)
        {
            HttpContext.Response.StatusCode = (int) HttpStatusCode.Created;
            attendee.EventId = eventId;
            attendee.PersonId = attendeeId;
            return _attendeeService.CreateAttendeeRelationshipAsync(attendee);
        }

        [HttpPost("{eventId}/attendees/{attendeeId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(AttendeeRelationship))]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = AttendeeErrorCodes.NO_PERMISSION_TO_ADD_GUEST + "\n\nUser dosent have permission to add a Guest")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = "\n" + AttendeeErrorCodes.NO_PERMISSION_TO_ADD_HELPER + "\n\nUser dosent have permission to add a Helper")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent doest exist")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + PersonErrorCodes.PERSON_NOT_FOUND + "\n\nPerson could not be found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + AttendeeErrorCodes.RELATIONSHIP_NOT_FOUND + "\n\nRaltionship could not be found")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<AttendeeRelationship> UpdateGuestRelationsship([FromBody] AttendeeRelationship attendee, int eventId, int attendeeId)
        {
            attendee.EventId = eventId;
            attendee.PersonId = attendeeId;
            return _attendeeService.UpdateAttendeeRelationshipAsync(attendee);
        }

        [HttpGet("{eventId}/attendees")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<AttendeeRelationship>))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent doest exist")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = AttendeeErrorCodes.NO_PERMISSION_TO_GET_RELATIONSHIP + "\n\nUser has no permissions to see Attendees of this Event")]
        public virtual Task<IEnumerable<AttendeeRelationship>> GetAllAttendeeRelationshipsForEvent(int eventId)
        {
            return _attendeeService.GetAllRelationshipsForEventAsync(eventId);
        }

        [HttpGet("{eventId}/attendees/{attendeeId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(AttendeeRelationship))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent doest exist")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + PersonErrorCodes.PERSON_NOT_FOUND + "\n\nPerson could not be found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + AttendeeErrorCodes.RELATIONSHIP_NOT_FOUND + "\n\nRaltionship could not be found")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = AttendeeErrorCodes.NO_PERMISSION_TO_GET_RELATIONSHIP + "\n\nUser has no permissions to see Attendees of this Event")]
        public virtual Task<AttendeeRelationship> GetAttendeeRelationship(int attendeeId, int eventId)
        {
            return _attendeeService.GetSingleRelationshipAsync(eventId, attendeeId);
        }

        [HttpDelete("{eventId}/attendees/{attendeeId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent doest exist")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + PersonErrorCodes.PERSON_NOT_FOUND + "\n\nPerson could not be found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + AttendeeErrorCodes.RELATIONSHIP_NOT_FOUND + "\n\nRaltionship could not be found")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = AttendeeErrorCodes.NO_PERMISSION_TO_DELETE_RELATIONSHIP + "\n\nUser doesnt have permission to delete this AttendeeRelationship")]
        public virtual Task DeleteAttendeeRelationship(int attendeeId, int eventId)
        {
            return _attendeeService.DeleteAttendeeRelationshipAsync(eventId, attendeeId);
        }

        #endregion Attendees

        #region Location

        [HttpGet("{eventId}/location")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Location))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent doest exist")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = EventErrorCodes.NO_GET_PERMISSIONS + "\n\nUser doesnt have permission to view Event Details")]
        public virtual Task<Location> GetLocationForEvent(int eventId)
        {
            return _eventLocationService.GetLocationForEventAsync(eventId);
        }

        [HttpPost("{eventId}/location")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Location))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = EventErrorCodes.EVENT_NOT_FOUND + "\n\nEvent doest exist")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = EventErrorCodes.NO_UPDATE_PERMISSIONS + "\n\nUser doesnt have permission to update Event")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<Location> UpdateLocationForEvent(int eventId, [FromBody] Location location)
        {
            return _eventLocationService.UpdateLocationForEventAsync(eventId, location);
        }

        #endregion Location
    }
}