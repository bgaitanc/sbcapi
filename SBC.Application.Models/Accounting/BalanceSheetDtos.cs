namespace SBC.Application.Models.Accounting;

public class BalanceSheetDto
{
    public DateTime Date { get; set; }
    public List<BalanceSheetLineDto> Assets { get; set; } = [];
    public decimal TotalAssets { get; set; }
    public List<BalanceSheetLineDto> Liabilities { get; set; } = [];
    public decimal TotalLiabilities { get; set; }
    public List<BalanceSheetLineDto> Equity { get; set; } = [];
    public decimal TotalEquity { get; set; }
    public decimal NetIncome { get; set; }
    public decimal TotalLiabilitiesAndEquity => TotalLiabilities + TotalEquity + NetIncome;
}

public class BalanceSheetLineDto
{
    public Guid AccountId { get; set; }
    public string AccountCode { get; set; }
    public string AccountName { get; set; }
    public decimal Amount { get; set; }
}
