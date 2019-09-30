using System;
using System.Net;
using System.Threading.Tasks;
using EventSystemWebApi.IntTest.Extensions;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataContracts;
using Infrastructure.DataModel.People;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.PersonTests
{
    public class PersonControllerPostTests : PersonControllerTestBase
    {
        [Fact]
        public async Task UpdatePerson_MustBeAuthenticated()
        {
            var r = await _Client.PostAsync("/api/person/profile", new RealPerson().ToStringContent());
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task UpdatePerson_NotFound_1_NoPerson()
        {
            var user = await SetupAuthenticationAsync();
            var person = await _Context.RealPeople.FirstOrDefaultAsync(x => x.UserId == user.Id);
            _Context.RealPeople.Remove(person);
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync("api/person/profile", new RealPerson().ToStringContent());
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PersonErrorCodes.PERSON_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task UpdatePerson_Success()
        {
            var user = await SetupAuthenticationAsync();

            await _Context.SaveChangesAsync();

            var person = new RealPerson
            {
                Firstname = Guid.NewGuid().ToString(),
                Lastname = Guid.NewGuid().ToString()
            };

            var r = await _Client.PostAsync("/api/person/profile", person.ToStringContent());
            r.EnsureSuccessStatusCode();

            var dbPerson = await _Context.RealPeople.FirstOrDefaultAsync(x => x.UserId == user.Id);
            await _Context.Entry(dbPerson).ReloadAsync();

            Assert.Equal(person.Firstname, dbPerson.Firstname);
            Assert.Equal(person.Lastname, dbPerson.Lastname);
        }

        [Fact]
        public async Task UpdateProfilePicture_MustBeAuthenticated()
        {
            var r = await _Client.PostAsync("api/person/profile/picture", null);
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task UpdateProfilePicture_NotFound_1_NoPerson()
        {
            var user = await SetupAuthenticationAsync();
            var person = await _Context.RealPeople.FirstOrDefaultAsync(x => x.UserId == user.Id);
            _Context.RealPeople.Remove(person);
            await _Context.SaveChangesAsync();

            var r = await _Client.PostAsync("api/person/profile/picture", new PictureDTO {Picture = Guid.NewGuid().ToByteArray()}.ToStringContent());
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PersonErrorCodes.PERSON_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateProfilePicture_Success()
        {
            var user = await SetupAuthenticationAsync();
            var picture = new PictureDTO {Picture = Guid.NewGuid().ToByteArray()};

            var r = await _Client.PostAsync("api/person/profile/picture", picture.ToStringContent());
            r.EnsureSuccessStatusCode();

            var dbPerson = await _Context.RealPeople.FirstOrDefaultAsync(x => x.UserId == user.Id);
            await _Context.Entry(dbPerson).ReloadAsync();

            Assert.Equal(picture.Picture, dbPerson.ProfilePicture);
        }

        [Fact]
        public async Task UpdatePerson_422_0_NoData()
        {
            await SetupAuthenticationAsync();
            var r = await _Client.PostAsync("api/person/profile", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }

        [Fact]
        public async Task UpdateProfilePicture_422_0_NoData()
        {
            await SetupAuthenticationAsync();
            var r = await _Client.PostAsync("api/person/profile/picture", "".ToStringContent());

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(GlobalErrorCodes.NO_DATA), error.ErrorCode);
        }
        
    }
}