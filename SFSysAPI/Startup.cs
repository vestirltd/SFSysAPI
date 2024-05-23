using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFSysAPI.Interfaces;
using SFSysAPI.Services;
//using FunTranslation.Services;
//using FunTranslation.Services.Interface;

namespace SFSysAPI
{
     public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //Registering the poke services and ApiNetServices
            //services.AddTransient<IFunTranslationServices, FunTranslationServices>();
            services.AddTransient<IAccountsService, AccountsService>();
            //services.AddTransient<IEncryptionService, EncryptionService>();
            
            //services.AddControllers();
            
            services.AddControllers()
            .AddNewtonsoftJson(options =>
            {
                // Configure JSON serialization options to ignore null values
                options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            });

            services.AddSwaggerGen();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //app.UseSwagger();
            //app.UseSwaggerUI();
        }
    }
}