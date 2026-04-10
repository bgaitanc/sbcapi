using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Enums;
using SBC.Domain.Entities.Logging;
using SBC.Domain.Repositories.Interfaces;
using System.Text.Json;

namespace SBC.Application.Services.Implementation;

public class FinancialReportService(
    IJournalEntryRepository journalEntryRepository,
    ITransactionLogService transactionLogService) : IFinancialReportService
{
    static FinancialReportService()
    {
        QuestPDF.Settings.License = LicenseType.Community;
    }
    public async Task<IncomeStatementDto> GetIncomeStatementAsync(DateTime startDate, DateTime endDate, bool includeUnposted = false)
    {
        var entries = await journalEntryRepository.GetByDateRangeWithLinesAsync(startDate, endDate, includeUnposted);
        var allLines = entries.SelectMany(e => e.Lines).ToList();

        var report = new IncomeStatementDto
        {
            StartDate = startDate,
            EndDate = endDate,
            IsProvisional = includeUnposted
        };

        // Revenues
        var revenueLines = allLines
            .Where(l => l.Account.Type == AccountType.Revenue)
            .GroupBy(l => new { l.AccountId, l.Account.Code, l.Account.Name })
            .Select(g => new IncomeStatementLineDto
            {
                AccountId = g.Key.AccountId,
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                Amount = g.Sum(l => l.Credit - l.Debit)
            })
            .Where(l => l.Amount != 0)
            .OrderBy(l => l.AccountCode)
            .ToList();

        report.Revenues = revenueLines;
        report.TotalRevenues = revenueLines.Sum(r => r.Amount);

        // Costs
        var costLines = allLines
            .Where(l => l.Account.Type == AccountType.Cost)
            .GroupBy(l => new { l.AccountId, l.Account.Code, l.Account.Name })
            .Select(g => new IncomeStatementLineDto
            {
                AccountId = g.Key.AccountId,
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                Amount = g.Sum(l => l.Debit - l.Credit)
            })
            .Where(l => l.Amount != 0)
            .OrderBy(l => l.AccountCode)
            .ToList();

        report.Costs = costLines;
        report.TotalCosts = costLines.Sum(c => c.Amount);

        // Expenses
        var expenseLines = allLines
            .Where(l => l.Account.Type == AccountType.Expense)
            .GroupBy(l => new { l.AccountId, l.Account.Code, l.Account.Name })
            .Select(g => new IncomeStatementLineDto
            {
                AccountId = g.Key.AccountId,
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                Amount = g.Sum(l => l.Debit - l.Credit)
            })
            .Where(l => l.Amount != 0)
            .OrderBy(l => l.AccountCode)
            .ToList();

        report.Expenses = expenseLines;
        report.TotalExpenses = expenseLines.Sum(e => e.Amount);

        await transactionLogService.LogTransactionAsync(null, TransactionActions.GenerateIncomeStatement, nameof(IncomeStatementDto), null, TransactionStatus.Success, JsonSerializer.Serialize(new { startDate, endDate, includeUnposted }));

        return report;
    }

    public async Task<byte[]> GenerateIncomeStatementExcelAsync(DateTime startDate, DateTime endDate, bool includeUnposted = false)
    {
        var report = await GetIncomeStatementAsync(startDate, endDate, includeUnposted);
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Estado de Resultados");

        var currentRow = 1;
        worksheet.Cell(currentRow, 1).Value = "ESTADO DE RESULTADOS";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
        worksheet.Range(currentRow, 1, currentRow, 2).Merge();

        currentRow++;
        worksheet.Cell(currentRow, 1).Value = $"Del {startDate:dd/MM/yyyy} al {endDate:dd/MM/yyyy}";
        if (includeUnposted) worksheet.Cell(currentRow, 2).Value = "(Provisional)";

        currentRow += 2;
        worksheet.Cell(currentRow, 1).Value = "INGRESOS";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        currentRow++;
        foreach (var line in report.Revenues)
        {
            worksheet.Cell(currentRow, 1).Value = $"{line.AccountCode} - {line.AccountName}";
            worksheet.Cell(currentRow, 2).Value = line.Amount;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
            currentRow++;
        }
        worksheet.Cell(currentRow, 1).Value = "Total Ingresos";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Value = report.TotalRevenues;
        worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Cell(currentRow, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;

        currentRow += 2;
        worksheet.Cell(currentRow, 1).Value = "COSTOS";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        currentRow++;
        foreach (var line in report.Costs)
        {
            worksheet.Cell(currentRow, 1).Value = $"{line.AccountCode} - {line.AccountName}";
            worksheet.Cell(currentRow, 2).Value = line.Amount;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
            currentRow++;
        }
        worksheet.Cell(currentRow, 1).Value = "Total Costos";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Value = report.TotalCosts;
        worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Cell(currentRow, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;

        currentRow += 2;
        worksheet.Cell(currentRow, 1).Value = "Utilidad Bruta";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Value = report.GrossProfit;
        worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";

        currentRow += 2;
        worksheet.Cell(currentRow, 1).Value = "GASTOS";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        currentRow++;
        foreach (var line in report.Expenses)
        {
            worksheet.Cell(currentRow, 1).Value = $"{line.AccountCode} - {line.AccountName}";
            worksheet.Cell(currentRow, 2).Value = line.Amount;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
            currentRow++;
        }
        worksheet.Cell(currentRow, 1).Value = "Total Gastos";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Value = report.TotalExpenses;
        worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Cell(currentRow, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;

        currentRow += 2;
        worksheet.Cell(currentRow, 1).Value = "UTILIDAD NETA";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 12;
        worksheet.Cell(currentRow, 2).Value = report.NetIncome;
        worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Style.Font.FontSize = 12;
        worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Cell(currentRow, 2).Style.Border.TopBorder = XLBorderStyleValues.Double;

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> GenerateIncomeStatementPdfAsync(DateTime startDate, DateTime endDate, bool includeUnposted = false)
    {
        var report = await GetIncomeStatementAsync(startDate, endDate, includeUnposted);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("ESTADO DE RESULTADOS").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                        col.Item().Text($"Del {startDate:dd/MM/yyyy} al {endDate:dd/MM/yyyy}");
                        if (includeUnposted) col.Item().Text("PROVISIONAL").FontColor(Colors.Red.Medium);
                    });
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn();
                        });

                        // Ingresos
                        table.Cell().Element(BlockHeader).Text("INGRESOS");
                        table.Cell().Element(BlockHeader).AlignRight().Text("Monto");

                        foreach (var line in report.Revenues)
                        {
                            table.Cell().Element(TableCell).Text($"{line.AccountCode} - {line.AccountName}");
                            table.Cell().Element(TableCell).AlignRight().Text(line.Amount.ToString("N2"));
                        }

                        table.Cell().Element(TableFooter).Text("Total Ingresos");
                        table.Cell().Element(TableFooter).AlignRight().Text(report.TotalRevenues.ToString("N2"));

                        // Costos
                        table.Cell().Element(BlockHeader).Text("COSTOS");
                        table.Cell().Element(BlockHeader).AlignRight().Text("");

                        foreach (var line in report.Costs)
                        {
                            table.Cell().Element(TableCell).Text($"{line.AccountCode} - {line.AccountName}");
                            table.Cell().Element(TableCell).AlignRight().Text(line.Amount.ToString("N2"));
                        }

                        table.Cell().Element(TableFooter).Text("Total Costos");
                        table.Cell().Element(TableFooter).AlignRight().Text(report.TotalCosts.ToString("N2"));

                        // Utilidad Bruta
                        table.Cell().Element(NetSection).Text("UTILIDAD BRUTA");
                        table.Cell().Element(NetSection).AlignRight().Text(report.GrossProfit.ToString("N2"));

                        // Gastos
                        table.Cell().Element(BlockHeader).Text("GASTOS");
                        table.Cell().Element(BlockHeader).AlignRight().Text("");

                        foreach (var line in report.Expenses)
                        {
                            table.Cell().Element(TableCell).Text($"{line.AccountCode} - {line.AccountName}");
                            table.Cell().Element(TableCell).AlignRight().Text(line.Amount.ToString("N2"));
                        }

                        table.Cell().Element(TableFooter).Text("Total Gastos");
                        table.Cell().Element(TableFooter).AlignRight().Text(report.TotalExpenses.ToString("N2"));

                        // Utilidad Neta
                        table.Cell().Element(FinalNetSection).Text("UTILIDAD NETA");
                        table.Cell().Element(FinalNetSection).AlignRight().Text(report.NetIncome.ToString("N2"));
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                });
            });
        }).GeneratePdf();
    }

    public async Task<BalanceSheetDto> GetBalanceSheetAsync(DateTime date, bool includeUnposted = false)
    {
        var entries = await journalEntryRepository.GetByDateRangeWithLinesAsync(DateTime.MinValue, date, includeUnposted);
        var allLines = entries.SelectMany(e => e.Lines).ToList();

        var report = new BalanceSheetDto
        {
            Date = date,
            IsProvisional = includeUnposted
        };

        // Assets
        var assetLines = allLines
            .Where(l => l.Account.Type == AccountType.Asset)
            .GroupBy(l => new { l.AccountId, l.Account.Code, l.Account.Name })
            .Select(g => new BalanceSheetLineDto
            {
                AccountId = g.Key.AccountId,
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                Amount = g.Sum(l => l.Debit - l.Credit)
            })
            .Where(l => l.Amount != 0)
            .OrderBy(l => l.AccountCode)
            .ToList();

        report.Assets = assetLines;
        report.TotalAssets = assetLines.Sum(a => a.Amount);

        // Liabilities
        var liabilityLines = allLines
            .Where(l => l.Account.Type == AccountType.Liability)
            .GroupBy(l => new { l.AccountId, l.Account.Code, l.Account.Name })
            .Select(g => new BalanceSheetLineDto
            {
                AccountId = g.Key.AccountId,
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                Amount = g.Sum(l => l.Credit - l.Debit)
            })
            .Where(l => l.Amount != 0)
            .OrderBy(l => l.AccountCode)
            .ToList();

        report.Liabilities = liabilityLines;
        report.TotalLiabilities = liabilityLines.Sum(l => l.Amount);

        // Equity
        var equityLines = allLines
            .Where(l => l.Account.Type == AccountType.Equity)
            .GroupBy(l => new { l.AccountId, l.Account.Code, l.Account.Name })
            .Select(g => new BalanceSheetLineDto
            {
                AccountId = g.Key.AccountId,
                AccountCode = g.Key.Code,
                AccountName = g.Key.Name,
                Amount = g.Sum(l => l.Credit - l.Debit)
            })
            .Where(l => l.Amount != 0)
            .OrderBy(l => l.AccountCode)
            .ToList();

        report.Equity = equityLines;
        report.TotalEquity = equityLines.Sum(e => e.Amount);

        // Net Income (Historical)
        var revenue = allLines.Where(l => l.Account.Type == AccountType.Revenue).Sum(l => l.Credit - l.Debit);
        var costs = allLines.Where(l => l.Account.Type == AccountType.Cost).Sum(l => l.Debit - l.Credit);
        var expenses = allLines.Where(l => l.Account.Type == AccountType.Expense).Sum(l => l.Debit - l.Credit);
        report.NetIncome = revenue - costs - expenses;

        await transactionLogService.LogTransactionAsync(null, TransactionActions.GenerateBalanceSheet, nameof(BalanceSheetDto), null, TransactionStatus.Success, JsonSerializer.Serialize(new { date, includeUnposted }));

        return report;
    }

    public async Task<byte[]> GenerateBalanceSheetExcelAsync(DateTime date, bool includeUnposted = false)
    {
        var report = await GetBalanceSheetAsync(date, includeUnposted);
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Balance General");

        var currentRow = 1;
        worksheet.Cell(currentRow, 1).Value = "BALANCE GENERAL";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 1).Style.Font.FontSize = 14;
        worksheet.Range(currentRow, 1, currentRow, 2).Merge();

        currentRow++;
        worksheet.Cell(currentRow, 1).Value = $"Al {date:dd/MM/yyyy}";
        if (includeUnposted) worksheet.Cell(currentRow, 2).Value = "(Provisional)";

        currentRow += 2;
        worksheet.Cell(currentRow, 1).Value = "ACTIVOS";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        currentRow++;
        foreach (var line in report.Assets)
        {
            worksheet.Cell(currentRow, 1).Value = $"{line.AccountCode} - {line.AccountName}";
            worksheet.Cell(currentRow, 2).Value = line.Amount;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
            currentRow++;
        }
        worksheet.Cell(currentRow, 1).Value = "Total Activos";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Value = report.TotalAssets;
        worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Cell(currentRow, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;

        currentRow += 2;
        worksheet.Cell(currentRow, 1).Value = "PASIVOS";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        currentRow++;
        foreach (var line in report.Liabilities)
        {
            worksheet.Cell(currentRow, 1).Value = $"{line.AccountCode} - {line.AccountName}";
            worksheet.Cell(currentRow, 2).Value = line.Amount;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
            currentRow++;
        }
        worksheet.Cell(currentRow, 1).Value = "Total Pasivos";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Value = report.TotalLiabilities;
        worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Cell(currentRow, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;

        currentRow += 2;
        worksheet.Cell(currentRow, 1).Value = "PATRIMONIO";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        currentRow++;
        foreach (var line in report.Equity)
        {
            worksheet.Cell(currentRow, 1).Value = $"{line.AccountCode} - {line.AccountName}";
            worksheet.Cell(currentRow, 2).Value = line.Amount;
            worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
            currentRow++;
        }
        worksheet.Cell(currentRow, 1).Value = "Utilidad del Periodo (Histórica)";
        worksheet.Cell(currentRow, 2).Value = report.NetIncome;
        worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
        currentRow++;

        worksheet.Cell(currentRow, 1).Value = "Total Patrimonio";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Value = report.TotalEquity + report.NetIncome;
        worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Cell(currentRow, 2).Style.Border.TopBorder = XLBorderStyleValues.Thin;

        currentRow += 2;
        worksheet.Cell(currentRow, 1).Value = "TOTAL PASIVO Y PATRIMONIO";
        worksheet.Cell(currentRow, 1).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Value = report.TotalLiabilitiesAndEquity;
        worksheet.Cell(currentRow, 2).Style.Font.Bold = true;
        worksheet.Cell(currentRow, 2).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Cell(currentRow, 2).Style.Border.TopBorder = XLBorderStyleValues.Double;

        worksheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> GenerateBalanceSheetPdfAsync(DateTime date, bool includeUnposted = false)
    {
        var report = await GetBalanceSheetAsync(date, includeUnposted);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("BALANCE GENERAL").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                        col.Item().Text($"Al {date:dd/MM/yyyy}");
                        if (includeUnposted) col.Item().Text("PROVISIONAL").FontColor(Colors.Red.Medium);
                    });
                });

                page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
                {
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn();
                        });

                        // Activos
                        table.Cell().Element(BlockHeader).Text("ACTIVOS");
                        table.Cell().Element(BlockHeader).AlignRight().Text("Monto");

                        foreach (var line in report.Assets)
                        {
                            table.Cell().Element(TableCell).Text($"{line.AccountCode} - {line.AccountName}");
                            table.Cell().Element(TableCell).AlignRight().Text(line.Amount.ToString("N2"));
                        }

                        table.Cell().Element(TableFooter).Text("Total Activos");
                        table.Cell().Element(TableFooter).AlignRight().Text(report.TotalAssets.ToString("N2"));

                        // Pasivos
                        table.Cell().Element(BlockHeader).Text("PASIVOS");
                        table.Cell().Element(BlockHeader).AlignRight().Text("");

                        foreach (var line in report.Liabilities)
                        {
                            table.Cell().Element(TableCell).Text($"{line.AccountCode} - {line.AccountName}");
                            table.Cell().Element(TableCell).AlignRight().Text(line.Amount.ToString("N2"));
                        }

                        table.Cell().Element(TableFooter).Text("Total Pasivos");
                        table.Cell().Element(TableFooter).AlignRight().Text(report.TotalLiabilities.ToString("N2"));

                        // Patrimonio
                        table.Cell().Element(BlockHeader).Text("PATRIMONIO");
                        table.Cell().Element(BlockHeader).AlignRight().Text("");

                        foreach (var line in report.Equity)
                        {
                            table.Cell().Element(TableCell).Text($"{line.AccountCode} - {line.AccountName}");
                            table.Cell().Element(TableCell).AlignRight().Text(line.Amount.ToString("N2"));
                        }
                        table.Cell().Element(TableCell).Text("Utilidad del Periodo (Histórica)");
                        table.Cell().Element(TableCell).AlignRight().Text(report.NetIncome.ToString("N2"));

                        table.Cell().Element(TableFooter).Text("Total Patrimonio");
                        table.Cell().Element(TableFooter).AlignRight().Text((report.TotalEquity + report.NetIncome).ToString("N2"));

                        // Total Pasivo y Patrimonio
                        table.Cell().Element(FinalNetSection).Text("TOTAL PASIVO Y PATRIMONIO").FontSize(12).SemiBold();
                        table.Cell().Element(FinalNetSection).AlignRight().Text(report.TotalLiabilitiesAndEquity.ToString("N2")).FontSize(12).SemiBold();
                    });
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Página ");
                    x.CurrentPageNumber();
                });
            });
        }).GeneratePdf();
    }

    private static IContainer BlockHeader(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Black)
            .PaddingVertical(5)
            .DefaultTextStyle(x => x.SemiBold().FontSize(12));
    }

    private static IContainer TableCell(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(5);
    }

    private static IContainer TableFooter(IContainer container)
    {
        return container
            .PaddingVertical(5)
            .DefaultTextStyle(x => x.SemiBold());
    }

    private static IContainer NetSection(IContainer container)
    {
        return container
            .Background(Colors.Grey.Lighten4)
            .PaddingVertical(5)
            .PaddingHorizontal(5)
            .DefaultTextStyle(x => x.SemiBold());
    }

    private static IContainer FinalNetSection(IContainer container)
    {
        return container
            .Background(Colors.Blue.Lighten5)
            .PaddingVertical(10)
            .PaddingHorizontal(5);
    }
}
