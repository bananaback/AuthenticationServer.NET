using AuthenticationServer.API.Models;
using AuthenticationServer.API.Serivces.Authenticators;
using AuthenticationServer.API.Serivces.PasswordHasher;
using AuthenticationServer.API.Serivces.RefreshTokenRepository;
using AuthenticationServer.API.Serivces.TokenGenerators;
using AuthenticationServer.API.Serivces.TokenValidators;
using AuthenticationServer.API.Serivces.UserRepositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthenticationServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Configuration.AddJsonFile("appsettings.json");
            var authenticationConfiguration = new AuthenticationConfiguration();
            builder.Configuration.GetSection("Authentication").Bind(authenticationConfiguration);

            builder.Services.AddSingleton(authenticationConfiguration);

            // Add services to the container.

            builder.Services.AddControllers();

            var connection = String.Empty;

            if (builder.Environment.IsDevelopment())
            {
                connection = builder.Configuration.GetConnectionString("AZURE_SQL_CONNECTIONSTRING");
            }
            else
            {
                connection = Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTIONSTRING");
            }

            //builder.Services.AddDbContext<AuthenticationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("sqlserver")));
            builder.Services.AddDbContext<AuthenticationDbContext>(options => options.UseSqlServer(connection));

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
            {
                o.TokenValidationParameters = new TokenValidationParameters()
                {
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationConfiguration.AccessTokenSecret)),
                    ValidIssuer = authenticationConfiguration.Issuer,
                    ValidAudience = authenticationConfiguration.Audience,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ClockSkew = TimeSpan.Zero
                };
            });

            builder.Services.AddSingleton<AccessTokenGenerator>();
            builder.Services.AddSingleton<RefreshTokenGenerator>();
            builder.Services.AddSingleton<RefreshTokenValidator>();
            builder.Services.AddSingleton<TokenGenerator>();
            builder.Services.AddScoped<Authenticator>();
            builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
            builder.Services.AddScoped<IUserRepository, DatabaseUserRepository>();
            builder.Services.AddScoped<IRefreshTokenRepository, DatabaseRefreshTokenRepository>();

            var app = builder.Build();

            // Execute DB Migrations Automatically on Startup with .NET Entity Framework
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AuthenticationDbContext>();
                dbContext.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run();
        }
    }
}