using System.Collections.Generic;
using EMark.Api.Models.Enums;
using EMark.Application.Mapping;
using EMark.Application.Options;
using EMark.Application.Services;
using EMark.Application.Services.Contracts;
using EMark.DataAccess.Connection;
using Kirpichyov.FriendlyJwt.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace EMark.Api
{
    public class Startup
    {
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;
        
        public Startup(IConfiguration configuration, IHostEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<DatabaseContext>(options =>
            {
                options.UseNpgsql(_configuration.GetConnectionString("PostgreSqlConnection"));

                if (_environment.IsDevelopment())
                {
                    options.UseLoggerFactory(LoggerFactory.Create(builder => builder.AddConsole()));
                    options.EnableSensitiveDataLogging();
                }
            });

            services.Configure<JwtOptions>(_configuration.GetSection(nameof(JwtOptions)));

            services.AddScoped<IAuthService, AuthService>();
            services.AddFriendlyJwt();
            
            services.AddAutoMapper((config) =>
            {
                config.AddMaps(typeof(AuthProfile).Assembly);
            });
            
            services.AddControllers()
                    .AddNewtonsoftJson(options =>
                    {
                        options.SerializerSettings.Converters.Add(
                            new StringEnumConverter(new CamelCaseNamingStrategy())
                        );

                        options.SerializerSettings.Converters.Add(new StringEnumConverter());
                    })
                    .AddFriendlyJwtAuthentication(configuration =>
                    {
                        var jwtOptions = new JwtOptions();
                        _configuration.GetSection(nameof(JwtOptions)).Bind(jwtOptions);

                        configuration.Audience = jwtOptions.Audience;
                        configuration.Issuer = jwtOptions.Issuer;
                        configuration.Secret = jwtOptions.Secret;
                    });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "EMark.Api", Version = "v1"});

                c.MapType<RoleModel>(() => new OpenApiSchema()
                {
                    Enum = new List<IOpenApiAny>()
                    {
                        new OpenApiString(nameof(RoleModel.Teacher)),
                        new OpenApiString(nameof(RoleModel.Student))
                    }
                });
                
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EMark.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}