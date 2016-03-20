using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNet.Authentication.OAuth;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NannyApp.Models;
using NannyApp.Services;
using System.Threading;
using System.Data.SqlClient;

namespace NannyApp
{
    public class Startup
    {
        private Timer threadingTimer;
        string connection = null;
        string command = null;
        string parameterName = null;
        string methodName = null;

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.

            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();

                // This will push telemetry data through Application Insights pipeline faster, allowing you to view results immediately.
                builder.AddApplicationInsightsSettings(developerMode: true);
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            connection = Configuration["Data:DefaultConnection:ConnectionString"];
            StartTimer();
        }

        public IConfigurationRoot Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddApplicationInsightsTelemetry(Configuration);

            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<ApplicationDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddMvc();

            // Add application services.
            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();
            services.Configure<AuthMessageSenderOptions>(options =>
            {
                options.SendGridUser = Configuration["SENDGRID_USER"];
                options.SendGridPassword = Configuration["SENDGRID_PASS"];
                options.SendGridKey = Configuration["SENDGRID_APIKEY"];
            });
            services.Configure<AuthMessageSMSSenderOptions>(options =>
            {
                options.SID = Configuration["TWILIO_SID"];
                options.AuthToken = Configuration["TWILIO_AUTHTOKEN"];
                options.SendNumber = Configuration["TWILIO_NUMBER"];
            });
            services.AddSingleton<IFileService, FileService>();
            services.Configure<FileServiceOptions>(options =>
            {
                options.StorageConnectionString = Configuration["StorageConnectionString"];
            });
            services.AddScoped<INannyAppRepository, NannyAppRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseApplicationInsightsRequestTelemetry();

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");

                // For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
                try
                {
                    using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
                        .CreateScope())
                    {
                        serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
                            .Database.Migrate();
                    }
                }
                catch { }
            }

            app.UseIISPlatformHandler(options => options.AuthenticationDescriptions.Clear());

            app.UseApplicationInsightsExceptionTelemetry();

            app.UseStaticFiles();

            app.UseIdentity();

            app.UseFacebookAuthentication(options =>
            {
                options.AppId = Configuration["Authentication:Facebook:AppId"];
                options.AppSecret = Configuration["Authentication:Facebook:AppSecret"];
                options.Scope.Add("email");
                options.Scope.Add("user_birthday");
                options.SaveTokensAsClaims = true;
                options.BackchannelHttpHandler = new HttpClientHandler();
                options.UserInformationEndpoint = "https://graph.facebook.com/v2.5/me?fields=id,name,birthday,first_name,last_name,email";
                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = (context) =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim("FacebookAccessToken", context.AccessToken));
                        foreach (var claim in context.User)
                        {
                            var claimType = string.Format("urn:facebook:{0}", claim.Key);
                            string claimValue = claim.Value.ToString();
                            if (!context.Identity.HasClaim(claimType, claimValue))
                                context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, "XmlSchemaString", "Facebook"));
                        }
                        return Task.FromResult(0);
                    }
                };
            });

            app.UseGoogleAuthentication(options =>
            {
                options.ClientId = Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
                options.Scope.Add("https://www.googleapis.com/auth/plus.login");
                options.Scope.Add("https://www.googleapis.com/auth/plus.profile.emails.read");
                options.Events = new OAuthEvents
                {
                    OnCreatingTicket = (context) =>
                    {
                        context.Identity.AddClaim(new System.Security.Claims.Claim("GoogleAccessToken", context.AccessToken));
                        foreach (var claim in context.User)
                        {
                            var claimType = string.Format("urn:google:{0}", claim.Key);
                            string claimValue = claim.Value.ToString();
                            if (!context.Identity.HasClaim(claimType, claimValue))
                                context.Identity.AddClaim(new System.Security.Claims.Claim(claimType, claimValue, "XmlSchemaString", "Google"));
                        }
                        return Task.FromResult(0);
                    }
                };
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);

        #region DeleteUser
        private void DeleteUserFromDatabase(string userid)
        {
            List<CommAndParams> commAndParam = new List<CommAndParams>();
            commAndParam.Add(new CommAndParams() { command = "DELETE FROM AspNetUserLogins WHERE UserId = @UserId", parameterName = "@UserId" });
            commAndParam.Add(new CommAndParams() { command = "DELETE FROM AspNetUserClaims WHERE UserId = @UserId", parameterName = "@UserId" });
            commAndParam.Add(new CommAndParams() { command = "DELETE FROM AspNetUserRoles WHERE UserId = @UserId", parameterName = "@UserId" });
            commAndParam.Add(new CommAndParams() { command = "DELETE FROM AspNetUsers WHERE Id = @Id", parameterName = "@Id" });
            foreach (var cap in commAndParam)
            {
                command = cap.command;
                parameterName = cap.parameterName;
                using (SqlConnection myConnection = new SqlConnection(connection))
                using (SqlCommand cmd = new SqlCommand(command, myConnection))
                {
                    cmd.Parameters.AddWithValue(parameterName, userid);
                    myConnection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            cmd.ExecuteNonQuery();
                            myConnection.Close();
                        }
                    }
                }
            }
        }

        private void DeleteUser()
        {
            using (SqlConnection myConnection = new SqlConnection(connection))
            using (SqlCommand cmd = new SqlCommand(command, myConnection))
            {
                cmd.Parameters.AddWithValue(parameterName, false);
                myConnection.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        // Read advances to the next row.
                        if (reader.Read())
                        {
                            if (methodName == "DeleteUncorfirmedAccounts")
                            {
                                DateTime emailLinkDate = (DateTime)reader["EmailLinkDate"];
                                if (emailLinkDate.AddHours(1) < DateTime.Now)
                                {
                                    DeleteById();
                                }
                            }
                            else if (methodName == "DeleteById")
                            {
                                string userid = reader["Id"].ToString();
                                DeleteUserFromDatabase(userid);
                            }
                        }
                        myConnection.Close();
                    }
                }
            }
        }

        private void DeleteById()
        {
            command = "SELECT Id AS Id FROM AspNetUsers WHERE EmailConfirmed = @EmailConfirmed";
            parameterName = "@EmailConfirmed";
            methodName = "DeleteById";
            DeleteUser();
        }
        private void DeleteUncorfirmedAccounts(object sender)
        {
            command = "SELECT EmailLinkDate AS EmailLinkDate FROM AspNetUsers WHERE EmailConfirmed = @EmailConfirmed";
            parameterName = "@EmailConfirmed";
            methodName = "DeleteUncorfirmedAccounts";
            DeleteUser();
        }

        private void StartTimer()
        {
            if (threadingTimer == null)
            {
                //raise timer callback 
                string str = DateTime.Now.ToLongTimeString(); threadingTimer = new Timer(new TimerCallback(DeleteUncorfirmedAccounts), str, 1000, 1000);
            }
        }
        #endregion DeleteUser
    }

    public class CommAndParams
    {
        public string command { get; set; }
        public string parameterName { get; set; }
    }
}
