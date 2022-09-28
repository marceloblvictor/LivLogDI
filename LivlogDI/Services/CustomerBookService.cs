using LivlogDI.Constants;
using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Enums;
using LivlogDI.Models.DTO;
using LivlogDI.Models.Entities;
using LivlogDI.Services.Interfaces;
using LivlogDI.Validators.Interfaces;
using static LivlogDI.Enums.CustomerCategory;

namespace LivlogDI.Services
{
    public class CustomerBookService : ICustomerBookService
    {
        private readonly ICustomerBookRepository _repo;
        private readonly ICustomerService _customerService;
        private readonly IBookService _bookService;
        private readonly IFineService _fineService;
        private readonly IBookRentalValidator _rentalValidator;
        private readonly IMessagerService _messager;

        public CustomerBookService(
            ICustomerBookRepository repo,
            ICustomerService customerService,
            IBookService bookService,
            IFineService fineService,
            IBookRentalValidator rentalValidator,
            IMessagerService messagerService)
        {
            _repo = repo;
            _customerService = customerService;
            _bookService = bookService;
            _fineService = fineService;
            _rentalValidator = rentalValidator;
            _messager = messagerService;
        }

        public CustomerBookDTO Get(int id)
        {
            var customerBook = _repo.Get(id);

            return CreateDTO(customerBook);
        }

        public IList<CustomerBookDTO> GetAll()
        {
            var customerBooks = _repo.GetAll();

            return CreateDTOs(customerBooks);
        }

        public IList<CustomerBookDTO> GetByCustomer(int customerId)
        {
            var allCustomerBooks = GetAll();
            var customerBooks = FilterByCustomer(allCustomerBooks, customerId);

            return customerBooks;
        }

        public IList<CustomerBookDTO> GetByCostumerAndBook(
            int customerId,
            int bookId)
        {
            return GetAll()
                .Where(cb => cb.CustomerId == customerId &&
                             cb.BookId == bookId)
                .ToList();
        }

        public CustomerBookDTO Update(int customerBookId, CustomerBookDTO customerBookDTO)
        {
            var customerBook = _repo.Get(customerBookId);

            customerBook.StartDate = customerBookDTO.StartDate;
            customerBook.DueDate = customerBookDTO.DueDate;
            customerBook.Status = customerBookDTO.Status;

            var updatedBook =
                _repo.Update(customerBook);

            return CreateDTO(updatedBook);
        }

        public bool Delete(int id)
        {
            return _repo.Delete(id);
        }

        public IList<CustomerBookDTO> RentBooks(CustomerBooksRequestDTO request)
        {
            // Obter cliente 
            var customer = _customerService.Get(request.CustomerId);

            // Verifica se o usuário possui alguma dívida em aberto
            var allFines = _fineService
                .GetAll()
                .ToList();

            _rentalValidator.ValidateCustomerNotInDebt(
                allFines,
                customer);

            // Obter os livros requisitados
            var requestedBooks = _bookService.FilterByIds(
                _bookService.GetAll(),
                request.BookIds);

            // Obter todos os livros de todos os clientes
            var allCustomerBooks = GetAll();

            // Filtrar os livros de clientes pertencentes ao cliente que fez o request
            var customerBooks = FilterByCustomer(
                allCustomerBooks, customer.Id);

            // Filtrar os livros do cliente que tenham o status "Ativo"
            var activeCustomerBooks = FilterByStatus(
                customerBooks, BookRentalStatus.Active);

            // Validar se o cliente pode alugar mais livros
            _rentalValidator.ValidateCustomerRentalsLimit(
                activeCustomerBooks,
                requestedBooks,
                GetCustomerRentalsMaxLimit(customer.Category));

            // Validar se existem livros suficientes no estoque
            _rentalValidator.ValidateBookAvailability(
                requestedBooks,
                allCustomerBooks);

            DateTime currentDateTime = DateTime.Now;

            // Calcula o prazo do empréstimo do cliente baseado em sua categoria
            var customerDueDate = CalculateDueDate(customer, currentDateTime);

            var customerBooksCreated = new List<CustomerBook>();

            // Cria uma nova entidade CustomerBook para cada livro alugado além de remover o registro da lista de espera
            foreach (int bookId in request.BookIds)
            {
                var allBooksWaitingList = FilterByStatus(
                    GetAll(),
                    BookRentalStatus.WaitingQueue);

                var bookWaitingList = FilterByBooks(
                    allBooksWaitingList,
                    new List<int>() { bookId });

                _rentalValidator.ValidateBookWaitingQueue(
                    bookWaitingList,
                    allFines,
                    customer.Id);

                RemoveFromWaitingQueueByCustomerAndBook(customer.Id, bookId);

                var bookCreated = _repo.Add(CreateEntity(new CustomerBookDTO()
                {
                    CustomerId = request.CustomerId,
                    BookId = bookId,
                    StartDate = currentDateTime,
                    DueDate = customerDueDate,
                    Status = BookRentalStatus.Active
                }));

                customerBooksCreated.Add(bookCreated);

                _messager.SendEmail(
                    "from@example.com",
                    "to@example.com",
                    Messages.BOOK_RENTED_SUBJECT,
                    string.Format(Messages.BOOK_RENTED_BODY, customer.Name, bookCreated.Book.Title, bookCreated.DueDate));
            }

            // Retorna uma lista com informações dos livros alugados pelo cliente
            var rentedBookDtos = CreateDTOs(customerBooksCreated);

            return rentedBookDtos;
        }

