using FluentAssertions;
using PeblClone.Application.Workforce;
using PeblClone.Infrastructure.Workforce;
using Xunit;

namespace PeblClone.Application.Tests;

public sealed class WorkforceDashboardServiceTests
{
    [Fact]
    public async Task GetDashboardAsync_ComposesSummaryFromReadModel()
    {
        var service = new WorkforceDashboardService(new InMemoryWorkforceReadModel());

        var dashboard = await service.GetDashboardAsync(CancellationToken.None);

        dashboard.Summary.TotalWorkers.Should().Be(5);
        dashboard.Summary.ActiveCountries.Should().Be(5);
        dashboard.Summary.WorkersNeedingAttention.Should().Be(1);
        dashboard.ComplianceAlerts.Should().NotBeEmpty();
        dashboard.Workers[0].RequiresAttention.Should().BeTrue();
    }
}
