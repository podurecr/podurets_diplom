using Domain.Services.Interfaces;
using Domain.Services.Services;

using Microsoft.EntityFrameworkCore;

using Repositories.Data;
using Repositories.Repositories.Interfaces;
using Repositories.Repositories.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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

app.UseAuthorization();

app.MapControllers();

app.Run();