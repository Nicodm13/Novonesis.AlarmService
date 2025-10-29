using Microsoft.Extensions.Options;
using Novonesis.AlarmService.Core;
using Novonesis.AlarmService.Core.Model;

namespace Novonesis.AlarmService.Infrastructure.Clients {
    /// <summary>
    /// Simulation that gradually warms with small random noise.
    /// </summary>
    public sealed class MockThermoClient : IThermoClient {
        private readonly Random _rng = new();
        private readonly string[] _ids = new[] { "CT-1001", "CT-1002", "CT-1003" };
        private readonly MockThermoOptions _options;
        private readonly Dictionary<string, double> _current = new();
        private DateTimeOffset _lastTick;

        public MockThermoClient(IOptions<MockThermoOptions> options) {
            _options = options.Value;
            _lastTick = DateTimeOffset.UtcNow;

            foreach (var id in _ids)
                _current[id] = _options.BaseTemp;
        }

        public Task<IReadOnlyList<Reading>> GetReadingsAsync(CancellationToken ct) {
            var now = DateTimeOffset.UtcNow;
            var minutes = Math.Max(0.0, (now - _lastTick).TotalMinutes);
            var list = new List<Reading>(_ids.Length);

            foreach (var id in _ids) {
                // pick a random increase rate between min and max
                var rate = _options.MinIncreaseRate +
                           (_options.MaxIncreaseRate - _options.MinIncreaseRate) * _rng.NextDouble();

                // temperature increase from rate
                var rise = rate * minutes;
                var noise = _rng.NextDouble() * _options.NoiseRange;

                _current[id] += rise + noise;

                list.Add(new Reading(id, now, _current[id]));
            }

            _lastTick = now;
            return Task.FromResult<IReadOnlyList<Reading>>(list);
        }
    }
}