using System;
using System.Net;
using System.Threading.Tasks;
using Infrastructure.AspCore.Exceptions;
using Infrastructure.DataModel.People;
using Infrastructure.DataModel.Security;
using Infrastructure.ErrorCodes;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Xunit;

namespace EventSystemWebApi.IntTest.EventTests
{
    public class EventControllerDeleteTests : EventControllerTestBase
    {
        [Fact]
        public async Task DeleteEvent_Forbidden_3()
        {
            var setup = await SetupAsync();
            var user = new User
            {
                EMail = "a@a.de",
                Password = "asdkfgj",
                Salt = "flkjgn",
                Person = new RealPerson
                {
                    Firstname = "adlkfgjhn",
                    Lastname = "sdlgfkjn"
                }
            };

            var ctx = CreateDataContext();
            ctx.RealPeople.Add(user.Person);
            ctx.Users.Add(user);
            (await ctx.Events.FirstOrDefaultAsync(x => x.Id == setup.eventItem.Id)).Host = user.Person;
            await ctx.SaveChangesAsync();

            var r = await _Client.DeleteAsync($"/api/event/{setup.eventItem.Id}");

            Assert.Equal(HttpStatusCode.Forbidden, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.NO_DELETE_PERMISSIONS), error.ErrorCode);
        }

        [Fact]
        public async Task DeleteEvent_MustBeAuthenticated()
        {
            var r = await _Client.DeleteAsync("/api/event/0");

            Assert.Equal(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        [Fact]
        public async Task DeleteEvent_NotFound_1()
        {
            await SetupAuthenticationAsync();
            var r = await _Client.DeleteAsync("/api/event/888");

            Assert.Equal(HttpStatusCode.NotFound, r.StatusCode);

            var error = JsonConvert.DeserializeObject<ExceptionDTO>(await r.Content.ReadAsStringAsync());
            Assert.Equal(Guid.Parse(EventErrorCodes.EVENT_NOT_FOUND), error.ErrorCode);
        }

        [Fact]
        public async Task DeleteEvent_Success()
        {
            var setup = await SetupAsync();

            var r = await _Client.DeleteAsync($"/api/event/{setup.eventItem.Id}");
            r.EnsureSuccessStatusCode();

            var ctx = CreateDataContext();

            var dbEvent = await ctx.Events.FirstOrDefaultAsync(x => x.Id == setup.eventItem.Id);

            Assert.Null(dbEvent);
        }
    }
}