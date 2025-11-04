using Expenses.API.Data;
using Expenses.API.Data.Services;
using Expenses.API.DTOs;
using Expenses.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Expenses.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAll")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionsService _service;
        public TransactionsController(ITransactionsService service)
        {
            _service = service;
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllTransactions()
        {
            var nameIdentifierClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifierClaim)) return BadRequest("Could not get the user id.");
            if (!int.TryParse(nameIdentifierClaim, out int userId))
                return BadRequest();
            var allTransactions = await _service.GetAll(userId);
            return Ok(allTransactions);
        }

        [HttpGet("Details/{id}")]
        public async Task<IActionResult> GetTransactionById(int id)
        {
            var transaction = await _service.GetById(id);
            if (transaction == null) return NotFound();
            return Ok(transaction);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateTransaction([FromBody]PostTransactionDto payload)
        {
            var nameIdentifierClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(nameIdentifierClaim)) return BadRequest("Could not get the user id.");
            if (!int.TryParse(nameIdentifierClaim, out int userId))
                return BadRequest();
            var newTransaction = await _service.Create(payload, userId);
            return Ok(newTransaction);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateTransaction(int id, [FromBody] PutTransactionDto dto)
        {
            var updateTransaction = await _service.Update(id, dto);
            if (updateTransaction == null) return NotFound();
            return Ok(updateTransaction);


        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            await _service.Delete(id);

            return Ok();
        }
    }
}
