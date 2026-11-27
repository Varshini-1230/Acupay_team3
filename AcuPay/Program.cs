
using AcuPay.Data;
using AcuPay.Repositories;
using AcuPay.Services;

var builder = WebApplication.CreateBuilder(args);

// Register controllers
builder.Services.AddControllers();

// Register Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register database connection factory
builder.Services.AddScoped<DbConnectionFactory>();

// Register repositories
builder.Services.AddScoped<IInvoiceRepository, InvoiceRepository>();
builder.Services.AddScoped<IPurchaseOrderRepository, PurchaseOrderRepository>();

// Register Invoice Matching Service
builder.Services.AddScoped<InvoiceMatchingService>(provider =>
{
    var invoiceRepository =
        provider.GetRequiredService<IInvoiceRepository>();

    var purchaseOrderRepository =
        provider.GetRequiredService<IPurchaseOrderRepository>();

    var configuration =
        provider.GetRequiredService<IConfiguration>();

    decimal tolerance = configuration.GetValue<decimal>(
        "InvoiceMatching:TolerancePercentage");

    return new InvoiceMatchingService(
        tolerance,
        invoiceRepository,
        purchaseOrderRepository);
});

var app = builder.Build();

// Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
