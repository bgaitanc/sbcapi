using SBC.Domain.Entities.Base;

namespace SBC.Domain.Entities.Accounting;

/// <summary>
/// Represents a journal entry in an accounting system, which is used to record financial transactions.
/// A journal entry contains metadata such as the transaction date, description,
/// and a collection of individual journal entry lines representing the debit and credit movements.
/// </summary>
public class JournalEntry : BaseEntity // Asiento
{
    public string Code { get; set; } // Código del asiento

    /// <summary>
    /// Gets or sets the date of the journal entry.
    /// This date represents the specific timestamp when the financial transaction took place.
    /// It is used for organizing and associating journal entries with their respective periods.
    /// </summary>
    public DateTime Date { get; set; } // fecha

    /// <summary>
    /// Gets or sets the day component of the journal entry's date.
    /// This property represents the specific day of the month when the financial transaction occurred,
    /// aiding in the precise categorization of journal entries within a specific period.
    /// </summary>
    public int Day { get; set; } // Día

    /// <summary>
    /// Gets or sets the month associated with the journal entry.
    /// This value is used to identify and organize financial transactions
    /// within a specific month of the accounting period.
    /// </summary>
    public int Month { get; set; } // Mes

    /// <summary>
    /// Gets or sets the year associated with the journal entry.
    /// This property indicates the fiscal or calendar year to which
    /// the financial transaction belongs, helping to categorize and align
    /// entries with reporting periods.
    /// </summary>
    public int Year { get; set; } // Año

    /// <summary>
    /// Gets or sets the description of the journal entry.
    /// This description provides a summary or narrative for the financial transaction,
    /// offering context and aiding in understanding the purpose of the entry.
    /// </summary>
    public string Description { get; set; } // Descripción

    /// <summary>
    /// Gets or sets a value indicating whether the journal entry has been posted.
    /// When a journal entry is posted, it indicates that the financial transaction
    /// has been finalized and recorded in the accounting system, rendering it immutable.
    /// </summary>
    public bool IsPosted { get; set; } // Si está mayorizado no se puede editar

    /// <summary>
    /// Gets or sets the collection of journal entry lines associated with this journal entry.
    /// Each line represents an individual debit or credit transaction, specifying the account,
    /// amount, and direction of the movement. It is used for ensuring the balance and accuracy
    /// of financial transactions.
    /// </summary>
    public ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();

    /// <summary>
    /// Validates that the journal entry complies with the double-entry bookkeeping principle,
    /// ensuring that the total debits equal the total credits for all associated lines.
    /// </summary>
    /// <returns>
    /// A boolean value indicating whether the journal entry is balanced (true if total debits equal total credits, otherwise false).
    /// </returns>
    public bool ValidateDoubleEntry() // Validación de partida doble en el dominio
    {
        var totalDebit = Lines.Sum(x => x.Debit);
        var totalCredit = Lines.Sum(x => x.Credit);
        return totalDebit == totalCredit;
    }
}