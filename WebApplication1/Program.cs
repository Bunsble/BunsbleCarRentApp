using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            // Configure error handling for other environments
            // For example:
            // app.UseExceptionHandler("/Error");
            // app.UseStatusCodePagesWithRedirects("/Error/{0}");
        }

        // Other middleware configurations...
    }


    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.ConfigureServices((hostContext, services) =>
                {
                    services.AddRazorPages();
                    services.AddHttpContextAccessor(); // Add registration for IHttpContextAccessor

                    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                        .AddCookie(options =>
                        {
                            options.Cookie.Name = "YourCookieName"; // Optional: customize cookie name
                            options.LoginPath = "/Login"; // Optional: set login page path
                        });
                });

                webBuilder.Configure((hostContext, app) =>
                {
                    if (hostContext.HostingEnvironment.IsDevelopment())
                    {
                        app.UseDeveloperExceptionPage();
                    }
                    else
                    {
                        app.UseExceptionHandler("/Error");
                        app.UseHsts();
                    }

                    app.UseHttpsRedirection();
                    app.UseStaticFiles();

                    app.UseRouting();

                    app.UseAuthentication(); // Enable authentication middleware
                    app.UseAuthorization();

                    app.UseEndpoints(endpoints =>
                    {
                        endpoints.MapRazorPages();
                    });
                });
            });
}
