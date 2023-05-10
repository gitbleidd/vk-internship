using System.Reflection;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using VkInternship.App.Filters;
using VkInternship.App.Authentication;
using VkInternship.App.Services;
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
        services.AddAuthentication("BasicAuthentication")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
        
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
            
            c.CustomOperationIds(apiDescription => apiDescription.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null);

            c.AddSecurityDefinition("basic", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "basic",
                In = ParameterLocation.Header,
                Description = "Basic Authorization header using the Bearer scheme."
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "basic"
                        }
                    },
                    new string[] { }
                }
            });

        });

        var connectionString = Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<UsersContext>(options => options.UseNpgsql(connectionString));
        
        services.AddScoped<UserService>();
        services.AddScoped<AuthService>();

        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        services.AddMemoryCache();
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
                c.RoutePrefix = string.Empty;
            });
        }

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}