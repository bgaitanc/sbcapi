using SBC.Application.Models.Dashboard;

namespace SBC.Application.Services.Interfaces;

public interface IDashboardService
{
    Task<DashboardSummaryDto> GetSummaryAsync();
}
