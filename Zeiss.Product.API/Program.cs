using Microsoft.EntityFrameworkCore;
using Polly;
using Zeiss.Product.API.Helpers;
using Zeiss.Product.DAL.Data;
using Zeiss.Product.Services.ProductService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ProductDbContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductDbConnection")));
builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>()); // This requires the AutoMapper.Extensions.Microsoft.DependencyInjection NuGet package
//builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddResponseCaching();
builder.Services.AddHttpClient("ProductClient")
    .AddTransientHttpErrorPolicy(policy => policy.WaitAndRetryAsync(3, _ => TimeSpan.FromMilliseconds(500)));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/test", () => "Test endpoint hit");

app.UseResponseCaching();
app.UseHttpsRedirection();
app.Run();
