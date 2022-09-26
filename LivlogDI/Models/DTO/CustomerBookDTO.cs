using LivlogDI.Enums;

namespace LivlogDI.Models.DTO
{
    public class CustomerBookDTO
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? DueDate { get; set; }
        public BookRentalStatus Status { get; set; }        
    }
}
