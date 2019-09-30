using System;
using System.Linq;
using System.Text.RegularExpressions;
using Infrastructure;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel;
using Infrastructure.DataModel.People;
using Infrastructure.ErrorCodes;
using Infrastructure.Services;

namespace Common.People
{
    public class PersonValidator
    {
        #region Vars

        private readonly IAuthenticationService _authenticationService;
        private readonly DataContext _personContext;

        #endregion

        #region Constructors

        public PersonValidator(DataContext personContext, IAuthenticationService authenticationService)
        {
            _personContext = personContext;
            _authenticationService = authenticationService;
        }

        #endregion

        public void ValidateCurrentUserHasPerson()
        {
            var user = _authenticationService.GetCurrentUserAsync().Result;
            if (!_personContext.RealPeople.Any(x => x.UserId == user.Id))
                throw new NotFoundException(user.Id, nameof(RealPerson), Guid.Parse(PersonErrorCodes.PERSON_NOT_FOUND), "This is the user Id since there is no Person Id available");
        }

        public void ValidatePersonExists(int personId)
        {
            if (!_personContext.People.Any(x => x.Id == personId))
                throw new NotFoundException(personId, nameof(Person), Guid.Parse(PersonErrorCodes.PERSON_NOT_FOUND));
        }

        public void ValidateSearchTerm(string term)
        {
            if (!Regex.IsMatch(term, ConstantsAndStatics.PersonSearchPattern))
                throw new UnprocessableEntityException("Invalid search term", Guid.Parse(PersonErrorCodes.INVALID_SEARCH_TERM));
        }
    }
}