namespace SBC.Application.Models.Accounting;

public class IncomeStatementDto
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public List<IncomeStatementLineDto> Revenues { get; set; } = [];
    public decimal TotalRevenues { get; set; }
    public List<IncomeStatementLineDto> Costs { get; set; } = [];
    public decimal TotalCosts { get; set; }
    public decimal GrossProfit => TotalRevenues - TotalCosts;
    public List<IncomeStatementLineDto> Expenses { get; set; } = [];
    public decimal TotalExpenses { get; set; }
    public decimal NetIncome => GrossProfit - TotalExpenses;
}

public class IncomeStatementLineDto
{
    public Guid AccountId { get; set; }
    public string AccountCode { get; set; }
    public string AccountName { get; set; }
    public decimal Amount { get; set; }
}
