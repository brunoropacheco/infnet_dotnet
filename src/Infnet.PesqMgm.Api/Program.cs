using Infnet.PesqMgm.Domain.Repositories;
using Infnet.PesqMgm.Infrastructure.Data.Repositories;
using Infnet.PesqMgm.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuração do Banco de Dados In-Memory
builder.Services.AddDbContext<PesquisaDbContext>(options =>
    options.UseInMemoryDatabase("PesqMgmDb"));

// Injeção de Dependência dos Repositórios
builder.Services.AddScoped<IUsuarioRepository, InMemoryUsuarioRepository>();
builder.Services.AddScoped<IPesquisaRepository, InMemoryPesquisaRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Em ambientes de desenvolvimento (especialmente containers), o redirecionamento pode falhar se não houver certificado.
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();