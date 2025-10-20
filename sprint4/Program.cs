using System.Text;
using Asp.Versioning;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using sprint4.Data;
using sprint4.DTO.Bike;
using sprint4.DTO.Subsidiary;
using sprint4.DTO.Yard;
using sprint4.Exceptions;
using sprint4.Services;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddDbContext<AppDbContext>(options => options.UseOracle(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IService<SubsidiaryResponse, SubsidiaryDTO>, SubsidiaryService>();
        builder.Services.AddScoped<IService<YardResponse, YardDTO>, YardService>();
        builder.Services.AddScoped<IService<BikeResponse, BikeDTO>, BikeService>();
        builder.Services.AddControllers(opt =>
        {
            opt.Filters.Add<ExceptionFilter>();
        });

        builder.Services.AddHealthChecks();

        builder.Services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.AssumeDefaultVersionWhenUnspecified = true;
            options.ReportApiVersions = true;
        }).
        AddApiExplorer(options =>
        {
            options.GroupNameFormat = "v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sprint04 .NET API", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });


        var key = Encoding.ASCII.GetBytes(builder.Configuration.GetSection("JWT").GetSection("Key").Value);

        builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapOpenApi();

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.MapHealthChecks("/health");
        app.Run();
    }
}
