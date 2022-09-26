using LivlogDI.Enums;
using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;

namespace LivlogDI.Services.Interfaces
{
    public interface IFineService
    {
        decimal CalculateFineAmount(CustomerCategory category, int overdueDays);
        FineDTO Create(FineDTO fineDTO);
        FineDTO CreateDTO(Fine fine);
        IEnumerable<FineDTO> CreateDTOs(IEnumerable<Fine> fines);
        Fine CreateEntity(FineDTO dto);
        bool Delete(int id);
        IList<FineDTO> FilterByCustomer(IEnumerable<FineDTO> fines, int customerId);
        IList<FineDTO> FilterByIds(IEnumerable<FineDTO> fines, IList<int> ids);
        IList<FineDTO> FilterByStatus(IEnumerable<FineDTO> fines, FineStatus status);
        FineDTO FineCustomer(int customerId, int daysOverdue);
        FineDTO Get(int fineid);
        IEnumerable<FineDTO> GetAll();
        decimal GetCategoryFineRate(CustomerCategory category);
        decimal GetCustomerDebts(int customerId);
        FineDTO SetFineStatusToPaid(FineDTO fineDto);
        FineDTO UpdateFineToPaid(int fineId, decimal amountPaid);
    }
}