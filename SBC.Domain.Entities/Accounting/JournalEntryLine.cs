using SBC.Domain.Entities.Base;

namespace SBC.Domain.Entities.Accounting;

/// <summary>
/// Represents an individual line item within a journal entry, used to record the debit
/// and credit amounts associated with a specific financial account.
/// Each line is associated with a journal entry and an account, thereby providing
/// detailed tracking of financial transactions at the account level.
/// </summary>
public class JournalEntryLine : BaseEntity
{
    /// <summary>
    /// Gets or sets the unique identifier of the journal entry associated with this line item.
    /// This property establishes the relationship between the journal entry and its line items,
    /// enabling the grouping of financial transactions under a single journal entry.
    /// </summary>
    public Guid JournalEntryId { get; set; }

    /// <summary>
    /// Gets or sets the journal entry associated with this line item.
    /// This property provides the linkage between the line item and the journal entry,
    /// facilitating the organization and management of financial transactions.
    /// </summary>
    public JournalEntry JournalEntry { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the financial account associated with this journal entry line.
    /// This property establishes the link between the line item and its corresponding account,
    /// enabling precise categorization and tracking of financial transactions.
    /// </summary>
    public Guid AccountId { get; set; }

    /// <summary>
    /// Gets or sets the financial account associated with this journal entry line.
    /// This property establishes the relationship between the line item and the account,
    /// enabling the accurate categorization and tracking of debit and credit transactions
    /// within the accounting system.
    /// </summary>
    public Account Account { get; set; }

    /// <summary>
    /// Gets or sets the debit amount for the financial transaction recorded in this journal entry line.
    /// This property represents the value being added to the specified account as a debit entry,
    /// contributing to the double-entry accounting system validation.
    /// </summary>
    public decimal Debit { get; set; }

    /// <summary>
    /// Gets or sets the credit amount associated with this journal entry line item.
    /// This property reflects the value to be recorded as a credit in the corresponding financial account
    /// and is used to balance transactions within the double-entry accounting system.
    /// </summary>
    public decimal Credit { get; set; }
}