        public IList<CustomerBookDTO> ReturnBooks(IList<int> customerBookIds)
        {
            var returnedCustomerBooks = FilterByIds(
                GetAll(),
                customerBookIds);

            _rentalValidator.ValidateCustomerBooksSameCustomer(
                returnedCustomerBooks);

            DateTime currentDateTime = DateTime.Now;

            // TODO: apagar próxima linha! SOMENTE PARA TESTE DE LIVROS ATRASADOS!!
            // currentDateTime = currentDateTime.AddDays(20);

            // Verifica se tem algum livro em atraso e cria multa, caso exista
            foreach (var returnedBook in returnedCustomerBooks)
            {
                if (IsReturnedBookOverdue(returnedBook, currentDateTime))
                {
                    var overdueDays = GetOverdueDays(
                        returnedBook.DueDate.Value,
                        currentDateTime);

                    _fineService.FineCustomer(
                        returnedBook.CustomerId,
                        overdueDays);
                }
            }

            returnedCustomerBooks = SetCustomerBookStatusToReturned(
                returnedCustomerBooks);

            foreach (var returnedBook in returnedCustomerBooks)
            {
                Update(returnedBook.Id, returnedBook);
            }

            return returnedCustomerBooks;
        }

        public IEnumerable<CustomerBookDTO> RenewBookRental(IList<int> customerBookIds)
        {
            var booksToBeRenewed = FilterByIds(
                GetAll(),
                customerBookIds);

            _rentalValidator.ValidateCustomerBooksSameCustomer(
                booksToBeRenewed);

            _rentalValidator.ValidateRenewalOnlyInDueDate(
                booksToBeRenewed);

            var customer = _customerService.Get(
                booksToBeRenewed.First().CustomerId);

            var allFines = _fineService
                .GetAll()
                .ToList();

            _rentalValidator.ValidateCustomerNotInDebt(
                allFines,
                customer);

            var allBooksWaitingList = FilterByStatus(
                GetAll(),
                BookRentalStatus.WaitingQueue);

            foreach (var bookToBeRenewed in booksToBeRenewed)
            {
                var bookWaitingList = FilterByBooks(
                    allBooksWaitingList,
                    new List<int>() { bookToBeRenewed.BookId });

                _rentalValidator.ValidateBookWaitingQueue(
                    bookWaitingList,
                    allFines,
                    customer.Id);

                // Renova o prazo de devolução
                bookToBeRenewed.DueDate = CalculateDueDate(
                    customer,
                    bookToBeRenewed.DueDate.Value);

                Update(bookToBeRenewed.Id, bookToBeRenewed);
            }

            return booksToBeRenewed;
        }

        public IList<CustomerBookDTO> GetWaitingList(int bookId)
        {
            var waitedBooks = FilterByStatus(
                GetAll(),
                BookRentalStatus.WaitingQueue);

            var bookWaitingList = FilterByBooks(
                waitedBooks,
                new List<int> { bookId });

            return bookWaitingList;
        }

        public List<CustomerBookDTO> AddToWaitingList(CustomerBooksRequestDTO request)
        {
            var customer = _customerService.Get(request.CustomerId);

            var allCustomerBooks = GetAll();

            var booksToWait = _bookService.FilterByIds(
                _bookService.GetAll(),
                request.BookIds);

            var waitedBooks = new List<CustomerBookDTO>();

            var activeCustomerBooks =
                    FilterByStatus(
                        FilterByBooks(
                            GetAll(),
                            request.BookIds),
                        BookRentalStatus.Active);

            foreach (var bookToWait in booksToWait)
            {
                _rentalValidator.ValidateIfCustomerIsAlreadyInQueueOrHasTheBook(
                    customer,
                    bookToWait.Id,
                    allCustomerBooks);

                var bookQuantity = _bookService.GetBookQuantity(
                    bookToWait);

                _rentalValidator.ValidateAnyFreeBook(
                    activeCustomerBooks,
                    bookQuantity);

                var waitingBook = CreateEntity(new CustomerBookDTO()
                {
                    BookId = bookToWait.Id,
                    CustomerId = customer.Id,
                    StartDate = null,
                    DueDate = null,
                    Status = BookRentalStatus.WaitingQueue
                });

                waitedBooks.Add(
                    CreateDTO(_repo.Add(waitingBook)));
            }

            return waitedBooks;
        }

