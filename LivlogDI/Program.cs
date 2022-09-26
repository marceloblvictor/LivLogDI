using System.Text;
using LivlogDI.Constants;
using LivlogDI.Data;
using LivlogDI.Data.Repositories;
using LivlogDI.Data.Repositories.Interfaces;
using LivlogDI.Services;
using LivlogDI.Services.Interfaces;
using LivlogDI.Validators;
using LivlogDI.Validators.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<LivlogDIContext>(
    options => options
        .UseSqlServer(builder.Configuration.GetConnectionString("LivlogDIContext") 
            ?? throw new InvalidOperationException("Connection string 'LivlogDIContext' not found.")));

// Add dependencies to the container.
builder.Services.AddScoped(typeof(ILivlogDIContext), typeof(LivlogDIContext));

builder.Services.AddScoped(typeof(IBookService), typeof(BookService));
builder.Services.AddScoped(typeof(IBookRepository), typeof(BookRepository));

builder.Services.AddScoped(typeof(IAuthService), typeof(AuthService));
builder.Services.AddScoped(typeof(IUserRepository), typeof(UserRepository));

builder.Services.AddScoped(typeof(ICustomerService), typeof(CustomerService));
builder.Services.AddScoped(typeof(ICustomerRepository), typeof(CustomerRepository));

builder.Services.AddScoped(typeof(ICustomerBookService), typeof(CustomerBookService));
builder.Services.AddScoped(typeof(ICustomerBookRepository), typeof(CustomerBookRepository));

builder.Services.AddScoped(typeof(IFineService), typeof(FineService));
builder.Services.AddScoped(typeof(IFineRepository), typeof(FineRepository));

builder.Services.AddScoped(typeof(IMessagerService), typeof(MessagerService));

builder.Services.AddScoped(typeof(IAuthValidator), typeof(AuthValidator));
builder.Services.AddScoped(typeof(IBookRentalValidator), typeof(BookRentalValidator));
builder.Services.AddScoped(typeof(IFineValidator), typeof(FineValidator));

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(s =>
{
    var xmlFile = $"APIDocumentation.XML";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    s.IncludeXmlComments(xmlPath);

    OpenApiSecurityScheme openApiSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "JWT Authentication",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Insira aqui seu JWT Token",
        Reference = new OpenApiReference
        {
            Id = "Bearer",
            Type = ReferenceType.SecurityScheme
        }
    };
    s.AddSecurityDefinition(openApiSecurityScheme.Reference.Id, openApiSecurityScheme);
    s.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            openApiSecurityScheme,
            Array.Empty<string>()
        }
    });
});

builder.Services
    .AddAuthentication(options =>
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
            ValidIssuer = JWT.JWT_ISSUER,
            ValidAudience = JWT.JWT_AUDIENCE,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JWT.JWT_KEY))
        };
    });

builder.Services.Configure<RouteOptions>(
    options => options.LowercaseUrls = true);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
