using Microsoft.AspNetCore.Mvc;
using PeblClone.Application.Workforce;

namespace PeblClone.Api.Controllers;

[ApiController]
[Route("api/workforce")]
public sealed class WorkforceController(
    WorkforceDashboardService dashboardService,
    IHiringQuoteCalculator quoteCalculator) : ControllerBase
{
    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(WorkforceDashboardDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<WorkforceDashboardDto>> GetDashboard(CancellationToken cancellationToken)
    {
        var dashboard = await dashboardService.GetDashboardAsync(cancellationToken);
        return Ok(dashboard);
    }

    [HttpPost("quotes")]
    [ProducesResponseType(typeof(HiringQuoteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<HiringQuoteDto>> CreateQuote(
        [FromBody] HiringQuoteRequest request,
        CancellationToken cancellationToken)
    {
        var result = await quoteCalculator.CalculateAsync(request, cancellationToken);

        if (!result.IsSuccess || result.Value is null)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Quote could not be created",
                Detail = result.Error,
                Status = StatusCodes.Status400BadRequest
            });
        }

        return Ok(result.Value);
    }
}
