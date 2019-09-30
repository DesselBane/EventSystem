using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.DataModel.People;
using Infrastructure.DataModel.Security;

namespace EventSystemWebApi.IntTest.PersonTests
{
    public abstract class PersonControllerTestBase : TestBase
    {
        protected async Task<(User user, List<RealPerson> people)> SetupPeopleAsync()
        {
            var user = CreateUser();
            _Context.Users.Add(user);
            await _Context.SaveChangesAsync();
            await SetupBasicAuthenticationAsync(_Client, user.EMail);

            var users = new List<User>();

            users.AddRange(new[]
            {
                new User
                {
                    Person = new RealPerson
                    {
                        Firstname = "James",
                        Lastname = "Bütt"
                    }
                },
                new User
                {
                    Person = new RealPerson
                    {
                        Firstname = "Josephine",
                        Lastname = "Daräkjy"
                    }
                },
                new User
                {
                    Person = new RealPerson
                    {
                        Firstname = "Art",
                        Lastname = "Venere"
                    }
                },
                new User
                {
                    Person = new RealPerson
                    {
                        Firstname = "Lenna",
                        Lastname = "Papröcki"
                    }
                },
                new User
                {
                    Person = new RealPerson
                    {
                        Firstname = "Donnette",
                        Lastname = "Foller"
                    }
                },
                new User
                {
                    Person = new RealPerson
                    {
                        Firstname = "Simona",
                        Lastname = "Morasca"
                    }
                },
                new User
                {
                    Person = new RealPerson
                    {
                        Firstname = "Mitsue",
                        Lastname = "Tollner"
                    }
                },
                new User
                {
                    Person = new RealPerson
                    {
                        Firstname = "Leota123",
                        Lastname = "Dilliard"
                    }
                }
            });
            _Context.Users.AddRange(users);
            await _Context.SaveChangesAsync();
            return (user, users.Select(x => x.Person).ToList());
        }
    }
}