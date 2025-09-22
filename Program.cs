using Recorder.Interfaces;
using Recorder.Recorders;
using Recorder.Services;
using Serilog;
using Serilog.Events;
using StreamMirrorer.Components;

var builder = WebApplication.CreateBuilder(args);

//setup logging
string processName = builder.Configuration["ProcessName"] ?? throw new NullReferenceException();
string seqServerUrl = builder.Configuration["Seq:ServerUrl"] ?? throw new NullReferenceException();
string seqApiKey = builder.Configuration["Seq:ApiKey"] ?? throw new NullReferenceException();
string minLogLevel = builder.Configuration["Seq:MinLogLevel"] ?? throw new NullReferenceException();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Is((LogEventLevel)int.Parse(minLogLevel))
    .Enrich.WithProperty("XPG", processName)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(seqServerUrl, apiKey: seqApiKey)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
IServiceCollection services = builder.Services;

services.AddSingleton<IRecordController, RecordController>();

//Recorder implementations
services.AddTransient<IRecorder, TwitchRecorder>();

services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

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
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();