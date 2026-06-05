using Domain.Services.Interfaces;
using Domain.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Repositories.Data;
using Repositories.Repositories.Interfaces;
using Repositories.Repositories.Repositories;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),

            RoleClaimType = ClaimTypes.Role
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// DI: Services
builder.Services.AddScoped<IAnalysisResultService, AnalysisResultService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IBatchService, BatchService>();
builder.Services.AddScoped<IProductQualitySpecificationService, ProductQualitySpecificationService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IQualityAssessmentService, QualityAssessmentService>();
builder.Services.AddScoped<IQualityCertificateService, QualityCertificateService>();
builder.Services.AddScoped<IShipmentDecisionService, ShipmentDecisionService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IQualityParameterService, QualityParameterService>();

// DI: Repositories
builder.Services.AddScoped<IAnalysisResultRepository, AnalysisResultRepository>();
builder.Services.AddScoped<IBatchRepository, BatchRepository>();
builder.Services.AddScoped<IProductQualitySpecificationRepository, ProductQualitySpecificationRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IQualityAssessmentRepository, QualityAssessmentRepository>();
builder.Services.AddScoped<IQualityCertificateRepository, QualityCertificateRepository>();
builder.Services.AddScoped<IQualityParameterRepository, QualityParameterRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IShipmentDecisionRepository, ShipmentDecisionRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<QualityCertificateRepository>();
builder.Services.AddScoped<BatchRepository>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<QualityAssessmentRepository>();
builder.Services.AddScoped<AnalysisResultRepository>();
builder.Services.AddScoped<ProductQualitySpecificationRepository>();

// Если у тебя есть общий IRepository / Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Product Control API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();