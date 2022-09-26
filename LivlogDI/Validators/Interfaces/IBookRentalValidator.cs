using LivlogDI.Models.DTO;

namespace LivlogDI.Validators.Interfaces
{
    public interface IBookRentalValidator
    {
        void ValidateAnyFreeBook(IList<CustomerBookDTO> activeCustomerBooks, int bookQuantity);
        void ValidateBookAvailability(IList<BookDTO> requestedBooks, IList<CustomerBookDTO> allBooks);
        void ValidateBookWaitingQueue(IList<CustomerBookDTO> bookWaitingList, IList<FineDTO> allFines, int customerId);
        void ValidateCustomerBooksSameCustomer(IList<CustomerBookDTO> returnedBooks);
        void ValidateCustomerNotInDebt(IList<FineDTO> allFines, CustomerDTO customer);
        void ValidateCustomerRentalsLimit(IList<CustomerBookDTO> activeCustomerBooks, IList<BookDTO> requestedBooks, int customerMaxLimit);
        void ValidateIfCustomerIsAlreadyInQueueOrHasTheBook(CustomerDTO customer, int bookId, IList<CustomerBookDTO> allCustomerBooks);
        void ValidateRenewalOnlyInDueDate(IList<CustomerBookDTO> booksToBeRenewed);
        void ValidateReturnedBookStatus(CustomerBookDTO returnedBook);
        void ValidateWaitedBookStatus(CustomerBookDTO customerBook);
    }
}