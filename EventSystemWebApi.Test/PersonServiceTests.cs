using Autofac;
using Common.People;
using Common.Test;
using Infrastructure.DataModel.People;
using Xunit;

namespace EventSystemWebApi.Test
{
    public class PersonServiceTests : TestBase
    {
        protected override void BuildUp(ContainerBuilder builder)
        {
        }

        [Fact]
        public void UpdatePerson_Everything()
        {
            const string FIRSTNAME = "this is the new Firstname";
            const string LASTNAME = "this is the new Lastname";
            byte[] picture = {1, 12, 24, 56, 111, 23, 56, 89};
            byte[] oldPicture = {1, 45, 66, 7, 87, 44};

            var oldPerson = new RealPerson
            {
                Firstname = "firsntafkn",
                Lastname = "Lastname",
                ProfilePicture = oldPicture
            };

            var newPerson = new RealPerson
            {
                Firstname = FIRSTNAME,
                Lastname = LASTNAME,
                ProfilePicture = picture
            };

            PersonService.UpdatePerson(newPerson, oldPerson);

            Assert.Equal(newPerson.Firstname, oldPerson.Firstname);
            Assert.Equal(newPerson.Lastname, oldPerson.Lastname);
            Assert.Equal(oldPicture, oldPerson.ProfilePicture);
        }

        [Fact]
        public void UpdatePerson_HonorNulls()
        {
            const string FIRSTNAME = "this is the new Firstname";
            const string LASTNAME = "this is the new Lastname";
            byte[] picture = {1, 12, 24, 56, 111, 23, 56, 89};

            var oldPerson = new RealPerson
            {
                Firstname = FIRSTNAME,
                Lastname = LASTNAME,
                ProfilePicture = picture
            };

            var newPerson = new RealPerson
            {
                Firstname = "",
                Lastname = "",
                ProfilePicture = null
            };

            PersonService.UpdatePerson(newPerson, oldPerson);

            Assert.Equal("", oldPerson.Firstname);
            Assert.Equal("", oldPerson.Lastname);
            Assert.Equal(picture, oldPerson.ProfilePicture);
        }
    }
}