using SBC.Application.Models.Accounting;
using SBC.Application.Services.Interfaces;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Entities.Enums;
using SBC.Domain.Entities.Logging;
using SBC.Domain.Exceptions;
using SBC.Domain.Repositories.Interfaces;
using System.Text.Json;

namespace SBC.Application.Services.Implementation;

public class AccountingPeriodService(
    IAccountingPeriodRepository periodRepository,
    IJournalEntryRepository journalEntryRepository,
    IFinancialReportService financialReportService,
    IAccountRepository accountRepository,
    ITransactionLogService transactionLogService) : IAccountingPeriodService
{
    public async Task<AccountingPeriodDto> ClosePeriodAsync(int year, int month, Guid equityAccountId)
    {
        // 1. Validar si el periodo ya está cerrado
        var period = await periodRepository.GetByPeriodAsync(year, month);
        if (period != null && period.IsClosed)
        {
            var error = $"El período {month}/{year} ya se encuentra cerrado.";
            await transactionLogService.LogTransactionAsync(null, TransactionActions.CloseAccountingPeriod, nameof(AccountingPeriod), null, TransactionStatus.ValidationError, JsonSerializer.Serialize(new { year, month, equityAccountId }), error);
            throw new DomainException(error);
        }

        // 2. Verificar que la cuenta de patrimonio exista
        var equityAccount = await accountRepository.GetByIdAsync(equityAccountId);
        if (equityAccount == null)
        {
            var error = $"Account with id {equityAccountId} was not found.";
            await transactionLogService.LogTransactionAsync(null, TransactionActions.CloseAccountingPeriod, nameof(AccountingPeriod), null, TransactionStatus.Failure, JsonSerializer.Serialize(new { year, month, equityAccountId }), error);
            throw new NotFoundException(nameof(Account), equityAccountId);
        }

        // 3. Obtener todos los asientos del periodo para mayorizarlos
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1).AddDays(-1);
        var entries = await journalEntryRepository.GetByDateRangeWithLinesAsync(startDate, endDate, true);
        
        // Mayorización (marcar como IsPosted)
        foreach (var entry in entries.Where(e => !e.IsPosted))
        {
            entry.IsPosted = true;
            await journalEntryRepository.UpdateAsync(entry);
        }

        // 4. Obtener Estado de Resultados para generar el asiento de cierre
        var incomeStatement = await financialReportService.GetIncomeStatementAsync(startDate, endDate);
        
        if (incomeStatement.NetIncome == 0 && !incomeStatement.Revenues.Any() && !incomeStatement.Costs.Any() && !incomeStatement.Expenses.Any())
        {
            // No hay nada que cerrar, pero marcamos el periodo como cerrado
            var result = await MarkAsClosed(period, year, month, null);
            await transactionLogService.LogTransactionAsync(null, TransactionActions.CloseAccountingPeriod, nameof(AccountingPeriod), result.Id.ToString(), TransactionStatus.Success, JsonSerializer.Serialize(new { year, month, equityAccountId }));
            return result;
        }

        // 5. Generar Asiento de Cierre
        var closingEntry = new JournalEntry
        {
            Id = Guid.NewGuid(),
            Date = endDate,
            Day = endDate.Day,
            Month = endDate.Month,
            Year = endDate.Year,
            Description = $"Asiento de Cierre Contable - Periodo {month}/{year}",
            IsPosted = true // El asiento de cierre nace mayorizado
        };

        // Cerrar Ingresos (Debitar)
        foreach (var rev in incomeStatement.Revenues)
        {
            closingEntry.Lines.Add(new JournalEntryLine
            {
                Id = Guid.NewGuid(),
                AccountId = rev.AccountId,
                Debit = rev.Amount, // Los ingresos tienen saldo acreedor, para cerrar debitamos
                Credit = 0
            });
        }

        // Cerrar Costos (Acreditar)
        foreach (var cost in incomeStatement.Costs)
        {
            closingEntry.Lines.Add(new JournalEntryLine
            {
                Id = Guid.NewGuid(),
                AccountId = cost.AccountId,
                Debit = 0,
                Credit = cost.Amount // Los costos tienen saldo deudor, para cerrar acreditamos
            });
        }

        // Cerrar Gastos (Acreditar)
        foreach (var exp in incomeStatement.Expenses)
        {
            closingEntry.Lines.Add(new JournalEntryLine
            {
                Id = Guid.NewGuid(),
                AccountId = exp.AccountId,
                Debit = 0,
                Credit = exp.Amount // Los gastos tienen saldo deudor, para cerrar acreditamos
            });
        }

        // Resultado del ejercicio (Diferencia a cuenta de patrimonio)
        if (incomeStatement.NetIncome > 0)
        {
            // Utilidad -> Acreditar Patrimonio
            closingEntry.Lines.Add(new JournalEntryLine
            {
                Id = Guid.NewGuid(),
                AccountId = equityAccountId,
                Debit = 0,
                Credit = incomeStatement.NetIncome
            });
        }
        else if (incomeStatement.NetIncome < 0)
        {
            // Pérdida -> Debitar Patrimonio
            closingEntry.Lines.Add(new JournalEntryLine
            {
                Id = Guid.NewGuid(),
                AccountId = equityAccountId,
                Debit = Math.Abs(incomeStatement.NetIncome),
                Credit = 0
            });
        }

        if (!closingEntry.ValidateDoubleEntry())
        {
            var error = "Error al generar el asiento de cierre: No cumple con la partida doble.";
            await transactionLogService.LogTransactionAsync(null, TransactionActions.CloseAccountingPeriod, nameof(AccountingPeriod), null, TransactionStatus.ValidationError, JsonSerializer.Serialize(new { year, month, equityAccountId }), error);
            throw new DomainException(error);
        }

        closingEntry.Code = await GenerateClosingCodeAsync(year, month);
        var createdEntry = await journalEntryRepository.AddAsync(closingEntry);

        // 6. Marcar periodo como cerrado
        var closedPeriod = await MarkAsClosed(period, year, month, createdEntry.Id);
        await transactionLogService.LogTransactionAsync(null, TransactionActions.CloseAccountingPeriod, nameof(AccountingPeriod), closedPeriod.Id.ToString(), TransactionStatus.Success, JsonSerializer.Serialize(new { year, month, equityAccountId }));
        return closedPeriod;
    }

    public async Task<IEnumerable<AccountingPeriodDto>> GetAllPeriodsAsync()
    {
        var periods = await periodRepository.GetAllAsync();
        return periods.Select(MapToDto);
    }

    public async Task<AccountingPeriodDto?> GetByPeriodAsync(int year, int month)
    {
        var period = await periodRepository.GetByPeriodAsync(year, month);
        return period == null ? null : MapToDto(period);
    }

    public async Task<AccountingPeriodDto> CreatePeriodAsync(int year, int month)
    {
        var existingPeriod = await periodRepository.GetByPeriodAsync(year, month);
        if (existingPeriod != null)
        {
            var error = $"El período {month}/{year} ya existe.";
            await transactionLogService.LogTransactionAsync(null, TransactionActions.CreateAccountingPeriod, nameof(AccountingPeriod), null, TransactionStatus.ValidationError, JsonSerializer.Serialize(new { year, month }), error);
            throw new DomainException(error);
        }

        var period = new AccountingPeriod
        {
            Id = Guid.NewGuid(),
            Year = year,
            Month = month,
            IsClosed = false
        };

        var createdPeriod = await periodRepository.AddAsync(period);
        await transactionLogService.LogTransactionAsync(null, TransactionActions.CreateAccountingPeriod, nameof(AccountingPeriod), createdPeriod.Id.ToString(), TransactionStatus.Success, JsonSerializer.Serialize(new { year, month }));
        return MapToDto(createdPeriod);
    }

    private async Task<AccountingPeriodDto> MarkAsClosed(AccountingPeriod? period, int year, int month, Guid? closingEntryId)
    {
        if (period == null)
        {
            period = new AccountingPeriod
            {
                Id = Guid.NewGuid(),
                Year = year,
                Month = month,
                IsClosed = true,
                ClosedAt = DateTime.Now,
                ClosingJournalEntryId = closingEntryId
            };
            await periodRepository.AddAsync(period);
        }
        else
        {
            period.IsClosed = true;
            period.ClosedAt = DateTime.Now;
            period.ClosingJournalEntryId = closingEntryId;
            await periodRepository.UpdateAsync(period);
        }

        return MapToDto(period);
    }

    private async Task<string> GenerateClosingCodeAsync(int year, int month)
    {
        return $"CIE-{year}-{month:D2}";
    }

    private static AccountingPeriodDto MapToDto(AccountingPeriod period)
    {
        return new AccountingPeriodDto
        {
            Id = period.Id,
            Year = period.Year,
            Month = period.Month,
            IsClosed = period.IsClosed,
            ClosedAt = period.ClosedAt,
            ClosedBy = period.ClosedBy,
            ClosingJournalEntryId = period.ClosingJournalEntryId
        };
    }
}
