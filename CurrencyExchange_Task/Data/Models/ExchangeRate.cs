namespace CurrencyExchangeApi.Data.Models
{
    public class ExchangeRate
    {
        public int Id { get; set; }
        public string CurrencyPair { get; set; }
        public decimal Rate { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
