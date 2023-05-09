using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using VkInternship.App.Filters;
using VkInternship.Data;

namespace VkInternship.App;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthorization();
        services.AddControllers(options =>
        {
            options.Filters.Add<DatabaseExceptionFilter>();
        });
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Vk Internship Task Users' API",
                Version = "v1",
                Description = "ASP.NET Core Web API for Vk Internship Task"
            });
            
            c.CustomOperationIds(apiDescription =>
            {
                return apiDescription.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null;
            });

        });

        var connectionString = Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<UsersContext>(options => options.UseNpgsql(connectionString));

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UsersContext usersContext)
    {
        usersContext.Database.Migrate();

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vk Internship Task v1");
                c.DisplayOperationId();
            });
        }

        app.UseRouting();
        app.UseAuthentication();
        
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}