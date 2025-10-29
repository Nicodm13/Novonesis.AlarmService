using Novonesis.AlarmService.Core.Model;

namespace Novonesis.AlarmService.Core {
    /// <summary>
    /// Contract for fetching container readings from any vendor.
    /// </summary>
    public interface IThermoClient {
        /// <summary>
        /// Returns recent readings across one or more containers.
        /// </summary>
        Task<IReadOnlyList<Reading>> GetReadingsAsync(CancellationToken ct);
    }
}
