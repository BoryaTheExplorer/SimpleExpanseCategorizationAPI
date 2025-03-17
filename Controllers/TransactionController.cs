using ExpanseCategorizationAPI.Data;
using ExpanseCategorizationAPI.Models;
using ExpanseCategorizationAPI.Services;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Text.Json;




namespace ExpanseCategorizationAPI.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controllers]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly CategorizationService _categorizationService;
        private readonly StackExchange.Redis.IDatabase _cache;
        public TransactionController(AppDbContext context, IConnectionMultiplexer redis)
        {
            _appDbContext = context;
            _categorizationService = new CategorizationService();
            _cache = redis.GetDatabase();
        }

        [HttpPost]
        public async Task<IActionResult> AddTransaction([FromBody] Transaction transaction)
        {
            transaction.Category = _categorizationService.CategorizeTransaction(transaction.Description);
            _appDbContext.Transactions.Add(transaction);
            await _appDbContext.SaveChangesAsync();

            await _cache.StringSetAsync($"transaction: {transaction.Id}", JsonSerializer.Serialize(transaction));
            return CreatedAtAction(nameof(GetTransaction), new { id = transaction.Id }, transaction);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetTransaction(int id)
        {
            var cachedTransaction = await _cache.StringGetAsync($"transaction:{id}");
            if (!cachedTransaction.IsNullOrEmpty) return Ok(JsonSerializer.Deserialize<Transaction>(cachedTransaction));

            var transaction = await _appDbContext.Transactions.FindAsync(id);
            if (transaction == null) return NotFound();

            return Ok(transaction);
        }
    }
}