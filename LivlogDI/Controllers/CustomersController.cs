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
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomersController(
            ICustomerService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obter todos os clientes.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<CustomerDTO>> GetAll()
            => Ok(_service.GetAll());

        /// <summary>
        /// Obter um cliente por Id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<CustomerDTO> Get(int id)
            => Ok(_service.Get(id));

        /// <summary>
        /// Criar um cliente.
        /// </summary>
        /// <param name="customerDTO"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<CustomerDTO> Create(CustomerDTO customerDTO)
            => Ok(_service.Create(customerDTO));

        /// <summary>
        /// Atualizar um cliente.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customerDTO"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public ActionResult<CustomerDTO> Update(int id, CustomerDTO customerDTO)
            => Ok(_service.Update(id, customerDTO));

        /// <summary>
        /// Deletar um cliente.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ActionResult<bool> Delete(int id) 
            => Ok(_service.Delete(id));
    }
}
