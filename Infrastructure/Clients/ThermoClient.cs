using Novonesis.AlarmService.Core;
using Novonesis.AlarmService.Core.Model;

namespace Novonesis.AlarmService.Infrastructure.Clients;

public sealed class ThermoClient : IThermoClient {
    public Task<IReadOnlyList<Reading>> GetReadingsAsync(CancellationToken ct) {
        // TODO: implement when endpoint details are known
        throw new NotImplementedException();
    }
}
