using Inventory_Mgmt_System.Data;
using Inventory_Mgmt_System.Repositories;
using Inventory_Mgmt_System.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register AppDbContext with the dependency injection container.  Configure it to use PostgreSQL as the database provider,  with the connection string read from appsettings.json ("DefaultConnection").
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
//services.AddDbContext<MyDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection")));


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
