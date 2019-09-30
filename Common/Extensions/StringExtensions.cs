using System.IO;
using System.Text.RegularExpressions;

namespace Common.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        ///     Checks the string if it is a valid email
        /// </summary>
        /// <param name="email">String to be checked</param>
        /// <returns>true if email format is valid</returns>
        public static bool IsValidEmail(this string email)
        {
            var rx = new Regex(@"^[a-zA-Z0-9._-]+@[a-zA-Z0-9._-]+\.[a-zA-Z]+$");
            return rx.IsMatch(email);
        }

        public static bool IsValidFilePath(this string path)
        {
            try
            {
                // ReSharper disable once UnusedVariable
                // Used to determine if the File path is a valid input for the FileInfo class
                var info = new FileInfo(path);
            }
            catch
            {
                return false;
            }


            return true;
        }
    }
}