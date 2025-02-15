using ActivationKeyService.Mutation;
using ActivationKeyService.Query;
using Mutation = ActivationKeyService.Mutation;
using Microsoft.EntityFrameworkCore;
using System;
using HotChocolate;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Query = ActivationKeyService.Query.Query;

var builder = WebApplication.CreateBuilder(args);


// Konfiguracja bazy danych SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=activationKeys.db"));

// Konfiguracja GraphQL
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<ActivationKeyService.Mutation.Mutation>();

var app = builder.Build();

app.MapGraphQL();

app.Run();
