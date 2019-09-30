using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using Infrastructure.DataModel.People;
using Infrastructure.Interception;
using Infrastructure.Services;

namespace Common.People
{
    public class PersonServiceInterceptor : InterceptingMappingBase, IPersonService
    {
        #region Vars

        private readonly PersonValidator _personValidator;

        #endregion

        #region Constructors

        public PersonServiceInterceptor(PersonValidator personValidator)
        {
            _personValidator = personValidator;

            BuildUp(new Dictionary<string, Action<IInvocation>>
            {
                {
                    nameof(UpdatePersonAsync),
                    x => UpdatePersonAsync((Person) x.Arguments[0], (int) x.Arguments[1])
                },
                {
                    nameof(UpdateProfilePictureAsync),
                    x => UpdateProfilePictureAsync((byte[]) x.Arguments[0], (int) x.Arguments[1])
                },
                {
                    nameof(GetPersonForUserAsync),
                    x => GetPersonForUserAsync()
                },
                {
                    nameof(GetPersonByIdAsync),
                    x => GetPersonByIdAsync((int) x.Arguments[0])
                },
                {
                    nameof(SearchForPerson),
                    x => SearchForPerson((string) x.Arguments[0])
                }
            });
        }

        #endregion

        #region Implementation of IPersonService

        public Task UpdatePersonAsync(Person newValues, int personId)
        {
            _personValidator.ValidatePersonExists(personId);
            return null;
        }

        public Task UpdateProfilePictureAsync(byte[] picture, int personId)
        {
            _personValidator.ValidatePersonExists(personId);
            return null;
        }

        public Task<RealPerson> GetPersonForUserAsync()
        {
            _personValidator.ValidateCurrentUserHasPerson();
            return null;
        }

        public Task<Person> GetPersonByIdAsync(int personId)
        {
            _personValidator.ValidatePersonExists(personId);
            return null;
        }

        public Task<IEnumerable<Person>> SearchForPerson(string searchTerm)
        {
            _personValidator.ValidateSearchTerm(searchTerm);
            return null;
        }

        #endregion
    }
}