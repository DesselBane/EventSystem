using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.Service;
using Infrastructure.DataModel.ServiceAttributes;
using Infrastructure.ErrorCodes;
using Infrastructure.Services.EventServiceModels;
using Infrastructure.Services.ServiceTypes;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;

namespace EventSystemWebApi.Controllers
{
    [Route("api/[controller]")]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class ServiceTypesController : Controller
    {
        #region Vars

        private readonly IServiceTypeService _serviceTypeService;
        private readonly IAttributeSpecificationService _attributeSpecificationService;
        private readonly IEventServiceModelService _eventServiceModelService;

        #endregion

        #region Constructors

        public ServiceTypesController(IServiceTypeService serviceTypeService, IAttributeSpecificationService attributeSpecificationService, IEventServiceModelService eventServiceModelService)
        {
            _serviceTypeService = serviceTypeService;
            _attributeSpecificationService = attributeSpecificationService;
            _eventServiceModelService = eventServiceModelService;
        }

        #endregion

        #region Types

        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<ServiceType>))]
        public virtual Task<IEnumerable<ServiceType>> GetServiceProviderTyps()
        {
            return _serviceTypeService.GetServiceAllTypesAsync();
        }

        [HttpPut]
        [SwaggerResponse(HttpStatusCode.Created, typeof(ServiceType))]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.SERVICE_TYPE_ALREADY_EXISTS + "\n\nOccurs if Type already exists")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE_TYPE + "\n\nUser doenst have permission to update an EventServiceProviderType")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.TYPE_NAME_TOO_LONG + "\n\nType Name exceeds max length of 50 chars")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + ServiceTypeErrorCodes.TYPE_NAME_MUST_BE_SET + "\n\nName must be set")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<ServiceType> CreateServiceProviderType([FromBody] ServiceType type)
        {
            HttpContext.Response.StatusCode = (int) HttpStatusCode.Created;
            return _serviceTypeService.CreateServiceTypeAsync(type);
        }

        [HttpPost("{typeId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ServiceType))]
        [SwaggerResponse(HttpStatusCode.Conflict, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.SERVICE_TYPE_ALREADY_EXISTS + "\n\nOccurs if Type already exists")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE_TYPE + "\n\nUser doenst have permission to update an EventServiceProviderType")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND + "\n\nServiceType not found")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.TYPE_NAME_TOO_LONG + "\n\nType Name exceeds max length of 50 chars")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + ServiceTypeErrorCodes.TYPE_NAME_MUST_BE_SET + "\n\nName must be set")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<ServiceType> UpdateServiceProviderType([FromBody] ServiceType type, int typeId)
        {
            type.Id = typeId;
            return _serviceTypeService.UpdateServiceTypeAsync(type);
        }

        [HttpDelete("{typeId}")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.NO_PERMISSION_TO_DELETE_SERVICE_TYPE + "\n\nUser doenst have permission to delete an EventServiceProviderType")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND + "\n\nServiceType not found")]
        public virtual Task DeleteServiceProviderType(int typeId)
        {
            return _serviceTypeService.DeleteServiceTypeAsync(typeId);
        }

        [HttpGet("{typeId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ServiceType))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND + "\n\nServiceType not found")]
        public virtual Task<ServiceType> GetServiceProviderType(int typeId)
        {
            return _serviceTypeService.GetSingleServiceTypeAsync(typeId);
        }

        #endregion

        #region Attribute Specification

        [HttpGet("{typeId}/spec")]
        [SwaggerResponse(HttpStatusCode.OK,typeof(IEnumerable<ServiceAttributeSpecification>))]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND + "\n\nService Type not found")]
        public Task<IEnumerable<ServiceAttributeSpecification>> GetAllServiceAttributeSpecificationsForServiceType(int typeId)
        {
            return _attributeSpecificationService.GetAllSpecificationsForTypeAsync(typeId);
        }

        [HttpGet("{typeId}/spec/{specId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ServiceAttributeSpecification))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND + "\n\nService Type not found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + AttributeSpecificationErrorCodes.NOT_FOUND + "\n\nService Attribute Specification not found")]
        public Task<ServiceAttributeSpecification> GetSingleAttributeSpecificationForServiceType(int typeId, int specId)
        {
            return _attributeSpecificationService.GetSingleSpecificationAsync(specId, typeId);
        }

        [HttpPut("{typeId}/spec")]
        [SwaggerResponse(HttpStatusCode.Created,typeof(ServiceAttributeSpecification))]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND + "\n\nService Type not found")]
        [SwaggerResponse(HttpStatusCode.Forbidden,typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE_TYPE + "\n\nUser doest have permission to update Service Types")]
        [SwaggerResponse(422,typeof(ExceptionDTO), Description = AttributeSpecificationErrorCodes.NAME_MUST_BE_SET + "\n\nSpecification Name must be set")]
        [SwaggerResponse(422,typeof(ExceptionDTO), Description = "\n" + AttributeSpecificationErrorCodes.SPEC_TYPE_MUST_BE_SET + "\n\nSpecification Type must be set")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<ServiceAttributeSpecification> CreateServiceAttributeSpecification(int typeId, [FromBody] ServiceAttributeSpecification spec)
        {
            HttpContext.Response.StatusCode = (int) HttpStatusCode.Created;
            spec.ServiceTypeId = typeId;
            return _attributeSpecificationService.CreateAttributeSpecificationAsync(spec);
        }

        [HttpPost("{typeId}/spec/{specId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(ServiceAttributeSpecification))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND + "\n\nService Type not found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + AttributeSpecificationErrorCodes.NOT_FOUND + "\n\nService Attribute Specification not found")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE_TYPE + "\n\nUser doest have permission to update Service Types")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = AttributeSpecificationErrorCodes.NAME_MUST_BE_SET + "\n\nSpecification Name must be set")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + AttributeSpecificationErrorCodes.SPEC_TYPE_MUST_BE_SET + "\n\nSpecification Type must be set")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = "\n" + GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual Task<ServiceAttributeSpecification> UpdateServiceAttributeSpecification(int typeId, int specId, [FromBody] ServiceAttributeSpecification spec)
        {
            spec.ServiceTypeId = typeId;
            spec.Id = specId;
            return _attributeSpecificationService.UpdateAttributeSpecificationAsync(spec);
        }

        [HttpDelete("{typeId}/spec/{specId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(void))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND + "\n\nService Type not found")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = "\n" + AttributeSpecificationErrorCodes.NOT_FOUND + "\n\nService Attribute Specification not found")]
        [SwaggerResponse(HttpStatusCode.Forbidden, typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE_TYPE + "\n\nUser doest have permission to update Service Types")]
        public Task DeleteServiceAttributeSpecification(int typeId, int specId)
        {
            return _attributeSpecificationService.DeleteSpecificationAsync(specId, typeId);
        }

        #endregion Attribute Specification

        #region ServiceProvider

        [HttpGet("{typeId}/serviceProvider")]
        [SwaggerResponse(HttpStatusCode.OK,typeof(IEnumerable<EventServiceModel>))]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = ServiceTypeErrorCodes.SERVICE_TYPE_NOT_FOUND + "\n\nServiceType not found")]
        public Task<IEnumerable<EventServiceModel>> GetAllServiceProvidersForServiceType(int typeId)
        {
            return _eventServiceModelService.GetServiceByTypeIdAsync(typeId);
        }

        #endregion ServiceProvider
    }
}