namespace LivlogDI.Models.Entities
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public int Quantity { get; set; }

        public ICollection<CustomerBook> CustomerBooks { get; set; } = new List<CustomerBook>();
    }
}
