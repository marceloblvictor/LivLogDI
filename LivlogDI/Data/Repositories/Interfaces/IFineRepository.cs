using LivlogDI.Models.Entities;

namespace LivlogDI.Data.Repositories.Interfaces
{
    public interface IFineRepository
    {
        Fine Add(Fine fine);
        bool Delete(int fineId);
        Fine Get(int fineId);
        List<Fine> GetAll();
        Fine Update(Fine fine);
    }
}