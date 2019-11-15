using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Victor.CUI.RHDM.KIE.Client;
using Victor.CUI.RHDM.KIE.Api;

namespace Victor.Server.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHealthChecks();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddTransient((provider) => new EDDIClient("http://victor-webapi-evals25-shared-7daa.apps.hackathon.rhmi.io/", Api.HttpClient));

            services.AddTransient((provider) =>
            {
                var config = new Configuration();
                config.BasePath = "https://victor-kieserver-evals25-shared-7daa.apps.hackathon.rhmi.io/services/rest/";
                config.Username = Api.Config("KIE_ADMIN_USER");
                config.Password = Api.Config("KIE_ADMIN_PWD");
                config.ApiClient.RestClient.Authenticator = new RestSharp.Authenticators.HttpBasicAuthenticator(config.Username, config.Password);
                return new KIESessionAssetsApi(config);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks("/health");

            // Optionally, initialize Db with data here

            app.UseMvc();
        }
    }
}
