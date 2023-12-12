using CurrencyExchangeApi.Data;
using CurrencyExchangeApi.Jobs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);




builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<CurrencyDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysqlConnectionString"), 
        new MySqlServerVersion(new Version(8, 0, 21))) 
        );
builder.Services.AddHostedService(provider =>
{
    var scopedProvider = provider.CreateScope().ServiceProvider;
    return scopedProvider.GetRequiredService<CurrencyUpdateService>();
});
builder.Services.AddScoped<CurrencyUpdateService>();  // Register as scoped

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
