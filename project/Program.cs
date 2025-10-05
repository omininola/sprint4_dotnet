using Microsoft.EntityFrameworkCore;
using project.Data;
using project.DTO.Bike;
using project.DTO.Subsidiary;
using project.DTO.Yard;
using project.Exceptions;
using project.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddScoped<IService<SubsidiaryResponse, SubsidiaryDTO>, SubsidiaryService>();
builder.Services.AddScoped<IService<YardResponse, YardDTO>, YardService>();
builder.Services.AddScoped<IService<BikeResponse, BikeDTO>, BikeService>();
builder.Services.AddControllers(opt =>
{
    opt.Filters.Add<ExceptionFilter>();
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});
builder.Services.AddDbContext<AppDbContext>(options => options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
