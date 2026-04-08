using StreamMirrorer.Interfaces;
using StreamMirrorer.Recorders;
using StreamMirrorer.Services;
using Serilog;
using Serilog.Configuration;
using Serilog.Core;
using Serilog.Events;
using StreamMirrorer.Components;
using StreamMirrorer.Utility;
using StreamMirrorer.Utility.DataSavers;

namespace StreamMirrorer;

internal class Program
{
    public static void Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        //setup logging
        string processName = builder.Configuration["ProcessName"] ?? throw new NullReferenceException();
        string seqServerUrl = builder.Configuration["Seq:ServerUrl"] ?? throw new NullReferenceException();
        string seqApiKey = builder.Configuration["Seq:ApiKey"] ?? throw new NullReferenceException();
        string minLogLevel = builder.Configuration["Seq:MinLogLevel"] ?? throw new NullReferenceException();
        IDictionary<string, string> minLogServices = builder.Configuration.GetSection("Seq:MinLogServices")
            .Get<IDictionary<string, string>>() ?? throw new NullReferenceException();

        LoggerConfiguration logConfig = new LoggerConfiguration()
            .MinimumLevel.Is((LogEventLevel)int.Parse(minLogLevel))
            .Enrich.WithProperty("XPG", processName)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.Seq(seqServerUrl, apiKey: seqApiKey);

        //set min log level per service
        foreach (KeyValuePair<string, string> logService in minLogServices)
        {
            logConfig.MinimumLevel.Override(logService.Key, (LogEventLevel)int.Parse(logService.Value));
        }
        
        Log.Logger = logConfig.CreateLogger();
        
        builder.Host.UseSerilog();

        // Add services to the container.
        IServiceCollection services = builder.Services;

        
        services.AddSingleton<IRecordController, RecordController>();
        services.AddSingleton<RecorderFactory>();

        //Recorders
        services.AddTransient<TwitchRecorder>();


        services.AddControllers();

        services.AddRazorComponents()
            .AddInteractiveServerComponents();


        WebApplication app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error", createScopeForErrors: true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapControllers().DisableAntiforgery();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode()
            .DisableAntiforgery();

        app.Run();
    }
}