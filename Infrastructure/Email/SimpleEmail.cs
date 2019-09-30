using Newtonsoft.Json;

namespace Infrastructure.Email
{
    public class SimpleEmail : IEmail
    {
        #region Implementation of IEmail

        public string To { get; set; }

        public string From { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        #endregion

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}