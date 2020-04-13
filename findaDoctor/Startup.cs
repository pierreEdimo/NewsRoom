using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using findaDoctor.DBcontext;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using NSwag.AspNetCore;
using Microsoft.AspNetCore.Mvc.Versioning;
using findaDoctor._Filters;

namespace findaDoctor
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connection = Configuration.GetConnectionString("FinDaDoctorDatabase"); 
            services.AddDbContext<DatabaseContext>(opt => opt.UseSqlServer(connection));
            services.AddControllers();

            services.AddSwaggerDocument();


            services.AddCors(options =>
            {
                options.AddPolicy("AllowAny",
                    policy => policy.AllowAnyOrigin()
                );
            });
           

            services.AddMvc(options =>
            {
                options.EnableEndpointRouting = false ;
                options.Filters.Add<ExceptionFilter>();
                options.Filters.Add<RequireHttpsOrClose>();
            }).SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.AddApiVersioning(
                options =>
                {
                    options.DefaultApiVersion = new ApiVersion(1, 0);
                    options.ApiVersionReader = new MediaTypeApiVersionReader();
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.ReportApiVersions = true;
                    options.ApiVersionSelector = new CurrentImplementationApiVersionSelector(options); 
                }
                );

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseOpenApi();

                app.UseSwaggerUi3(); 

            }

            app.UseCors("AllowAny");

            app.UseMvc();
       
            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
