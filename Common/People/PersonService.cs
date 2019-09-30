using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.DataModel;
using Infrastructure.DataModel.People;
using Infrastructure.Services;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace Common.People
{
    public class PersonService : IPersonService
    {
        #region Vars

        private readonly IAuthenticationService _authService;
        private readonly DataContext _personContext;

        #endregion

        #region Constructors

        public PersonService(DataContext personContext, IAuthenticationService authService)
        {
            _personContext = personContext;
            _authService = authService;
        }

        #endregion

        public async Task UpdatePersonAsync(Person newValues, int personId)
        {
            var dbPerson = await _personContext.RealPeople.FirstOrDefaultAsync(x => x.Id == personId);

            UpdatePerson(newValues, dbPerson);
            await _personContext.SaveChangesAsync();
        }

        public async Task UpdateProfilePictureAsync(byte[] picture, int personId)
        {
            var person = await _personContext.RealPeople.FirstOrDefaultAsync(x => x.Id == personId);

            person.ProfilePicture = picture;
            await _personContext.SaveChangesAsync();
        }

        public async Task<RealPerson> GetPersonForUserAsync()
        {
            var user = await _authService.GetCurrentUserAsync();
            return await _personContext.RealPeople.FirstOrDefaultAsync(x => x.UserId == user.Id);
        }

        public Task<Person> GetPersonByIdAsync(int personId)
        {
            return _personContext.People.FirstOrDefaultAsync(x => x.Id == personId);
        }

        public async Task<IEnumerable<Person>> SearchForPerson(string searchTerm)
        {
            var expr = PredicateBuilder.New<Person>(false);
            var terms = Regex.Matches(searchTerm, ConstantsAndStatics.PersonSearchPattern)
                             .Cast<Match>()
                             .Select(x => x.Value)
                             .ToList();

            foreach (var term in terms)
            {
                if (term.Length < 3)
                    expr = expr.Or(x => x.Firstname
                                         .ToLower()
                                         .StartsWith(term.ToLower()))
                               .Or(x => x.Lastname
                                         .ToLower()
                                         .StartsWith(term.ToLower()));
                else
                    expr = expr.Or(x => x.Firstname
                                         .ToLower()
                                         .Contains(term.ToLower()))
                               .Or(x => x.Lastname
                                         .ToLower()
                                         .Contains(term.ToLower()));
            }

            return await _personContext.People
                                       .Where(expr)
                                       .ToListAsync();
        }

        #region Impl

        public static void UpdatePerson(Person dtoPerson, Person dbPerson)
        {
            dbPerson.Firstname = dtoPerson.Firstname;
            dbPerson.Lastname = dtoPerson.Lastname;
        }

        #endregion
    }
}