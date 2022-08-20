using LivlogDI.Data;
using LivlogDI.Data.Repositories;
using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Services;
using LivlogDI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LivlogDIContext>(
    options => options
        .UseSqlServer(builder.Configuration.GetConnectionString("LivlogDIContext") 
            ?? throw new InvalidOperationException("Connection string 'LivlogDIContext' not found.")));

// Add services to the container.
builder.Services.AddScoped(typeof(IBookService), typeof(BookService));
builder.Services.AddScoped(typeof(IBookRepository), typeof(BookRepository));

builder.Services.AddControllers();

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
