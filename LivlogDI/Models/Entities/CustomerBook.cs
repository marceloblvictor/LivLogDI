using LivlogDI.Enums;

namespace LivlogDI.Models.Entities
{
    public class CustomerBook
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int CustomerId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public BookRentalStatus Status { get; set; }

        public Book Book { get; set; }
        public Customer Customer { get; set; }
    }
}
