using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hearts;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HeartsApp
{

    public class Startup
    {
        private readonly IConfiguration configuration;
        private RandomFactory randomFactory;

        public Startup(IHostingEnvironment env, IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = configuration["Database:ConnectionString"];
            if (connectionString == null)
            {
                throw new Exception("The property Database:ConnectionString must be specified");
            }
            services.AddDbContext<HeartsContext>(options =>
            {
                options.UseNpgsql(connectionString);
            });

            var randomSeed = configuration["App:RandomSeed"];
            int? randomSeedInt = null;
            if (randomSeed != null)
            {
                randomSeedInt = int.Parse(randomSeed);
            }
            randomFactory = new RandomFactory(randomSeedInt);
            services.AddScoped<Random>((_) => randomFactory.createRandom());
            services.AddScoped<Shuffler>();
            services.AddScoped<AppStatisticsRepository>();

            services.AddMvc();

            services.AddScoped<GetShuffledDeckUseCase>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
