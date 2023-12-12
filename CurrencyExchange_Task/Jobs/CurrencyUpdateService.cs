using CurrencyExchangeApi.Data;
using CurrencyExchangeApi.Data.Models;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Newtonsoft.Json;
using System.Net.Http;

namespace CurrencyExchangeApi.Jobs
{
    public class CurrencyUpdateService : BackgroundService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly CurrencyDbContext _dbContext;
        private readonly ILogger _logger;

        public CurrencyUpdateService(IHttpClientFactory httpClientFactory, IServiceScopeFactory scopeFactory, CurrencyDbContext dbContext, ILogger logger)
        {
            _httpClientFactory = httpClientFactory;
            _scopeFactory = scopeFactory;
            _dbContext = dbContext;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await UpdateCurrencyRatesAsync();
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Adjust the interval as needed
            }
        }
        protected async Task UpdateCurrencyRatesAsync()
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<CurrencyDbContext>();

                    var usdTryRate = await GetExchangeRate("USD", "TRY");
                    var eurTryRate = await GetExchangeRate("EUR", "TRY");

                    _logger.LogInformation($"UsdTryRate came as {usdTryRate}");
                    _logger.LogInformation($"EurTryRate came as {eurTryRate}");


                    var currencyOne = new ExchangeRate
                    {
                        CurrencyPair = "TRY-USD",
                        Rate = usdTryRate,
                        LastUpdated = DateTime.UtcNow,
                    };

                    var currencyTwo = new ExchangeRate
                    {
                        CurrencyPair = "TRY-EUR",
                        Rate = eurTryRate,
                        LastUpdated = DateTime.UtcNow,
                    };

                    await dbContext.ExchangeRates.AddAsync(currencyOne);
                    await dbContext.ExchangeRates.AddAsync(currencyTwo);

                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                // Log or handle exceptions as needed
            }
        }

        public async Task<decimal> GetExchangeRate(string currencyOne, string currencyTwo)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient();
                var myApiKey = "yourApiKey";
                // ....../apiKey/pair/base/target
                var endpoint = $"https://v6.exchangerate-api.com/v6/{myApiKey}/pair/{currencyOne}/{currencyTwo}";

                var response = await httpClient.GetAsync(endpoint);
                response.EnsureSuccessStatusCode(); // Ensure the request was successful

                var responseContent = await response.Content.ReadAsStringAsync();
                var apiData = JsonConvert.DeserializeObject<ExchangeRateApiResponse>(responseContent);

                return apiData.conversion_rate;
            }
            catch (Exception ex)
            {
                return 0;
            }


        }

    }
}
