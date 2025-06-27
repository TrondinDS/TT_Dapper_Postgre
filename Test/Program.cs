using Microsoft.OpenApi.Models;
using Test.DB.Mappings;
using Test.Repositories;
using Test.Repositories.Interfaces;
using Test.Services;
using Test.Services.Interfaces;
using Test1.DB.DbConnectionFactory;
using Test1.DB.Repositories;

namespace Test
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Test1 API",
                    Version = "v1"
                });
            });

            builder.Services.AddSingleton<IDbConnectionFactory, NpgsqlConnectionFactory>();

            builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
            builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            builder.Services.AddScoped<ICompanyService, CompanyService>();
            builder.Services.AddScoped<IDepartmentService, DepartmentService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();

            builder.Services.AddAutoMapper(typeof(MapModel));

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Test1 API V1");
                });

                app.MapGet("/", context =>
                {
                    context.Response.Redirect("/swagger");
                    return Task.CompletedTask;
                });
            }

            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
