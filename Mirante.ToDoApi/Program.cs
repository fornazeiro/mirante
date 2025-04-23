using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using Mirante.Data;
using Mirante.Data.Repositories.Entities;
using Mirante.Data.Repositories.Interfaces;
using Mirante.Services.Entities;
using Mirante.Services.Interfaces;
using Pomelo.EntityFrameworkCore.MySql;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Configuração do Banco de Dados)

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TodoApi", Version = "v1" });
});

// Injeção de Dependência
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITarefaService, TarefaSevice>();
//.Services.AddScoped<IRepository<Tarefa>, Repository<Tarefa>>();

//builder.Services.AddDbContext<ToDoDbContext>(opt => opt.UseInMemoryDatabase("TodoList"));
var _connectionString= builder.Configuration
                     .GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ToDoDbContext>(options =>
                  options.UseMySql(_connectionString,
                    ServerVersion.AutoDetect(_connectionString)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ToDoApi v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