        public bool RemoveFromWaitingList(int customerBookId)
        {
            var customerBook = Get(customerBookId);

            _rentalValidator.ValidateWaitedBookStatus(customerBook);

            return Delete(customerBookId);
        }

        public bool RemoveFromWaitingQueueByCustomerAndBook(int customerId, int bookId)
        {
            var customerBooks = GetByCostumerAndBook(
                customerId,
                bookId);

            var waitedCustomerBooks = FilterByStatus(
                customerBooks,
                BookRentalStatus.WaitingQueue);

            foreach (var waitedCustomerBook in waitedCustomerBooks)
            {
                RemoveFromWaitingList(waitedCustomerBook.Id);
            }

            return true;
        }        

        #region Helper Methods

        public IList<CustomerBookDTO> SetCustomerBookStatusToReturned(
            IList<CustomerBookDTO> returnedCustomerBooks)
        {
            foreach (var returnedBook in returnedCustomerBooks)
            {
                _rentalValidator.ValidateReturnedBookStatus(returnedBook);

                returnedBook.Status = BookRentalStatus.Returned;
            }

            return returnedCustomerBooks;
        }

        public int GetOverdueDays(DateTime dueDate, DateTime returnDate)
        {
            return (returnDate - dueDate).Days;
        }

        public bool IsReturnedBookOverdue(
            CustomerBookDTO customerBook,
            DateTime returnDate)
        {            
            return returnDate.CompareTo(customerBook.DueDate) > 0;
        }

        public DateTime CalculateDueDate(CustomerDTO customer, DateTime startTime)
        {
            return startTime.AddDays(
                GetCustomerCategoryDaysDuration(customer.Category));
        }

        public int GetCustomerCategoryDaysDuration(CustomerCategory category)
            => category switch
            {
                Top => 15,
                Medium => 10,
                Low => 5,
                _ => throw new ArgumentException()
            };

        public int GetCustomerRentalsMaxLimit(CustomerCategory category)
            => category switch
            {
                Top => 5,
                Medium => 3,
                Low => 1,
                _ => throw new ArgumentException()
            };

        public IList<CustomerBookDTO> FilterByCustomer(
            IList<CustomerBookDTO> dtos,
            int customerId)
        {
            return dtos
                .Where(dto => dto.CustomerId == customerId)
                .ToList();
        }

        public IList<CustomerBookDTO> FilterByBooks(
            IList<CustomerBookDTO> dtos,
            IList<int> bookIds)
        {
            return dtos
                .Where(dto => bookIds.Contains(dto.BookId))
                .ToList();
        }

        public IList<CustomerBookDTO> FilterByCustomerAndBook(
            IList<CustomerBookDTO> dtos,
            int customerId,
            IList<int> bookIds)
        {
            return dtos
                .Where(dto => dto.CustomerId == customerId &&
                              bookIds.Contains(dto.BookId))
                .ToList();
        }

        public IList<CustomerBookDTO> FilterByStatus(
            IList<CustomerBookDTO> dtos,
            BookRentalStatus status)
        {
            return dtos
                .Where(dto => dto.Status == status)
                .ToList();
        }

        public IList<CustomerBookDTO> FilterByIds(
            IEnumerable<CustomerBookDTO> customerBooks,
            IList<int> ids)
        {
            return customerBooks
                .Where(cb => ids.Contains(cb.Id))
                .ToList();
        }

        public CustomerBookDTO CreateDTO(CustomerBook customerBook)
        {
            return new CustomerBookDTO
            {
                Id = customerBook.Id,
                BookId = customerBook.BookId,
                BookTitle = customerBook.Book.Title,
                CustomerId = customerBook.CustomerId,
                CustomerName = customerBook.Customer.Name,
                StartDate = customerBook.StartDate,
                DueDate = customerBook.DueDate,
                Status = customerBook.Status
            };
        }

        public IList<CustomerBookDTO> CreateDTOs(IList<CustomerBook> customerBooks)
        {
            var customerBooksDtos = new List<CustomerBookDTO>();

            foreach (var customerBook in customerBooks)
            {
                customerBooksDtos.Add(CreateDTO(customerBook));
            }

            return customerBooksDtos;
        }

        public CustomerBook CreateEntity(
            CustomerBookDTO dto)
        {
            return new CustomerBook
            {
                Id = dto.Id,
                BookId = dto.BookId,
                CustomerId = dto.CustomerId,
                StartDate = dto.StartDate,
                DueDate = dto.DueDate,
                Status = dto.Status
            };
        }

        #endregion
    }
}
