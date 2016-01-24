using ImageUploadDemo.Helpers;
using ImageUploadDemo.ImageProcessProvider;
using ImageUploadDemo.models;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ImageUploadDemo
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; set; }

        public Startup(IHostingEnvironment env)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));
            //services.AddTransient<IImageProvider, AzureBlobProvider>();
            services.AddTransient<IImageProvider, LocalFileProvider>();
            services.AddSingleton<IResizer, GoogleCDNImageResizehelper>();
            services.AddSingleton<IFileSystemHelper, FileSystemHelper>();
            services.AddSingleton<IHttpFactory, HttpFactory>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseIISPlatformHandler();
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }

        // Entry point for the application.
        public static void Main(string[] args) => WebApplication.Run<Startup>(args);
    }
}
