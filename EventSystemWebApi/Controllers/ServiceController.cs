using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Events;
using Infrastructure.DataModel.Misc;
using Infrastructure.DataModel.Service;
using Infrastructure.DataModel.ServiceAttributes;
using Infrastructure.ErrorCodes;
using Infrastructure.Services.EventServiceModels;
using Infrastructure.Services.EventServiceModels.ServiceAttributes;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace EventSystemWebApi.Controllers
{
    [Route("api/[controller]")]
    public class ServiceController : Controller
    {
        #region Vars

        private readonly IEventServiceModelService _eventServiceModelService;
        private readonly IServiceLocationService _locationService;
        private readonly IServiceAttributeService _attributeService;

        #endregion

        #region Constructors

        public ServiceController(IEventServiceModelService eventServiceModelService, IServiceLocationService locationService, IServiceAttributeService attributeService)
        {
            _eventServiceModelService = eventServiceModelService;
            _locationService = locationService;
            _attributeService = attributeService;
        }

        #endregion

        #region Location

        [HttpGet("{serviceId}/location")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Location))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = ServiceErrorCodes.SERVICE_NOT_FOUND + "\n\nService doest exist")]
        public virtual Task<Location> GetLocationForEvent(int serviceId)
        {
            return _locationService.GetLocationForServiceAsync(serviceId);
        }

        [HttpPost("{serviceId}/location")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Location))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = ServiceErrorCodes.SERVICE_NOT_FOUND + "\n\nService doest exist")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = ServiceErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE + "\n\nUser doesnt have permission to update Service")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<Location> UpdateLocationForEvent(int serviceId, [FromBody] Location location)
        {
            return _locationService.UpdateLocationForServiceAsync(serviceId, location);
        }

        #endregion Location

        #region Service

        //TODO test this stuff
        [HttpGet("my")]
        [SwaggerResponse(HttpStatusCode.OK,typeof(IEnumerable<EventServiceModel>))]
        public Task<IEnumerable<EventServiceModel>> GetServiceForCurrentUser()
        {
            return _eventServiceModelService.GetServiceForCurrentUser();
        }
        
        [HttpGet("{serviceId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EventServiceModel))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = ServiceErrorCodes.SERVICE_NOT_FOUND + "\n\nService doest exist")]
        public Task<EventServiceModel> GetServiceById(int serviceId)
        {
            return _eventServiceModelService.GetServiceByIdAsync(serviceId);
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.Created, typeof(EventServiceModel))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND + "\n\nServiceType not found")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = ServiceErrorCodes.SALARY_MUST_BE_GREATER_OR_EQUAL_ZERO + "\n\nSalary must be greater or equal to 0")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<EventServiceModel> CreateEventService([FromBody] EventServiceModel serviceModel)
        {
            HttpContext.Response.StatusCode = (int) HttpStatusCode.Created;
            return _eventServiceModelService.CreateEventServiceModelAsync(serviceModel);
        }

        [HttpPost("{serviceId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(EventServiceModel))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND + "\n\nServiceType not found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + ServiceErrorCodes.SERVICE_NOT_FOUND + "\n\nService doest exist")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = ServiceErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE + "\n\nUser doesnt have permission to update Service")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = ServiceErrorCodes.SALARY_MUST_BE_GREATER_OR_EQUAL_ZERO + "\n\nSalary must be greater or equal to 0")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<EventServiceModel> UpdateService(int serviceId, [FromBody] EventServiceModel serviceModel)
        {
            serviceModel.Id = serviceId;
            return _eventServiceModelService.UpdateEventServiceModelAsync(serviceModel);
        }

        [HttpDelete("{serviceId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = ServiceErrorCodes.SERVICE_NOT_FOUND + "\n\nService doest exist")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = ServiceErrorCodes.NO_PERMISSION_TO_DELETE_SERVICE + "\n\nUser doesnt have permission to delete Service")]
        public Task DeleteService(int serviceId)
        {
            return _eventServiceModelService.DeleteEventServiceModelAsync(serviceId);
        }

        #endregion Service

        #region Attributes

        [HttpPost("{serviceId}/attributes/{specId}")]
        [SwaggerResponse(HttpStatusCode.OK,typeof(ServiceAttribute), Description = "Will be created if it doesnt exist. Value defaults to 'undefined'")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceErrorCodes.SERVICE_NOT_FOUND + "\n\nService could not be found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = "\n" + AttributeSpecificationErrorCodes.NOT_FOUND + "\n\nAttribute Specification not found")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = ServiceErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE + "\n\nUser doesnt have permission to update Service")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<ServiceAttribute> CreateNewServiceAttribute([FromBody] ServiceAttribute attr, int serviceId, int specId)
        {
            attr.EventServiceModelId = serviceId;
            attr.ServiceAttributeSpecificationId = specId;
            return _attributeService.UpdateAttributeAsync(attr);
        }

        [HttpGet("{serviceId}/attributes")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceErrorCodes.SERVICE_NOT_FOUND + "\n\nService could not be found")]
        public Task<IEnumerable<ServiceAttribute>> GetAllAttributesForService(int serviceId)
        {
            return _attributeService.GetAllAttributesForServiceAsync(serviceId);
        }

        [HttpGet("{serviceId}/attributes/{specId}")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceErrorCodes.SERVICE_NOT_FOUND + "\n\nService could not be found")]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = "\n" + AttributeSpecificationErrorCodes.NOT_FOUND + "\n\nAttribute Specification not found")]
        public Task<ServiceAttribute> GetSingleAttributeForServiceAndSpec(int serviceId, int specId)
        {
            return _attributeService.GetSingleAttributeAsync(serviceId, specId);
        }
        
        #endregion Attributes

        #region Agreements

        [HttpGet("my/agreements")]
        [SwaggerResponse(HttpStatusCode.OK,typeof(IEnumerable<ServiceAgreement>))]
        public Task<IEnumerable<ServiceAgreement>> GetAgreementsForCurrentUser()
        {
            return _eventServiceModelService.GetAgreementsForCurrentUser();
        }

        #endregion Agreements
    }
}