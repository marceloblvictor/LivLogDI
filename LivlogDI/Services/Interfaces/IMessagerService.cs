namespace LivlogDI.Services.Interfaces
{
    public interface IMessagerService
    {
        bool SendEmail(string from, string to, string subject, string body);
    }
}