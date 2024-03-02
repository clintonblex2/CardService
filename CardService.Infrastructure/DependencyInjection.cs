using CardService.Application.Common.Interfaces;
using CardService.Domain.Entities;
using CardService.Domain.Enums;
using CardService.Infrastructure.Persistence.DataAccess;
using CardService.Infrastructure.Persistence.DatabaseContext;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CardService.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddTransient(typeof(IRepository<>), typeof(Repository<,>));
            services.AddTransient<IUnitOfWork, UnitofWork<CardDbContext>>();

            return services;
        }

        public static void AddDbConnection(this IServiceCollection services, IConfiguration configuration)
        {
            string connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<CardDbContext>(options =>
            {
                options.UseSqlServer(connectionString,
                    x => x.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));

            });
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = Configuration.GetValue<bool>("Jwt:ValidateSigningKey"),
                    ValidateIssuer = Configuration.GetValue<bool>("Jwt:ValidateIssuer"),
                    ValidateAudience = Configuration.GetValue<bool>("Jwt:ValidateAudience"),
                    ValidateLifetime = Configuration.GetValue<bool>("Jwt:ValidateLifeTime"),
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration.GetValue<string>("Jwt:SecretKey")))
                };
            });
            return services;
        }

        public async static Task SeedUserData(this IApplicationBuilder app)
        {
            //initializing custom users 
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {

                var context = serviceScope.ServiceProvider.GetRequiredService<CardDbContext>();
                var hasher = serviceScope.ServiceProvider.GetRequiredService<IPasswordHasher>();

                var newUsers = new List<UserEntity>
                {
                    new UserEntity {
                        Email = "member@test.com",
                        Password = hasher.HashPassword("Member@123"),
                        Role = Role.Member
                    },
                    new UserEntity {
                        Email = "member2@test.com",
                        Password = hasher.HashPassword("Member@123"),
                        Role = Role.Member
                    },
                    new UserEntity {
                        Email = "admin@test.com",
                        Password = hasher.HashPassword("Admin@123"),
                        Role = Role.Admin
                    }
                };

                // Check and add new users if they don't exist
                foreach (var newUser in newUsers)
                {
                    // Check if the user already exists
                    var userExists = await context.Users.AnyAsync(u => u.Email == newUser.Email);
                    if (!userExists)
                    {
                        // User does not exist, add them to the database
                        context.Users.Add(newUser);
                    }
                }

                // Save changes to the database
                await context.SaveChangesAsync();
            }
        }

        public static void RunMigration(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope();
            serviceScope.ServiceProvider.GetRequiredService<CardDbContext>().Database
                .Migrate();
        }
    }
}
