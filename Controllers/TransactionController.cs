using ExpanseCategorizationAPI.Data;
using ExpanseCategorizationAPI.Models;
using ExpanseCategorizationAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json;




namespace ExpanseCategorizationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly CategorizationService _categorizationService;
        private readonly IMemoryCache _cache;
        public TransactionController(AppDbContext context, IMemoryCache cache)
        {
            _appDbContext = context;
            _categorizationService = new CategorizationService();
            _cache = cache;
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction([FromBody] Transaction transaction)
        {
            transaction.Category = _categorizationService.CategorizeTransaction(transaction.Description);
            _appDbContext.Transactions.Add(transaction);
            await _appDbContext.SaveChangesAsync();

            _cache.Set($"transaction: {transaction.Id}", transaction, TimeSpan.FromMinutes(5));
            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetTransaction(int id)
        {
            if (_cache.TryGetValue($"transaction:{id}", out Transaction cachedTransaction))
            {
                return Ok(cachedTransaction);
            }
            
            var transaction = await _appDbContext.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();

            _cache.Set($"transaction:{id}", transaction, TimeSpan.FromMinutes(5));
            return Ok(transaction);
        }
    }
}