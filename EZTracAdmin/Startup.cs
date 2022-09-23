using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EZTracAdminRSC.Models;
using Microsoft.Owin;
using Owin;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.Owin.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
namespace EZTracAdminRSC
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
            services.AddMvc();
       
            // Allow sign in via an OpenId Connect provider like EmpowerID
            services.AddAuthentication(options => {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
            .AddCookie(options => {
            // options.LoginPath = "/Account/Login/";
            })
            .AddOpenIdConnect(options =>
            {
                options.ClientId = Configuration["AppSettings:clientID"];
                options.ClientSecret = Configuration["AppSettings:clientSecret"];
                options.Authority = Configuration["AppSettings:Authority"];
                
                options.ResponseMode = OpenIdConnectResponseMode.Query;
                options.ResponseType = "code";
                options.ClaimsIssuer = "EmpowerID";
                options.TokenValidationParameters.ValidIssuer = "EmpowerID";
                options.ReturnUrlParameter = Configuration["AppSettings:redirectURI"];
                options.Events.OnRedirectToIdentityProvider = async n =>
                {
                    n.ProtocolMessage.RedirectUri = Configuration["AppSettings:redirectURI"];
                    await Task.FromResult(0);
                };
                
                //Change later!
                options.ProtocolValidator.RequireNonce = false;
                options.GetClaimsFromUserInfoEndpoint = false;
                
                options.CallbackPath = Configuration["AppSettings:callBackURL"];
            }
            );
            services.AddDataProtection();
            services.Configure<OidcOptions>(Configuration.GetSection("oidc"));

            // services.Configure<AWS>(Configuration.GetSection("AWS"));

            /*
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "dynamodb_sample", Version = "v1" });
            });

            var credentials = new BasicAWSCredentials("(Access Key Id)", "(Secret Value)");
            var config = new AmazonDBConfig()
            {
                RegionEndpoint = RegionEndpoint.USWest2;
            };
            var client = new AmazonDBClient(credentials, config);
            services.AddSingleton<IAmazonDynamoDB>(client);
            services.AddSingleton<IDynamoDBContext, DynamoDBContext>();
            */
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseAuthentication();
            
            app.UseCors(builder =>
                builder.WithOrigins("http://localhost:44592/"  /* et. al. */)
                .AllowAnyHeader() // allow 'Authentication' headers, et. al.
                .AllowAnyMethod() // allow GET, SET, OPTIONS, et. al.
                                    // .....
           );   // could also be an issue
            
            app.UseStaticFiles();
            app.UseCookiePolicy(new CookiePolicyOptions()
            {
                MinimumSameSitePolicy = (Microsoft.AspNetCore.Http.SameSiteMode)SameSiteMode.None
            }); // check this section
            app.UseRouting();
            app.UseAuthorization();

            // app.UseHttpsRedirection();

            // This is needed if running behind a reverse proxy
            // like ngrok which is great for testing while developing
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                RequireHeaderSymmetry = false,
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();

            });
        }
    }
}
