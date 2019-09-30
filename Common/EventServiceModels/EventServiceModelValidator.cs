using System;
using System.Linq;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Service;
using Infrastructure.ErrorCodes;
using Infrastructure.Services;
using JetBrains.Annotations;

namespace Common.EventServiceModels
{
    public class EventServiceModelValidator
    {
        #region Vars

        private readonly DataContext _modelContext;
        private readonly IPersonService _personService;

        #endregion

        #region Constructors

        public EventServiceModelValidator(DataContext modelContext, IPersonService personService)
        {
            _modelContext = modelContext;
            _personService = personService;
        }

        #endregion

        public void ValidateEventServiceModelExists(int serviceModelId)
        {
            if (!_modelContext.EventService.Any(x => x.Id == serviceModelId))
                throw new NotFoundException(serviceModelId, nameof(EventServiceModel), Guid.Parse(ServiceErrorCodes.SERVICE_NOT_FOUND));
        }

        [AssertionMethod]
        public void ValidateEventServiceModelSalary(EventServiceModel serviceModel)
        {
            if (serviceModel.Salary < 0)
                throw new UnprocessableEntityException("Salary must be greater or equal to 0", Guid.Parse(ServiceErrorCodes.SALARY_MUST_BE_GREATER_OR_EQUAL_ZERO));
        }

        [AssertionMethod]
        public void CanUpdateEventServiceModel(int serviceModelId)
        {
            var person = _personService.GetPersonForUserAsync().Result;

            if (!_modelContext.EventService.Any(x => x.PersonId == person.Id && x.Id == serviceModelId))
                throw new ForbiddenException("User donst have permissions to update this EventServiceModel", Guid.Parse(ServiceErrorCodes.NO_PERMISSION_TO_UPDATE_SERVICE));
        }

        [AssertionMethod]
        public void CanDeleteEventServiceModel(int serviceModelId)
        {
            var person = _personService.GetPersonForUserAsync().Result;

            if (!_modelContext.EventService.Any(x => x.PersonId == person.Id && x.Id == serviceModelId))
                throw new ForbiddenException("User doenst have permissions to delete this EventServiceModel", Guid.Parse(ServiceErrorCodes.NO_PERMISSION_TO_DELETE_SERVICE));
        }
    }
}