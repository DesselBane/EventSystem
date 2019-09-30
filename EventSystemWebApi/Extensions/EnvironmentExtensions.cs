using Microsoft.AspNetCore.Hosting;

namespace EventSystemWebApi.Extensions
{
    public static class EnvironmentExtensions
    {
        public static bool IsCustomDevelopment(this IHostingEnvironment env)
        {
            switch (env.EnvironmentName.ToLower())
            {
                case "production":
                case "staging":
                case "integrationTest":
                    return false;

                default:
                    return true;
            }
        }

        public static bool IsIntegrationTesting(this IHostingEnvironment env)
        {
            return env.EnvironmentName == "integrationTest";
        }
    }
}