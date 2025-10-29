using Novonesis.AlarmService.Core.Model;

namespace Novonesis.AlarmService.Core {
    /// <summary>
    /// Temperature rule with sustain window and de-duplication.
    /// Tracks per-container violation start and emits once the breach persists.
    /// </summary>
    public sealed class AlarmRule {

        public double MaxC => _maxC;

        private readonly double _maxC;
        private readonly TimeSpan _sustain;
        private readonly Dictionary<string, (DateTimeOffset start, double last)> _violations = new();

        /// <summary>
        /// Create a new temperature rule.
        /// </summary>
        /// <param name="minC">Inclusive minimum °C.</param>
        /// <param name="maxC">Inclusive maximum °C.</param>
        /// <param name="sustain">How long the breach must last before alarm.</param>
        public AlarmRule(double maxC, TimeSpan sustain) {
            _maxC = maxC;
            _sustain = sustain;
        }

        /// <summary>
        /// Evaluate a batch and yield container ids that should alarm now.
        /// Clears state for recovered containers.
        /// </summary>
        public IEnumerable<string> Evaluate(IEnumerable<Reading> readings, DateTimeOffset now) {
            foreach (var group in readings.GroupBy(r => r.ContainerId)) {
                var id = group.Key;

                var violating = group
                    .Where(r => r.TempC > _maxC)
                    .OrderBy(r => r.Ts)
                    .LastOrDefault();

                if (violating is null) {
                    _violations.Remove(id);
                    continue;
                }

                if (!_violations.TryGetValue(id, out var v)) {
                    _violations[id] = (violating.Ts, violating.TempC);
                }
                else {
                    _violations[id] = (v.start, violating.TempC);
                }

                if (now - _violations[id].start >= _sustain) {
                    _violations.Remove(id);
                    yield return id;
                }
            }
        }
    }
}
