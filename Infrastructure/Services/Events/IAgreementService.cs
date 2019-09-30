using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.DataModel.Events;

namespace Infrastructure.Services.Events
{
    public interface IAgreementService
    {
        Task<ServiceAgreement> GetSingleServiceAgreementByEventSlot(int eventId, int spsId);
        Task<IEnumerable<ServiceAgreement>> GetAllServiceAgreementsByEvent(int eventId);
        Task<ServiceAgreement> CreateServiceAgreement(int eventId, int spsId, int serviceId);
        Task<ServiceAgreement> UpdateServiceAgreement(ServiceAgreement agreement);
        Task DeleteAgreement(int eventId, int spsId);
        Task<ServiceAgreement> ProposeAgreement(int eventId, int spsId);
        Task<ServiceAgreement> AcceptAgreement(int eventId, int spsId);
        Task<ServiceAgreement> DeclineAgreement(int eventId, int spsId);
    }
}