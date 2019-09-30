using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Events;
using Infrastructure.DataModel.Service;
using Infrastructure.Services;
using Infrastructure.Services.EventServiceModels;
using Microsoft.EntityFrameworkCore;

namespace Common.EventService
{
    public class EventServiceModelService : IEventServiceModelService
    {
        #region Vars

        private readonly DataContext _dataContext;
        private readonly IPersonService _personService;

        #endregion

        #region Constructors

        public EventServiceModelService(DataContext dataContext, IPersonService personService)
        {
            _dataContext = dataContext;
            _personService = personService;
        }

        #endregion


        public Task<EventServiceModel> GetServiceByIdAsync(int serviceId)
        {
            return _dataContext.EventService.FirstOrDefaultAsync(x => x.Id == serviceId);
        }

        public async Task<EventServiceModel> CreateEventServiceModelAsync(EventServiceModel serviceModel)
        {
            serviceModel.Id = 0;
            serviceModel.PersonId = (await _personService.GetPersonForUserAsync()).Id;
            _dataContext.EventService.Add(serviceModel);
            await _dataContext.SaveChangesAsync();
            return serviceModel;
        }

        public async Task<EventServiceModel> UpdateEventServiceModelAsync(EventServiceModel serviceModel)
        {
            var dbServiceModel = await _dataContext.EventService.FirstOrDefaultAsync(x => x.Id == serviceModel.Id);
            dbServiceModel.Profile = serviceModel.Profile;
            dbServiceModel.Salary = serviceModel.Salary;
            dbServiceModel.TypeId = serviceModel.TypeId;
            await _dataContext.SaveChangesAsync();
            return dbServiceModel;
        }

        public async Task DeleteEventServiceModelAsync(int serviceModelId)
        {
            _dataContext.EventService.Remove(await _dataContext.EventService.FirstOrDefaultAsync(x => x.Id == serviceModelId));
            await _dataContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<EventServiceModel>> GetServiceForCurrentUser()
        {
            var person = await _personService.GetPersonForUserAsync();

            return await _dataContext.EventService
                                     .Where(x => x.PersonId == person.Id)
                                     .ToListAsync();
        }

        public async Task<IEnumerable<EventServiceModel>> GetServiceByTypeIdAsync(int typeId)
        {
            return await _dataContext.EventService
                                     .Where(x => x.TypeId == typeId)
                                     .ToListAsync();
        }

        public async Task<IEnumerable<ServiceAgreement>> GetAgreementsForCurrentUser()
        {
            var person = await _personService.GetPersonForUserAsync();

            return await _dataContext.ServiceAgreements
                                     .Join(_dataContext.EventService,
                                           agreement => agreement.EventServiceModelId,
                                           service => service.Id,
                                           (agreement, service) => new {ServiceAgreement = agreement, EventServiceModel = service})
                                     .Where(x => x.EventServiceModel.PersonId == person.Id)
                                     .Select(x => x.ServiceAgreement)
                                     .ToListAsync();
        }
    }
}