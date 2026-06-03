using CareSync.Data;
using CareSync.Models;
using CareSync.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IMedicineService, MedicineService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IAdminService, AdminService>();

// Add Controllers
builder.Services.AddControllers();

// DbContext
builder.Services.AddDbContext<EmedicineContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//seed the admin role//

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<EmedicineContext>();
    if (!context.Users.Any(u => u.Type == "Admin"))
    {
        context.Users.Add(new User
        {
            FirstName = "System",
            LastName = "Admin",
            Email = "admin@caresync.com",
            Password = BCrypt.Net.BCrypt.HashPassword("Admin123"),
            Type = "Admin",
            Status = 1,
            CreatedOn = DateTime.Now
        });

        context.SaveChanges();
    }
}

// Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
// Map Controllers
app.MapControllers();

app.Run();