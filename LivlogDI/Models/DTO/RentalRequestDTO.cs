namespace LivlogDI.Models.DTO
{
    public class CustomerBooksRequestDTO
    {        
        public int CustomerId { get; set; }
        public IList<int> BookIds { get; set; }
    }
}
