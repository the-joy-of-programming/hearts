// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using System;
using System.Collections.Generic;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AppIdentity
{
    public class Startup
    {
        public IHostingEnvironment Environment { get; }
        private IConfiguration configuration;

        public Startup(IHostingEnvironment environment, IConfiguration configuration)
        {
            this.configuration = configuration;
            Environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var identityConfiguration = configuration.GetSection("Identity").Get<IdentityConfig>();

            var builder = services.AddIdentityServer()
                .AddInMemoryIdentityResources(new List<IdentityResource>())
                .AddInMemoryApiResources(new List<ApiResource>())
                .AddInMemoryClients(new List<Client>());

            var authBuilder = services.AddAuthentication();

            foreach (var provider in identityConfiguration.Providers)
            {
                var cookieName = $"pace.{provider.Name}";
                authBuilder.AddCookie(cookieName);
                authBuilder.AddOpenIdConnect(provider.Name, options =>
                {
                    options.ClaimActions.MapAllExcept("iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash");
                    options.SignInScheme = cookieName;
                    options.Authority = provider.BaseUrl;
                    options.CallbackPath = "/auth/internal-callback";
                    options.ClientId = provider.ClientId;
                    options.ClientSecret = provider.ClientSecret;
                });
            }

            if (Environment.IsDevelopment())
            {
                builder.AddDeveloperSigningCredential();
            }
            else
            {
                throw new Exception("need to configure key material");
            }

            services.AddMvc();

            services.AddDbContext<IdentityContext>(options =>
            {
                options.UseInMemoryDatabase("Testing");
            });

            services.AddScoped<IdentityService>();
            services.AddScoped<AuthController>();
        }

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
            app.UseIdentityServer();
        }
    }
}