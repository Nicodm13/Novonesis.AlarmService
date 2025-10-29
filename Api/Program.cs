using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Novonesis.AlarmService.Api.Workers;
using Novonesis.AlarmService.Core;
using Novonesis.AlarmService.Infrastructure.Clients;
using Novonesis.AlarmService.Infrastructure.Notifiers;

var builder = WebApplication.CreateBuilder(args);

// Auth for S2S
var azureAd = builder.Configuration.GetSection("AzureAd");
if (!string.IsNullOrWhiteSpace(azureAd["ClientId"])) {
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddMicrosoftIdentityWebApi(azureAd);
}
builder.Services.AddAuthorization();

// Domain config
var max = builder.Configuration.GetValue<double>("Alarm:MaxC", 50.0);
var sustain = TimeSpan.FromSeconds(builder.Configuration.GetValue<int>("Alarm:SustainSeconds", 300));
builder.Services.AddSingleton(new AlarmRule(max, sustain));

// Notifier
builder.Services.AddSingleton<INotifier, LogNotifier>();

// Setup provider
if (builder.Environment.IsDevelopment()) {
    // Moch in Development
    builder.Services.Configure<MockThermoOptions>(
        builder.Configuration.GetSection("MockThermo"));
    builder.Services.AddSingleton<IThermoClient, MockThermoClient>();
}
else {
    // HTTP Client
    builder.Services.AddSingleton<IThermoClient, ThermoClient>();
}

// Worker + API
builder.Services.AddHostedService<AlarmMonitoringWorker>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/health/live", () => Results.Ok(new { status = "live" })).AllowAnonymous();
app.MapGet("/health/ready", () => Results.Ok(new { status = "ready" })).AllowAnonymous();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
