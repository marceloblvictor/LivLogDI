using LivlogDI.Enums;
using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;

namespace LivlogDI.Services.Interfaces
{
    public interface ICustomerBookService
    {
        List<CustomerBookDTO> AddToWaitingList(CustomerBooksRequestDTO request);
        DateTime CalculateDueDate(CustomerDTO customer, DateTime startTime);
        CustomerBookDTO CreateDTO(CustomerBook customerBook);
        IList<CustomerBookDTO> CreateDTOs(IList<CustomerBook> customerBooks);
        CustomerBook CreateEntity(CustomerBookDTO dto);
        bool Delete(int id);
        IList<CustomerBookDTO> FilterByBooks(IList<CustomerBookDTO> dtos, IList<int> bookIds);
        IList<CustomerBookDTO> FilterByCustomer(IList<CustomerBookDTO> dtos, int customerId);
        IList<CustomerBookDTO> FilterByCustomerAndBook(IList<CustomerBookDTO> dtos, int customerId, IList<int> bookIds);
        IList<CustomerBookDTO> FilterByIds(IEnumerable<CustomerBookDTO> customerBooks, IList<int> ids);
        IList<CustomerBookDTO> FilterByStatus(IList<CustomerBookDTO> dtos, BookRentalStatus status);
        CustomerBookDTO Get(int id);
        IList<CustomerBookDTO> GetAll();
        IList<CustomerBookDTO> GetByCostumerAndBook(int customerId, int bookId);
        IList<CustomerBookDTO> GetByCustomer(int customerId);
        int GetCustomerCategoryDaysDuration(CustomerCategory category);
        int GetCustomerRentalsMaxLimit(CustomerCategory category);
        int GetOverdueDays(DateTime dueDate, DateTime returnDate);
        IList<CustomerBookDTO> GetWaitingList(int bookId);
        bool IsReturnedBookOverdue(CustomerBookDTO customerBook, DateTime returnDate);
        bool RemoveFromWaitingList(int customerBookId);
        bool RemoveFromWaitingQueueByCustomerAndBook(int customerId, int bookId);
        IEnumerable<CustomerBookDTO> RenewBookRental(IList<int> customerBookIds);
        IList<CustomerBookDTO> RentBooks(CustomerBooksRequestDTO request);
        IList<CustomerBookDTO> ReturnBooks(IList<int> customerBookIds);
        IList<CustomerBookDTO> SetCustomerBookStatusToReturned(IList<CustomerBookDTO> returnedCustomerBooks);
        CustomerBookDTO Update(int customerBookId, CustomerBookDTO customerBookDTO);
    }
}