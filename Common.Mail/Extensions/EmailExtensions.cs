using Infrastructure.Email;
using MimeKit;

namespace Common.Mail.Extensions
{
    public static class EmailExtensions
    {
        public static MimeMessage ToMimeMessage(this IEmail mail)
        {
            var mime = new MimeMessage();
            mime.From.Add(new MailboxAddress(mail.From));
            mime.To.Add(new MailboxAddress(mail.To));
            mime.Subject = mail.Subject;
            mime.Body = new TextPart("plain")
            {
                Text = mail.Body
            };

            return mime;
        }
    }
}