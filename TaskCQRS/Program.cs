using Infrastructue.Data;
using Microsoft.EntityFrameworkCore;
using System;
using MediatR;
using Applications;
using Infrastructue.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Infrastructue;
using Infrastructue.BackgroundJobs;
using Hangfire;
using HangfireBasicAuthenticationFilter;
namespace TaskCQRS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            //DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DEV"));
            });
            builder.Services.AddHangfire(config =>
            {
                config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DEV"));
            });

            builder.Services.AddHangfireServer();

            builder.Services.AddScoped<LowStockNotifierJob>();
          


            builder.Services.AddMediatR(typeof(Class1).Assembly);
            builder.Services.AddMediatR(typeof(Program).Assembly);
            builder.Services.AddMediatR(typeof(Class3).Assembly);
            //jwt
           
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
           
            builder.Services.AddSingleton<JwtTokenGenerator>(sp =>
               new JwtTokenGenerator(
                    secretKey: jwtSettings["SecretKey"]!,
                    issuer: jwtSettings["Issuer"]!,
                    audience: jwtSettings["Audience"]!
                    ));
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
                    };
                });

          
            builder.Services.AddControllers();
            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            var app = builder.Build();

            //backJpbs
            using (var scope = app.Services.CreateScope())
            {
                var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
                recurringJobManager.AddOrUpdate<LowStockNotifierJob>(
                    "LowStockNotificationJob",
                    job => job.Execute(CancellationToken.None),
                    Cron.Minutely);
                recurringJobManager.AddOrUpdate<TransactionJob>(
                    "archive old transactions",
                    archiver => archiver.Execute(),
                    Cron.Minutely);
            }
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHangfireDashboard("/Hangfire", new DashboardOptions
            {
                Authorization =
              [
                  new HangfireCustomBasicAuthenticationFilter
                    {
                        User = app.Configuration.GetValue<string>("HangireSetting:Username"),
                        Pass = app.Configuration.GetValue<string>("HangireSetting:Password")
                    }
              ],
                DashboardTitle = "Inventory Background jobs Dash"
            });

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
