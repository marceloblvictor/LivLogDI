using LivlogDI.Enums;

namespace LivlogDI.Models.Entities
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public CustomerCategory Category { get; set; }

        public ICollection<CustomerBook> CustomerBooks { get; set; } = new List<CustomerBook>();
        public ICollection<Fine> Fines { get; set; } = new List<Fine>();
    }
}