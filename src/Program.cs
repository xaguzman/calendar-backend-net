using Microsoft.AspNetCore.Http.Json;
using CalendarBackend.Routes;
using FluentValidation;
using CalendarBackend.Models;
using CalendarBackend.Services;
using CalendarBackend;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MongoDB.Bson.Serialization.Conventions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<CalendarDBSettings>(builder.Configuration.GetSection("CalendarDBSettings"));
builder.Services.Configure<JsonOptions>(opt =>
{
    opt.SerializerOptions.IncludeFields = true;
});

builder.Services.AddTransient<JwtManager>();
builder.Services.AddScoped<IValidator<UserModel>, NewUserValidator>();
builder.Services.AddScoped<IValidator<EventModel>, EventValidator>();
builder.Services.AddSingleton<UsersService>();
builder.Services.AddSingleton<EventsService>();
builder.Services.AddAuthentication().AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        // Validate the Issuer
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SignKey"]!))
    };
});

builder.Services.AddAuthorizationBuilder().AddPolicy("user_logged", policy => policy.RequireClaim("uid"));

// builder.Services.AddCors();
builder.Services.AddCors(o =>
{
    o.AddPolicy("EnableCORS", builder => builder
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod()
            // .AllowCredentials()
            // .WithOrigins("http://localhost:5000", "http://localhost:3000")
        );
});

var app = builder.Build();

var convention = new ConventionPack{ new CamelCaseElementNameConvention() };
ConventionRegistry.Register("camelCase", convention, t => true);

app.MapGet("/hearthbeat", (UsersService serv) => {
    var ignoredResult = serv.GetAsync( x => x.Email == "someEmail@gmail.com" );
    
    return Results.Ok();
});
app.MapApi();

app.UseCors("EnableCORS");
app.UseAuthentication();
app.UseAuthorization();

app.Run();
