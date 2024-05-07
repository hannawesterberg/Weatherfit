using Microsoft.EntityFrameworkCore;
using WeatherFit.Data;
using WeatherFit;
using WeatherFit.Entities;
using System.Text.Json.Serialization;
using WeatherFit.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//1. Database context setup
builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
//Adds ApplicationDbContext to the Dependency Injection contatiner and configures it to use PostgreSQL
//with connection string specified in appsettings.json


// 2. Add HttpClient for WeatherService
builder.Services.AddHttpClient<WeatherService>(); // This line adds the HttpClient service for WeatherService


//3. Add BusinessLogic sevice 
builder.Services.AddScoped<BusinessLogic>(); // Adds BusinessLogic to the DI container

// Adds MVC controllers to the DI container, enables app to handle API request by finding appropriate controllers
// Configure JsonSerializerOptions to handle object cycles
builder.Services.AddLogging();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDevOrigin", policyBuilder =>
    {
        policyBuilder.WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

//Swagger generation 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add JWT Authentication
/*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

*/


//logging configuration
builder.Logging.ClearProviders(); // new
builder.Logging.AddConsole(); //new 
builder.Logging.AddDebug(); // new 


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngularDevOrigin");

app.UseAuthorization();
app.UseHttpsRedirection();
app.UseAuthentication();  // Make sure to call UseAuthentication before UseAuthorization
app.UseAuthorization();

app.MapControllers();

app.Run();


