using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Infrastructure.DataModel;
using Infrastructure.DataModel.Events;
using Infrastructure.Services.Events;
using Microsoft.EntityFrameworkCore;

namespace Common.Events.ServiceAgreements
{
    public class AgreementService : IAgreementService
    {
        private readonly DataContext _dataContext;

        public AgreementService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Task<ServiceAgreement> GetSingleServiceAgreementByEventSlot(int eventId, int spsId)
        {
            return _dataContext.ServiceAgreements.FirstAsync(x => x.EventId == eventId && x.ServiceSlotId == spsId);
        }

        public async Task<IEnumerable<ServiceAgreement>> GetAllServiceAgreementsByEvent(int eventId)
        {
            return await _dataContext.ServiceAgreements
                                     .Where(x => x.EventId == eventId)
                                     .ToListAsync();
        }

        public async Task<ServiceAgreement> CreateServiceAgreement(int eventId, int spsId, int serviceId)
        {
            var eventObj = await _dataContext.Events.FirstAsync(x => x.Id == eventId);
            var slot = await _dataContext.ServiceSlots.FirstAsync(x => x.EventId == eventId && x.Id == spsId);


            var agreement = new ServiceAgreement
                            {
                                EventId = eventId,
                                End = slot.End ?? eventObj.End,
                                Start = slot.Start ?? eventObj.Start,
                                EventServiceModelId = serviceId,
                                ServiceSlotId = spsId,
                                State = ServiceAgreementStates.Request
                            };

            _dataContext.ServiceAgreements.Add(agreement);
            await _dataContext.SaveChangesAsync();
            return agreement;
        }

        public async Task<ServiceAgreement> UpdateServiceAgreement(ServiceAgreement agreement)
        {
            var oldAgreement = await _dataContext.ServiceAgreements.FirstAsync(x => x.EventId == agreement.EventId && x.ServiceSlotId == agreement.ServiceSlotId);

            oldAgreement.Comment = agreement.Comment;
            oldAgreement.Cost = agreement.Cost;
            oldAgreement.Start = agreement.Start;
            oldAgreement.End = agreement.End;

            await _dataContext.SaveChangesAsync();
            return oldAgreement;
        }

        public async Task DeleteAgreement(int eventId, int spsId)
        {
            var oldAgreement = await _dataContext.ServiceAgreements.FirstAsync(x => x.EventId == eventId && x.ServiceSlotId == spsId);
            _dataContext.ServiceAgreements.Remove(oldAgreement);
            await _dataContext.SaveChangesAsync();
        }

        public Task<ServiceAgreement> ProposeAgreement(int eventId, int spsId)
        {
            return SetStateAsync(eventId,spsId,ServiceAgreementStates.Proposal);
        }

        public Task<ServiceAgreement> AcceptAgreement(int eventId, int spsId)
        {
            return SetStateAsync(eventId,spsId,ServiceAgreementStates.Accepted);
        }

        public Task<ServiceAgreement> DeclineAgreement(int eventId, int spsId)
        {
            return SetStateAsync(eventId, spsId, ServiceAgreementStates.Declined);
        }

        private async Task<ServiceAgreement> SetStateAsync(int eventId, int spsId, ServiceAgreementStates state)
        {
            var dbAgreement = await _dataContext.ServiceAgreements.FirstAsync(x => x.EventId == eventId
                                                                                   && x.ServiceSlotId == spsId);

            dbAgreement.State = state;
            await _dataContext.SaveChangesAsync();
            return dbAgreement;
        }
    }
}