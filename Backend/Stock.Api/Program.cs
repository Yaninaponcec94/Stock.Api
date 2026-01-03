using System.Text;
using System.Text.Json;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Stock.Api.Exceptions;
using Stock.Api.Seed;
using Stock.Application.Interfaces;
using Stock.Application.Services;
using Stock.Infrastructure.Data;
using Stock.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Db
builder.Services.AddDbContext<StockDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// DI
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IStockService, StockService>();
builder.Services.AddScoped<IStockRepository, StockRepository>();

// Auth
builder.Services
  .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
	  var jwt = builder.Configuration.GetSection("Jwt");
	  options.TokenValidationParameters = new TokenValidationParameters
	  {
		  ValidateIssuer = true,
		  ValidateAudience = true,
		  ValidateLifetime = true,
		  ValidateIssuerSigningKey = true,
		  ValidIssuer = jwt["Issuer"],
		  ValidAudience = jwt["Audience"],
		  IssuerSigningKey = new SymmetricSecurityKey(
		  Encoding.UTF8.GetBytes(jwt["Key"]!)
		)
	  };
  });

// Controllers + JSON
builder.Services
  .AddControllers()
  .AddJsonOptions(options =>
  {
	  options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
  });

// FluentValidation (1 sola vez)
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Stock API", Version = "v1" });

	c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
	{
		Name = "Authorization",
		Type = SecuritySchemeType.Http,
		Scheme = "bearer",
		BearerFormat = "JWT",
		In = ParameterLocation.Header,
		Description = "Pegá: Bearer {tu_token}"
	});

	c.AddSecurityRequirement(new OpenApiSecurityRequirement
  {
	{
	  new OpenApiSecurityScheme
	  {
		Reference = new OpenApiReference
		{
		  Type = ReferenceType.SecurityScheme,
		  Id = "Bearer"
		}
	  },
	  Array.Empty<string>()
	}
  });
});

// Error handling global (1 solo)
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// CORS
var corsPolicyName = "FrontCors";
builder.Services.AddCors(options =>
{
	options.AddPolicy(corsPolicyName, policy =>
	{
		policy
		  .WithOrigins("http://localhost:4200")
		  .AllowAnyHeader()
		  .AllowAnyMethod();
	});
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<StockDbContext>();
	await DbSeeder.SeedAsync(db);
}

app.UseCors(corsPolicyName);

// Exception handler (sin middleware extra)
app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.Run();
