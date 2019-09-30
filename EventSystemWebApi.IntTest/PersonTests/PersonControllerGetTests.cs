using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.People;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.PersonTests
{
    public class PersonControllerGetTests : PersonControllerTestBase
    {
        [Fact]
        public async Task GetProfile_MustBeAuthenticated()
        {
            var r = await _Client.GetAsync("api/person/999");
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task GetProfile_NotFound_1_Profile()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.GetAsync("api/person/999");
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PersonErrorCodes.PERSON_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task GetProfile_Success()
        {
            var setup = await SetupAuthenticationAsync();

            var r = await _Client.GetAsync($"api/person/{setup.Person.Id}");
            r.EnsureSuccessStatusCode();

            var person = JsonConvert.DeserializeObject<RealPerson>(await r.Content.ReadAsStringAsync());

            Assert.Equal(setup.Person.Firstname, person.Firstname);
            Assert.Equal(setup.Person.Lastname, person.Lastname);
            Assert.Equal(setup.Person.ProfilePicture, person.ProfilePicture);
            Assert.Equal(setup.Person.Id, person.Id);
        }

        [Fact]
        public async Task Search_1Char()
        {
            await SetupPeopleAsync();

            var r = await _Client.GetAsync("api/person/search?term=!§$%/()\\j´`^°");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<dynamic>>(await r.Content.ReadAsStringAsync());
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result.Count(x => x.firstname == "James"));
            Assert.Equal(1, result.Count(x => x.firstname == "Josephine"));
        }

        [Fact]
        public async Task Search_2Chars()
        {
            await SetupPeopleAsync();
            var r = await _Client.GetAsync("api/person/search?term=!§$%/()\\jA´`^°");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<dynamic>>(await r.Content.ReadAsStringAsync());
            Assert.Equal(1, result.Count);
            Assert.Equal(1, result.Count(x => x.firstname == "James"));
        }

        [Fact]
        public async Task Search_3Chars()
        {
            await SetupPeopleAsync();
            var r = await _Client.GetAsync("api/person/search?term=!§$%/()=?\\oll");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<dynamic>>(await r.Content.ReadAsStringAsync());
            Assert.Equal(2, result.Count);
            Assert.Equal(1, result.Count(x => x.firstname == "Donnette"));
            Assert.Equal(1, result.Count(x => x.firstname == "Mitsue"));
        }

        [Fact]
        public async Task Search_1CharAnd3Char()
        {
            await SetupPeopleAsync();
            var r = await _Client.GetAsync("api/person/search?term=oLL j");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<dynamic>>(await r.Content.ReadAsStringAsync());
            Assert.Equal(4, result.Count);
            Assert.Equal(1, result.Count(x => x.firstname == "James"));
            Assert.Equal(1, result.Count(x => x.firstname == "Josephine"));
            Assert.Equal(1, result.Count(x => x.firstname == "Donnette"));
            Assert.Equal(1, result.Count(x => x.firstname == "Mitsue"));
        }

        [Fact]
        public async Task Search_1CharAnd3Char_WithSpecialChars()
        {
            await SetupPeopleAsync();
            var r = await _Client.GetAsync("api/person/search?term=oLL!§$%/()=?\\j´`^°");
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<List<dynamic>>(await r.Content.ReadAsStringAsync());
            Assert.Equal(4, result.Count);
            Assert.Equal(1, result.Count(x => x.firstname == "James"));
            Assert.Equal(1, result.Count(x => x.firstname == "Josephine"));
            Assert.Equal(1, result.Count(x => x.firstname == "Donnette"));
            Assert.Equal(1, result.Count(x => x.firstname == "Mitsue"));
        }

        [Fact]
        public async Task Search_MultipleTermsWithNumbers()
        {
            await SetupPeopleAsync();
            var r = await _Client.GetAsync("api/person/search?term=123 öck äkj ütt");

            var result = JsonConvert.DeserializeObject<List<dynamic>>(await r.Content.ReadAsStringAsync());
            Assert.Equal(4, result.Count);
            Assert.Equal(1, result.Count(x => x.firstname == "James"));
            Assert.Equal(1, result.Count(x => x.firstname == "Josephine"));
            Assert.Equal(1, result.Count(x => x.firstname == "Lenna"));
            Assert.Equal(1, result.Count(x => x.firstname == "Leota123"));
        }

        [Fact]
       public async Task Search_422_1_InvalidSearchTerm()
        {
            await SetupPeopleAsync();
            var r = await _Client.GetAsync("api/person/search?term= !$§%/()=?`´\\}][{#+-.:,;*^°@€'~µ²³");

            Assert.Equal((HttpStatusCode) 422, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PersonErrorCodes.INVALID_SEARCH_TERM), error.ErrorCode);
        }
        
        [Fact]
        public async Task Current_MustBeAuthenticated()
        {
            var r = await _Client.GetAsync("api/person/profile");
            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task Current_NotFound_1_NoPerson()
        {
            var user = await SetupAuthenticationAsync();
            var person = await _Context.RealPeople.FirstOrDefaultAsync(x => x.UserId == user.Id);
            _Context.RealPeople.Remove(person);
            await _Context.SaveChangesAsync();

            var r = await _Client.GetAsync("/api/person/profile");
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PersonErrorCodes.PERSON_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task Current_Success()
        {
            var tmpContext = CreateDataContext();
            var user = await SetupAuthenticationAsync();

            var person = await tmpContext.RealPeople.FirstOrDefaultAsync(x => x.UserId == user.Id);

            person.Firstname = Guid.NewGuid().ToString();
            person.Lastname = Guid.NewGuid().ToString();

            await tmpContext.SaveChangesAsync();

            var r = await _Client.GetStringAsync("/api/person/profile");

            var retVal = JsonConvert.DeserializeObject<RealPerson>(r);

            Assert.Equal(person.Firstname, retVal.Firstname);
            Assert.Equal(person.Lastname, retVal.Lastname);
        }

        [Fact]
        public async Task GetProfilePicture_Success()
        {
            var setup = await SetupPeopleAsync();
            var newProfilePic = Guid.NewGuid().ToByteArray();

            setup.user.Person.ProfilePicture = newProfilePic;

            await _Context.SaveChangesAsync();

            var r = await _Client.GetAsync(setup.user.Person.ProfilePictureUrl);
            r.EnsureSuccessStatusCode();

            var result = JsonConvert.DeserializeObject<byte[]>(await r.Content.ReadAsStringAsync());

            Assert.NotNull(result);
            Assert.Equal(newProfilePic,result);
        }

        [Fact]
        public async Task GetProfilePicture_NotFound_Person()
        {
            await SetupAuthenticationAsync();

            var r = await _Client.GetAsync("api/person/999/picture");
            
            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);
            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(PersonErrorCodes.PERSON_NOT_FOUND), error.ErrorCode);
        }
    }
}