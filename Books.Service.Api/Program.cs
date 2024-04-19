using System.Text;
using Books.Service.Internal.Api.Infrastructure;
using Books.Service.Internal.Api.Infrastructure.Services;
using Books.Service.Internal.Api.Interceptors;
using customerapi.Model;
using Google.Apis.Books.v1.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddCors();
        builder.Services.AddDbContext<ReadersDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

        var _jwtsetting = builder.Configuration.GetSection("JWTSetting");
        builder.Services.Configure<JWTSetting>(_jwtsetting);

        var _apiSetting = builder.Configuration.GetSection("APISetting");
        builder.Services.Configure<APISetting>(_apiSetting);

        var authkey = builder.Configuration.GetValue<string>("JWTSetting:securitykey");
        var apikey = builder.Configuration.GetValue<string>("APISetting:apikey");

        var logger = new LoggerConfiguration()
                .ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(logger);

        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        builder.Services.AddMemoryCache();
        builder.Services.AddAuthentication(options => { options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; })
            .AddJwtBearer(item =>
            {
                item.RequireHttpsMetadata = true;
                item.SaveToken = true;
                item.TokenValidationParameters = new()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authkey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        builder.Services.AddHttpClient<IBookDataService, BookDataService>((serviceProvider, client) =>
        {
            var settings = serviceProvider
                .GetRequiredService<IOptions<APISetting>>().Value;

            client.BaseAddress = new Uri($"{settings.bookClientUrl}");
        });

        builder.Services.AddScoped<IRefreshTokenGenerator, RefreshTokenGenerator>();
        builder.Services.AddScoped<ICacheService, CacheService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //Replace Url of frontend application for specific access
        //app.UseCors(p => p.WithOrigins("https://<websitename>").AllowAnyHeader().AllowAnyMethod());
        app.UseCors(builder =>
        builder.AllowAnyOrigin().WithHeaders("X-Session-ID", "X-User-ID").AllowAnyMethod()
            );

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseMiddleware(typeof(ExceptionHandlingMiddleware));
        app.UseMiddleware(typeof(LoggingMiddleware));

        app.MapControllers();

        app.Run();
    }
}