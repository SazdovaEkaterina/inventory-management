using System.Text;
using System.Text.Json.Serialization;
using InventoryManagement.API.DbContexts;
using InventoryManagement.API.Models;
using InventoryManagement.API.Models.Entities;
using InventoryManagement.API.Services;
using InventoryManagement.API.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// var allowReactAppOrigin = "allowReactAppOrigin";
//
// builder.Services.AddCors(options =>
// {
//     options.AddPolicy(name: allowReactAppOrigin,
//         policy =>
//         {
//             policy.WithOrigins("http://localhost:3000");
//         });
// });

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(
        options => options.JsonSerializerOptions
            .ReferenceHandler=ReferenceHandler.IgnoreCycles)
    .AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<InventoryContext>(
    optionsBuilder => optionsBuilder.UseNpgsql(
        @"User ID = postgres;
        Password=password;
        Server=localhost;
        Port=54321;
        Database=InventoryContext;
        Integrated Security=true;
        Pooling=true"
    )
);

builder.Services.AddIdentity<User, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<InventoryContext>()
    .AddDefaultTokenProviders(); 

//builder.Services.AddIdentityCore<User>();

builder.Services.AddScoped<IInventoryManagementRepository, InventoryManagementRepository>();

builder.Services.AddScoped(typeof(UserManager<>));

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Authentication:Issuer"],
                ValidAudience = builder.Configuration["Authentication:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey
                (
                    Encoding.ASCII.GetBytes
                    (
                        builder.Configuration["Authentication:SecretForKey"]
                    )
                )
            };
        }
    );

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseCors(allowReactAppOrigin);
app.UseCors(x => x
    .AllowAnyMethod()
    .AllowAnyHeader()
    .WithOrigins("http://localhost:3000"));

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();