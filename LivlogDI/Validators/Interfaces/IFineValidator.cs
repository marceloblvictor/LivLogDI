using LivlogDI.Models.DTO;

namespace LivlogDI.Validators.Interfaces
{
    public interface IFineValidator
    {
        void ValidateFineToBePaid(FineDTO fineDto, decimal amountPaid);
    }
}