
using AcuPay.Data;
using AcuPay.Repositories;
using AcuPay.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

// Register database connection factory
builder.Services.AddScoped<DbConnectionFactory>();

// Register repositories
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();

// Register invoice matching service
builder.Services.AddScoped<InvoiceMatchingService>(provider =>
{
    return new InvoiceMatchingService(
        5.0m,
        provider.GetRequiredService<IInvoiceRepository>(),
        provider.GetRequiredService<IPurchaseOrderRepository>()
    );
});

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
