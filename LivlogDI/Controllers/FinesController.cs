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
    public class FinesController : ControllerBase
    {
        private readonly IFineService _service;

        public FinesController(
            IFineService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obter todas as multas.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<IEnumerable<FineDTO>> GetAll()
            => Ok(_service.GetAll());

        /// <summary>
        /// Obter o valor total devido por um cliente.
        /// </summary>
        /// <returns></returns>
        [HttpGet("debts/{customerId}")]
        public ActionResult<decimal> GetCustomerDebts(int customerId)
            => Ok(_service.GetCustomerDebts(customerId));

        /// <summary>
        /// Registrar pagamento de multa por cliente.
        /// </summary>
        /// <returns></returns>
        [HttpPatch("{fineId}")]
        public ActionResult<BookDTO> RegisterPayment(int fineId, decimal amountPaid)
            => Ok(_service.UpdateFineToPaid(fineId, amountPaid));

        /// <summary>
        /// Deletar uma multa.
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ActionResult<bool> Delete(int id)
            => Ok(_service.Delete(id));
    }
}
