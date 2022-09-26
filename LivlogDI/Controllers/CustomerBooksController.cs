using System.Diagnostics.CodeAnalysis;
using LivlogDI.Models.DTO;
using LivlogDI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LivlogDI.Controllers
{
    [ExcludeFromCodeCoverage]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CustomerBooksController : ControllerBase
    {
        private readonly ICustomerBookService _service;

        public CustomerBooksController(
            ICustomerBookService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obter todos os livros de clientes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<CustomerBookDTO>> GetAll()
            => Ok(_service.GetAll());

        /// <summary>
        /// Obter um livro de cliente por Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<CustomerBookDTO> Get(int id)
            => Ok(_service.Get(id));

        /// <summary>
        /// Obter os livros de clientes de um determinado cliente.
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpGet("by-customer")]
        public ActionResult<IEnumerable<CustomerBookDTO>> GetByCustomer([FromQuery] int customerId)
            => Ok(_service.GetByCustomer(customerId));

        /// <summary>
        /// Atualizar os dados de um livro de cliente.
        /// </summary>
        /// <returns></returns>
        [HttpPut("{id}")]
        public ActionResult<BookDTO> UpdateBookRentalData(int id, CustomerBookDTO customerBookDTO)
            => Ok(_service.Update(id, customerBookDTO));

        /// <summary>
        /// Deletar um registro de livro de cliente.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ActionResult<bool> Delete(int id) 
            => Ok(_service.Delete(id));

        /// <summary>
        /// Alugar livros.
        /// </summary>
        /// <returns></returns>
        [HttpPost("rent")]
        public ActionResult<CustomerBookDTO> RentBooks([FromBody] CustomerBooksRequestDTO request)
            => Ok(_service.RentBooks(request));

        /// <summary>
        /// Devolver Livros.
        /// </summary>
        /// <returns></returns>
        [HttpPost("return")]
        public ActionResult<CustomerBookDTO> ReturnBooks([FromBody] IList<int> customerbookIds)
            => Ok(_service.ReturnBooks(customerbookIds));

        /// <summary>
        /// Renovar aluguel de livro de cliente.
        /// </summary>
        /// <returns></returns>
        [HttpPost("renewal")]
        public ActionResult<CustomerBookDTO> RenewBookRental(IList<int> customerBookId)
            => Ok(_service.RenewBookRental(customerBookId));

        /// <summary>
        /// Obter a lista de esperas de um determinado livro.
        /// </summary>
        /// <returns></returns>
        [HttpGet("{bookId}/waiting-list")]
        public ActionResult<IEnumerable<CustomerBookDTO>> GetWaitingList([FromRoute] int bookId)
            => Ok(_service.GetWaitingList(bookId));

        /// <summary>
        /// Adicionar cliente na lista de espera por livro.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("waiting-list")]
        public ActionResult<CustomerBookDTO> AddToWaitingList(
            [FromBody] CustomerBooksRequestDTO request) 
                => Ok(_service.AddToWaitingList(request));

        /// <summary>
        /// Remover cliente da lista de espera por livro.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("waiting-list/{customerBookId}")]
        public ActionResult<bool> RemoveFromWaitingList(int customerBookId)
            => Ok(_service.RemoveFromWaitingList(customerBookId));
    }
}
