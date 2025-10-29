namespace Novonesis.AlarmService.Core {
    /// <summary>
    /// Contract for sending alarms to an external channel.
    /// </summary>
    public interface INotifier {
        /// <summary>
        /// Sends a notification for a specific container.
        /// </summary>
        Task NotifyAsync(string containerId, string message, CancellationToken ct);
    }
}
