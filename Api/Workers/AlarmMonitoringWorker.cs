using Novonesis.AlarmService.Core;

namespace Novonesis.AlarmService.Api.Workers {
    /// <summary>
    /// Polls the provider and routes sustained breaches to the notifier.
    /// </summary>
    public sealed class AlarmMonitoringWorker(
        IThermoClient thermo,
        AlarmRule rule,
        INotifier notifier,
        ILogger<AlarmMonitoringWorker> log) : BackgroundService {
        protected override async Task ExecuteAsync(CancellationToken ct) {
            while (!ct.IsCancellationRequested) {
                try {
                    var readings = await thermo.GetReadingsAsync(ct);
                    var now = DateTimeOffset.UtcNow;

                    // Show current temps
                    foreach (var r in readings)
                        Console.WriteLine($"[{r.ContainerId}] {r.TempC:F1}°C");

                    // Cache latest reading per container
                    var latest = readings
                        .GroupBy(r => r.ContainerId)
                        .ToDictionary(g => g.Key, g => g.OrderBy(r => r.Ts).Last());

                    foreach (var id in rule.Evaluate(readings, now)) {
                        var r = latest[id];
                        var msg = $"Temperature above allowed maximum {rule.MaxC:F1}°C; current={r.TempC:F1}°C";
                        await notifier.NotifyAsync(id, msg, ct);
                    }
                }
                catch (Exception ex) {
                    log.LogError(ex, "Monitor loop error");
                }
                await Task.Delay(TimeSpan.FromSeconds(5), ct);
            }
        }
    }
}
