using LivlogDI.Enums;

namespace LivlogDI.Models.DTO
{
    public class FineDTO
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public FineStatus Status { get; set; }

        public string CustomerName { get; set; }
        public int CustomerId { get; set; }
    }
}