namespace SBC.Domain.Entities.Logging;

public static class TransactionActions
{
    public const string CreateAccount = "CreateAccount";
    public const string UpdateAccount = "UpdateAccount";
    public const string DeleteAccount = "DeleteAccount";

    public const string CreateJournalEntry = "CreateJournalEntry";
    public const string UpdateJournalEntry = "UpdateJournalEntry";
    public const string DeleteJournalEntry = "DeleteJournalEntry";

    public const string CreateJournalEntryLine = "CreateJournalEntryLine";
    public const string UpdateJournalEntryLine = "UpdateJournalEntryLine";
    public const string DeleteJournalEntryLine = "DeleteJournalEntryLine";

    public const string CloseAccountingPeriod = "CloseAccountingPeriod";
    public const string CreateAccountingPeriod = "CreateAccountingPeriod";

    public const string BulkImport = "BulkImport";

    public const string RegisterUser = "RegisterUser";
    public const string Login = "Login";
    public const string RefreshToken = "RefreshToken";

    public const string GenerateIncomeStatement = "GenerateIncomeStatement";
    public const string GenerateBalanceSheet = "GenerateBalanceSheet";
}
