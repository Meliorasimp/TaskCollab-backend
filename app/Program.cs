using app.Data;
using app.GraphQL.Mutations;
using app.GraphQL.Queries;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options =>
 options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Add GraphQL services
builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<UserMutation>();

builder.Services.AddCors(options => {
    options.AddPolicy("AllowFrontendOrigin", policy => {
        policy.WithOrigins("http://localhost:5173", "https://localhost:7195")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();    
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCors("AllowFrontendOrigin");
app.UseAuthentication();
app.UseAuthorization();

app.MapGraphQL("/graphql");
app.MapControllers();
app.Run();