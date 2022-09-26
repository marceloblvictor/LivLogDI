using LivlogDI.Enums;
using LivlogDI.Models.DTO;
using LivlogDI.Validators.Interfaces;

namespace LivlogDI.Validators
{
    public class BookRentalValidator : IBookRentalValidator
    {
        public void ValidateCustomerNotInDebt(
            IList<FineDTO> allFines,
            CustomerDTO customer)
        {
            var isInDebt = allFines
                .Where(f => f.CustomerId == customer.Id &&
                            f.Status == FineStatus.Active)
                .Any();

            if (isInDebt)
            {
                throw new Exception("Este usuário não pode alugar livros pois possui multas não pagas");
            }
        }

        public void ValidateCustomerRentalsLimit(
            IList<CustomerBookDTO> activeCustomerBooks,
            IList<BookDTO> requestedBooks,
            int customerMaxLimit)
        {
            var requestedAmount = requestedBooks.Count;

            var remainingAmount = customerMaxLimit - activeCustomerBooks.Count;

            if (requestedAmount > remainingAmount)
                throw new Exception($"O cliente não pode utrapassar o limite de {customerMaxLimit} empréstimos");
        }

        public void ValidateBookAvailability(
            IList<BookDTO> requestedBooks,
            IList<CustomerBookDTO> allBooks)
        {
            foreach (var book in requestedBooks)
            {
                // Conta quantos exemplares do livro requisitado estão alugados
                int booksTakenCount = allBooks
                    .Where(b => b.BookId == book.Id &&
                                b.Status == BookRentalStatus.Active)
                    .Count();

                if (booksTakenCount >= book.Quantity)
                    throw new Exception($"Não existem mais exemplares disponíveis para empréstimo do livro '{book.Title}'");
            }
        }

        public void ValidateCustomerBooksSameCustomer(
            IList<CustomerBookDTO> returnedBooks)
        {
            bool isSameCustomer = returnedBooks
                .Select(cb => cb.CustomerId)
                .Distinct()
                .Count() == 1;

            if (!isSameCustomer)
            {
                throw new Exception("O retorno de livros ou a renovação do empréstimo só podem ser feito em relação a um usuário por vez");
            }
        }

        public void ValidateReturnedBookStatus(CustomerBookDTO returnedBook)
        {
            if (returnedBook.Status == BookRentalStatus.WaitingQueue)
            {
                throw new Exception("Livros em espera não podem ser devolvidos");
            }

            if (returnedBook.Status == BookRentalStatus.Returned)
            {
                throw new Exception("Este livro já foi devolvido");
            }
        }

        public void ValidateBookWaitingQueue(
            IList<CustomerBookDTO> bookWaitingList,
            IList<FineDTO> allFines,
            int customerId)
        {
            var customersInDebt = allFines
                .Where(f => f.Status == FineStatus.Active)
                .Select(f => f.CustomerId)
                .ToList();

            bool isThereWaitingList = bookWaitingList.Count() > 0;
            bool isCustomerInWaitingQueue = bookWaitingList.Select(cb => cb.CustomerId).Contains(customerId);
            bool isCustomerInDebt = bookWaitingList.Any(b => !customersInDebt.Contains(b.CustomerId));

            // Verifica se tem algum cliente na lista de esperas sem débitos e se o cliente não está na lista de espera
            if (isThereWaitingList &&
                !isCustomerInWaitingQueue &&
                !isCustomerInDebt)
            {
                throw new Exception("O empréstimo do livro não pode ser iniciado ou renovado porque existem clientes na fila de espera");
            }
        }

        public void ValidateAnyFreeBook(
            IList<CustomerBookDTO> activeCustomerBooks,
            int bookQuantity)
        {
            if (activeCustomerBooks.Count() < bookQuantity)
            {
                throw new Exception("Existem exemplares disponíveis para empréstimo do livro especificado");
            }
        }

        public void ValidateRenewalOnlyInDueDate(
            IList<CustomerBookDTO> booksToBeRenewed)
        {
            if (booksToBeRenewed.Any(b => b.DueDate.Value.DayOfYear != DateTime.Now.DayOfYear))
            {
                throw new Exception("Empréstimos só podem ser renovados na data de devolução do livro");
            }
        }

        public void ValidateIfCustomerIsAlreadyInQueueOrHasTheBook(
            CustomerDTO customer,
            int bookId,
            IList<CustomerBookDTO> allCustomerBooks)
        {
            if (allCustomerBooks.Any(cb => cb.CustomerId == customer.Id &&
                                      cb.BookId == bookId &&
                                      cb.Status == BookRentalStatus.WaitingQueue))
            {
                throw new Exception("O cliente já se encontra na lista de espera do livro");
            }

            if (allCustomerBooks.Any(cb => cb.CustomerId == customer.Id &&
                                      cb.BookId == bookId &&
                                      cb.Status == BookRentalStatus.Active))
            {
                throw new Exception("O cliente já se encontra na posse do livro");
            }
        }

        public void ValidateWaitedBookStatus(CustomerBookDTO customerBook)
        {
            if (customerBook.Status != BookRentalStatus.WaitingQueue)
            {
                throw new Exception("Não pode ser removido da lista de espera um aluguel ativo ou concluído");
            }
        }
    }
}
