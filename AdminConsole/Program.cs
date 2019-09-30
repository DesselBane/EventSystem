using System;
using System.Linq;
using Common.Extensions;
using Common.Security;
using Infrastructure.DataModel;
using Infrastructure.DataModel.People;
using Infrastructure.DataModel.Security;
using Infrastructure.DataModel.Security.Claims;
using Infrastructure.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MsSqlContext;
using MySqlContext;

namespace AdminConsole
{
    internal static class Program
    {
        private static IDesignTimeDbContextFactory<DataContext> _factory;
        
        private static void Main(string[] args)
        {
            Initialize();
            Console.WriteLine("Hello Master");
            Console.WriteLine("What would you like to do?");
            Console.WriteLine("\n\t1. Create new Admin User");
            Console.WriteLine("\t2. Give existing User admin rights");
            Console.WriteLine("\nIf you want to exit pleas type !q");

            string input;
            while ((input = Console.ReadLine()) != "!q")
                switch (input)
                {
                    case "1":
                        MakeUserAdmin(CreateUser().Id);
                        break;
                    case "2":
                        MakeUserAdmin();
                        break;
                }
        }

        private static void Initialize()
        {
            Console.WriteLine("Initializing...");
            var options = DataContextFactory.GetConfiguration();

            switch (options.Provider)
            {
                case DatabaseProviders.MySql:
                    _factory = new MySqlDataContextFactory();
                    break;
                case DatabaseProviders.MsSql:
                    _factory = new MsSqlDataContextFactory();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

        }
        
        private static User CreateUser()
        {
            using (var ctx = _factory.CreateDbContext(new string[0]))
            {
                Console.WriteLine("Username (must be an Email): ");
                string username;
                do
                {
                    username = Console.ReadLine();
                } while (!username.IsValidEmail());

                Console.WriteLine("Password");
                var password = Console.ReadLine();

                var user = JwtAuthenticationService.CreateNewUser(username);
                JwtAuthenticationService.UpdatePassword(user,password);
                ctx.Users.Add(user);
                ctx.SaveChanges();

                var person = new RealPerson
                             {
                                 Firstname = "Mr.",
                                 Lastname = "Administrator",
                                 UserId = user.Id
                             };

                ctx.RealPeople.Add(person);
                ctx.SaveChanges();
                
                return user;
            }
        }

        private static void MakeUserAdmin()
        {
            using (var ctx = _factory.CreateDbContext(new string[0]))
            {
                ctx.Users.Load();
                foreach (var user in ctx.Users)
                {
                    Console.WriteLine($"Id: {user.Id}\tUsername: {user.EMail}");
                }

                Console.WriteLine("Please choose a User by Id");

                int userId;
                while (!int.TryParse(Console.ReadLine(),out userId))
                {
                    Console.WriteLine("Input could not be parsed");
                }
                
                MakeUserAdmin(userId);
            }
        }
        
        private static void MakeUserAdmin(int userId)
        {
            using (var ctx = _factory.CreateDbContext(new string[0]))
            {
                var userClaim = new UserClaim
                {
                    Issuer = "AdminConsole",
                    OriginalIssuer = "ES_Authority",
                    Type = RoleClaim.ROLE_CLAIM_TYPE,
                    User_Id = userId,
                    Value = "1"
                };
                ctx.Claims.Add(userClaim);
                ctx.SaveChanges();
                Console.WriteLine("Permisions granted");
            }
        }
    }
}