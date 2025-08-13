using Microsoft.EntityFrameworkCore;
using ProductApi.Models;
using ProductApi.Repositories.Interfaces;
using ProductApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseInMemoryDatabase("ProductDb"));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ProductService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    if (!dbContext.Products.Any()) 
    {
        dbContext.Products.AddRange(new Product
        {
            Name = "Computador",
            Price = 1000.99m,
            Description = "Computador"
        }, new Product
        {
            Name = "Celular",
            Price = 699.99m,
            Description = "Celular"
        }, new Product
        {
            Name = "Monitor",
            Price = 299.99m,
            Description = "Monitor"
        });

        dbContext.SaveChanges(); 
    }
}

app.Use((context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/swagger");
        return Task.CompletedTask;
    }
    return next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
