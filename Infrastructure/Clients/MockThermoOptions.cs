namespace Novonesis.AlarmService.Infrastructure.Clients {
    /// <summary>
    /// Settings for the mock provider used during local development.
    /// Gradually increases temperature from a base value with random noise.
    /// </summary>
    public sealed class MockThermoOptions {
        /// <summary>
        /// Initial base temperature.
        /// </summary>
        public double BaseTemp { get; set; } = 5.0;

        /// <summary>
        /// Maximum random noise amplitude added each reading.
        /// </summary>
        public double NoiseRange { get; set; } = 0.2;

        /// <summary>
        /// Minimum temperature increase rate per minute.
        /// </summary>
        public double MinIncreaseRate { get; set; } = 0.3;

        /// <summary>
        /// Maximum temperature increase rate per minute.
        /// </summary>
        public double MaxIncreaseRate { get; set; } = 0.8;
    }
}