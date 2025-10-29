namespace Novonesis.AlarmService.Core.Model {
    /// <summary>
    /// Recpord record for a refrigerated container.
    /// </summary>
    /// <param name="ContainerId">Provider-issued container identifier.</param>
    /// <param name="Ts">UTC timestamp when the sample was measured.</param>
    /// <param name="TempC">Temperature in °C.</param>
    public sealed record Reading(string ContainerId, DateTimeOffset Ts, double TempC);
}
