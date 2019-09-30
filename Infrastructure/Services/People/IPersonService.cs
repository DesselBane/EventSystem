using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.DataModel.People;

namespace Infrastructure.Services
{
    public interface IPersonService
    {
        Task UpdatePersonAsync(Person newValues, int personId);
        Task UpdateProfilePictureAsync(byte[] picture, int personId);
        Task<RealPerson> GetPersonForUserAsync();
        Task<Person> GetPersonByIdAsync(int personId);
        Task<IEnumerable<Person>> SearchForPerson(string searchTerm);
    }
}