using CurrencyExchangeApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace CurrencyExchangeApi.Data
{
    public class CurrencyDbContext : DbContext
    {
        public CurrencyDbContext(DbContextOptions<CurrencyDbContext> options)
        : base(options) { }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }
    }
}
