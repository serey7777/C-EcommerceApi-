using Mapster;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Collections.Generic;
using System.IO;
using System.Text;
using WebApplicationProductAPI.DataConn;
using WebApplicationProductAPI.Models.Domain;
using WebApplicationProductAPI.Models.DTO.ProductDTO;
using WebApplicationProductAPI.Repositories.CategoryRepo;
using WebApplicationProductAPI.Repositories.ImageRepo;
using WebApplicationProductAPI.Repositories.ProductRepo;
using WebApplicationProductAPI.Repositories.SupplierRepo;
using WebApplicationProductAPI.Repositories.TokenRepo;
using System.Linq;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Logging
// -----------------------------
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .MinimumLevel.Information()
    .CreateLogger();

builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

// -----------------------------
// Add services to the container
// -----------------------------

builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();
builder.Services.AddEndpointsApiExplorer();

// CORS Policy to allow React frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()       // allows Content-Type: multipart/form-data
              .AllowAnyMethod()       // allows POST, OPTIONS etc.
              .AllowCredentials();   // if credentials needed
    });
});


// Swagger configuration
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "WebAPI",
        Version = "v1"
    });

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            new List<string>()
        }
    });
});

// -----------------------------
// Database connections
// -----------------------------

// Product database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// Auth database
//builder.Services.AddDbContext<ApplicationAuthDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultAuthConnectionString")));

// -----------------------------
// Dependency Injection
// -----------------------------
builder.Services.AddScoped<ISupplierRepository, SQLSupplierRepository>();
builder.Services.AddScoped<IProductRepository, SQLProductRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IImageRepository, SQLImageRepository>();
builder.Services.AddScoped<ICategoryRepository, SQLCategoryRepository>();

// -----------------------------
// Identity Configuration
// -----------------------------
//builder.Services.AddIdentity<IdentityUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationAuthDbContext>() 
//    .AddDefaultTokenProviders();

// Optional: Relax password rules during development
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
});

// -----------------------------
// JWT Authentication
// -----------------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
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
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization();

// -----------------------------
// Build the app
// -----------------------------
var app = builder.Build();

// -----------------------------
// Configure the HTTP pipeline
// -----------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}
app.UseHttpsRedirection();

app.UseDeveloperExceptionPage(); // show errors in dev

// Enable serving static files from wwwroot
var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images");
if (!Directory.Exists(imagesPath))
{
    Directory.CreateDirectory(imagesPath);
}
app.UseStaticFiles(); // this will serve /Images automatically


TypeAdapterConfig<ProductDomain, AllProductDto>.NewConfig()
    .Map(dest => dest.ImagePath, src => src.Images != null ? src.Images.Select(img => img.FilePath).ToList() : new List<string>())
    .Map(dest => dest.Category, src => src.Category != null ? src.Category.Name : null)
    .Map(dest => dest.Supplier, src => src.Supplier != null ? src.Supplier.Name : null);



// Routing + CORS
app.UseRouting();
app.UseCors("AllowReactApp"); // use one policy

// Auth middleware
app.UseAuthentication();
app.UseAuthorization();
// Controllers
app.MapControllers();

app.Run();
