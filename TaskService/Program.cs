using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using EventService.Controllers;
using EventService.Data;
using EventService.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EventServiceContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("EventServiceContext") ?? throw new InvalidOperationException("Connection string 'EventServiceContext' not found.")));


builder.Services.AddControllers();
builder.Services.AddSingleton<EventDb>();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
