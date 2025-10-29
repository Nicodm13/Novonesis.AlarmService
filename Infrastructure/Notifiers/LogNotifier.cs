using Novonesis.AlarmService.Core;

namespace Novonesis.AlarmService.Infrastructure.Notifiers {
    /// <summary>
    /// Writes alarms to logs. Useful during development and tests.
    /// </summary>
    public sealed class LogNotifier : INotifier {
        private readonly ILogger<LogNotifier> _log;

        public LogNotifier(ILogger<LogNotifier> log) => _log = log;

        /// <inheritdoc />
        public Task NotifyAsync(string containerId, string message, CancellationToken ct) {
            _log.LogWarning("ALARM containerId={ContainerId} message={Message}", containerId, message);
            return Task.CompletedTask;
        }
    }
}
