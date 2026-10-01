using System.Text;
using JwtAuthenticationDemo.Api.Data;
using Microsoft.EntityFrameworkCore;
using JwtAuthenticationDemo.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.SwaggerGen;
using Asp.Versioning;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddControllers();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme) //use Jwt bearer authentication as default
    .AddJwtBearer(options=> //When a request contains a Bearer token, validate it as a JWT.
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    builder.Configuration["Jwt:Key"]!
                )
            )
        };
                options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"OnChallenge error: {context.Error}, {context.ErrorDescription}");
                return Task.CompletedTask;
            }
        };
    }
    );
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
// Add Swagger services
// builder.Services.AddSwaggerGen(options =>
// {
//     options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
//     {
//         Title = "JWT Authentication Demo API",
//         Version = "v1",
//         Description = "API for JWT authentication demonstration"
//     });
// });
builder.Services.AddApiVersioning(Options =>
{
    Options.DefaultApiVersion = new ApiVersion(1, 0);
    Options.AssumeDefaultVersionWhenUnspecified = true;
    Options.ReportApiVersions = true;
    Options.ApiVersionReader = new UrlSegmentApiVersionReader();
}
).AddMvc();
builder.Services.AddAuthorization(Options =>
{
    Options.AddPolicy("V1Access", policy =>
    {
        policy.RequireRole("Admin");
    });
    Options.AddPolicy("V2Access", policy =>
    {
        policy.RequireRole("Admin", "User");
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers(); 

app.Run();
