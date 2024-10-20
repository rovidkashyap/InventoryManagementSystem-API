using IMS.Application.Configuration.MapperConfig;
using IMS.DataAccess.DataContext;
using IMS.DataAccess.UoW;
using IMS.WebApi.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<IMSContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("IMSConnectionString"));
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

// Register UniOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register application Services
builder.Services.AddDataAccessServices();

// Regsiter AutoMapper
builder.Services.AddAutoMapper(typeof(AutoMapperConfig));

builder.Services.AddControllers();

builder.Services.AddMemoryCache();  // Ensure IMemoryCache is registered
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAllOrigins"); // Enable CORS

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
