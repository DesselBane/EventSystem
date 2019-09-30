using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataContracts;
using Infrastructure.DataModel.People;
using Infrastructure.ErrorCodes;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.EntityFrameworkCore.Query.Internal;
using NSwag.Annotations;

namespace EventSystemWebApi.Controllers
{
    [Route("api/[controller]")]
    public class PersonController
    {
        #region Vars

        private readonly IPersonService _personService;

        #endregion

        #region Constructors

        public PersonController(IPersonService personService)
        {
            _personService = personService;
        }

        #endregion

        [HttpGet("{personId}")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Person))]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = PersonErrorCodes.PERSON_NOT_FOUND + "\n\nPerson doesnt exist")]
        public virtual Task<Person> GetPerson(int personId)
        {
            return _personService.GetPersonByIdAsync(personId);
        }

        [HttpGet("search")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(IEnumerable<Person>))]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = PersonErrorCodes.INVALID_SEARCH_TERM + "\n\nInvalid Search Term")]
        public virtual Task<IEnumerable<Person>> SearchForPerson([FromQuery] string term)
        {
            return _personService.SearchForPerson(term);
        }
        
        [HttpGet("profile")]
        [SwaggerResponse(HttpStatusCode.OK, typeof(Person), Description = "The Person object currently associated with the authenticated User")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = PersonErrorCodes.PERSON_NOT_FOUND + "\n\nThe currently authenticated User has no Person object associated")]
        public virtual async Task<Person> CurrentAsync()
        {
            return await _personService.GetPersonForUserAsync();
        }

        [HttpPost("profile")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = PersonErrorCodes.PERSON_NOT_FOUND + "\n\nThe currently authenticated User has no Person object associated")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual async Task UpdatePerson([FromBody] RealPerson person)
        {
            var currentPerson = await _personService.GetPersonForUserAsync();
            await _personService.UpdatePersonAsync(person, currentPerson.Id);
        }

        [HttpPost("profile/picture")]
        [SwaggerResponse(HttpStatusCode.NotFound, typeof(ExceptionDTO), Description = PersonErrorCodes.PERSON_NOT_FOUND + "\n\nThe currently authenticated User has no Person object associated")]
        [SwaggerResponse(422, typeof(ExceptionDTO), Description = GlobalErrorCodes.NO_DATA + "\n\n Body Argument was null")]
        public virtual async Task UpdateProfilePicture([FromBody] PictureDTO picture)
        {
            var currentPerson = await _personService.GetPersonForUserAsync();

            await _personService.UpdateProfilePictureAsync(picture.Picture, currentPerson.Id);
        }

        [HttpGet("{personId}/picture")]
        [SwaggerResponse(HttpStatusCode.OK,typeof(byte[]))]
        [SwaggerResponse(HttpStatusCode.NotFound,typeof(ExceptionDTO), Description = PersonErrorCodes.PERSON_NOT_FOUND + "\n\nPerson could not be found")]
        public async Task<byte[]> GetProfilePicture(int personId)
        {
            return (await _personService.GetPersonByIdAsync(personId)).ProfilePicture;
        }
    }
}