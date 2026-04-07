using SBC.Application.Models.Accounting;

namespace SBC.Application.Models.Dashboard;

public class DashboardSummaryDto
{
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal TotalEquity { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal TotalCosts { get; set; }
    public decimal TotalExpenses { get; set; }
    public decimal NetIncome { get; set; }
    public List<RecentMovementDto> RecentMovements { get; set; } = [];
    public List<TopAccountDto> TopAccounts { get; set; } = [];
}

public class RecentMovementDto
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; }
    public decimal TotalAmount { get; set; }
}

public class TopAccountDto
{
    public Guid AccountId { get; set; }
    public string AccountCode { get; set; }
    public string AccountName { get; set; }
    public int MovementCount { get; set; }
    public decimal TotalAmount { get; set; }
}
