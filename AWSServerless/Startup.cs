using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AWSServerless.IServices;
using AWSServerless.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace AWSServerless
{
    public class Startup
    {
        readonly string AllowAll = "_allowAll";
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;

            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddConfiguration(configuration)
            .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public static IConfiguration Configuration { get; private set; }

        // This method gets called by the runtime. Use this method to add services to the container
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<IPresignedUrlService, PresignedUrlService>();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "AWSServerless", Version = "v1" });
            });

            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowAll,
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        ;
                    });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddLambdaLogger(Configuration.GetLambdaLoggerOptions());

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            string swaggerActive = Environment.GetEnvironmentVariable("swaggerActive");
            string swaggerActiveAWS = Environment.GetEnvironmentVariable("swaggerActiveAWS");
            bool flag1 = bool.Parse(swaggerActive);
            bool flag2 = bool.Parse(swaggerActiveAWS);

            string swaggerUrl = "";
            string swaggerJSON = "";


            if (flag1.Equals(true))
            {
                if (flag2)
                {
                    swaggerUrl = Environment.GetEnvironmentVariable("swaggerUrl");
                    swaggerJSON = Environment.GetEnvironmentVariable("swaggerJSON");
                }
                else
                {
                    swaggerUrl = "/swagger/index.html";
                    swaggerJSON = "/swagger/v1/swagger.json";
                }

                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint(swaggerJSON, "AWSServerless v1"));
                /// Prod / swagger / v1 / swagger.json
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseCors(AllowAll);

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapGet("/", async context =>
                {
                    string html = $@"<!doctype html>
<html lang='en'>

<head>
    <!-- Required meta tags -->
    <meta charset='utf-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1'>

    <!-- Bootstrap CSS -->
    <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.1.1/dist/css/bootstrap.min.css' rel='stylesheet'
        integrity='sha384-F3w7mX95PdgyTmZZMECAngseQB83DfGTowi0iMjiWaeVhAn4FJkqJByhZMI3AhiU' crossorigin='anonymous'>
    <link href='https://maxcdn.bootstrapcdn.com/font-awesome/4.7.0/css/font-awesome.min.css' rel='stylesheet'>

    <link rel='stylesheet' href='https://unpkg.com/@coreui/icons/css/all.min.css'>


    <title>Hello, world!</title>
</head>

<body>

    <div class='px-4 py-5 my-5 text-center'>
        <img class='d-block mx-auto mb-4' src='	https://getbootstrap.com/docs/5.1/assets/brand/bootstrap-logo.svg' alt='' width='72' height='57'>
        <h1 class='display-5 fw-bold'>Welcome to running ASP.NET Core on AWS Lambda</h1>
        <div class='col-lg-6 mx-auto'>
            <p class='lead mb-4'>Quickly design and customize responsive mobile-first sites with Bootstrap, the world’s
                most popular front-end open source toolkit, featuring Sass variables and mixins, responsive grid system,
                extensive prebuilt components, and powerful JavaScript plugins.</p>
        </div>
    </div>



    <div class=''>
        <div class='container'>
            <div class='row'>

                <div class='col-lg-4 col-xs-12 text-center mb-3'>
                    <div class=''>
                        <i class='fa fa-github fa-3x' aria-hidden='true'></i>
                        <div class=''>
                            <h3>Github</h3>
                        </div>
                        <div class=''>
                            <span>Lorem ipsum dolor sit amet, id quo eruditi eloquentiam. Assum decore te sed. Elitr
                                scripta ocurreret qui ad.</span>
                        </div>
                        <div class=''>
                            <a href='https://github.com/patricioarena/ProgramacionEnTiempoReal'>Learn More</a>
                        </div>
                    </div>
                </div>

                <div class='col-lg-4 col-xs-12 text-center mb-3'>
                    <div class=''>
                        <img src='https://petstore.swagger.io/favicon-32x32.png' width='50' height='50'>
                        <div class=''>
                            <h3>Swagger</h3>
                        </div>
                        <div class=''>
                            <span>Lorem ipsum dolor sit amet, id quo eruditi eloquentiam. Assum decore te sed. Elitr
                                scripta ocurreret qui ad.</span>
                        </div>
                        <div class=''>
                            <a href='{swaggerUrl}'>Learn More</a>
                        </div>
                    </div>
                </div>

                <div class='col-lg-4 col-xs-12  text-center mb-3'>
                    <div class=''>
                        <i class='fa fa-twitter fa-3x' aria-hidden='true'></i>
                        <div class=''>
                            <h3>Twitter</h3>
                        </div>
                        <div class=''>
                            <span>Lorem ipsum dolor sit amet, id quo eruditi eloquentiam. Assum decore te sed. Elitr
                                scripta ocurreret qui ad.</span>
                        </div>
                        <div class=''>
                            <a href='#'>Learn More</a>
                        </div>
                    </div>
                </div>

                <div class='col-lg-4 col-xs-12 text-center mb-3'>
                    <div class=''>
                        <i class='fa fa-facebook fa-3x' aria-hidden='true'></i>
                        <div class=''>
                            <h3>Facebook</h3>
                        </div>
                        <div class=''>
                            <span>Lorem ipsum dolor sit amet, id quo eruditi eloquentiam. Assum decore te sed. Elitr
                                scripta ocurreret qui ad.</span>
                        </div>
                        <div class=''>
                            <a href='#'>Learn More</a>
                        </div>
                    </div>
                </div>

                <div class='col-lg-4 col-xs-12 text-center mb-3'>
                    <div class=''>
                        <i class='fa fa-pinterest-p fa-3x' aria-hidden='true'></i>
                        <div class=''>
                            <h3>Pinterest</h3>
                        </div>
                        <div class=''>
                            <span>Lorem ipsum dolor sit amet, id quo eruditi eloquentiam. Assum decore te sed. Elitr
                                scripta ocurreret qui ad.</span>
                        </div>
                        <div class=''>
                            <a href='#'>Learn More</a>
                        </div>
                    </div>
                </div>

                <div class='col-lg-4 col-xs-12 text-center mb-3'>
                    <div class=''>
                        <i class='fa fa-google-plus fa-3x' aria-hidden='true'></i>
                        <div class=''>
                            <h3>Google</h3>
                        </div>
                        <div class=''>
                            <span>Lorem ipsum dolor sit amet, id quo eruditi eloquentiam. Assum decore te sed. Elitr
                                scripta ocurreret qui ad.</span>
                        </div>
                        <div class=''>
                            <a href='#'>Learn More</a>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
</body>

</html>";

                    await context.Response.WriteAsync(html);
                });
            });
        }
    }
}