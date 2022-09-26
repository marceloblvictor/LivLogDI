using System.Net.Mail;
using System.Net;
using LivlogDI.Constants;
using LivlogDI.Services.Interfaces;

namespace LivlogDI.Services
{
    public class MessagerService : IMessagerService
    {
        public bool SendEmail(string from, string to, string subject, string body)
        {
            var username = Mailing.USERNAME; // Mailtrap
            var password = Mailing.PASSWORD; // Mailtrap
            var host = Mailing.HOST;
            var port = Mailing.PORT;

            var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            try
            {
                // TODO: código comentado para não encher o inbox do Mailtrap.
                //client.Send(from, to, subject, body);            
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return true;
        }
    }
}
