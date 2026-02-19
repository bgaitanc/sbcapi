using SBC.Domain.Entities.Base;
using SBC.Domain.Entities.Enums;

namespace SBC.Domain.Entities.Accounting;

// Catalogo contables
//1  Activo                   - 1 padreId null
//2       Activos corrientes - 1.1.0000.0000 padreID 1
//3               Caja       - 1.100.0000.1000 padreId 2

//4 Pasivo                   - 2.000.0000.0000 padreId null
//5       Pasivos corrientes - 2.100.0000.0000 padreId 4
//6               C x pagar  - 2.100.0000.1000 padreId 5

// Asientos contables Y teoría de partida doble
// Estados financieros

/// <summary>
/// Represents a financial account used to categorize transactions in the accounting system.
/// </summary>
public class Account : BaseEntity // Cuentas - Catalogo de cuentas
{
    /// <summary>
    /// Gets or sets the unique identifier for the account.
    /// This code is used for classification or reference purposes within the accounting system.
    /// </summary>
    public string Code { get; set; } // Código - tiene que ser unico

    /// <summary>
    /// Gets or sets the name of the account.
    /// This is a human-readable label used to identify the account within the accounting system.
    /// </summary>
    public string Name { get; set; } // Nombre 

    /// <summary>
    /// Gets or sets the type of the account, indicating its classification within the accounting system.
    /// This value determines whether the account is categorized as Asset, Liability, Equity, Revenue, or Expense.
    /// </summary>
    public AccountType Type { get; set; } // Tipo de cuenta

    /// <summary>
    /// Gets or sets the identifier of the parent account associated with this account.
    /// This property is used to establish hierarchical relationships between accounts.
    /// </summary>
    public Guid? ParentAccountId { get; set; } // Id de la cuenta padre

    /// <summary>
    /// Gets or sets the parent account associated with this account.
    /// This property defines the hierarchical relationship between accounts,
    /// allowing for the organization of accounts into a parent-child structure.
    /// </summary>
    public Account ParentAccount { get; set; } // Cuenta padre - navegación

    /// <summary>
    /// Gets or sets the collection of sub-accounts associated with the current account.
    /// Sub-accounts allow for hierarchical structuring of financial accounts,
    /// enabling more detailed categorization and reporting within the accounting system.
    /// </summary>
    public ICollection<Account> SubAccounts { get; set; } = new List<Account>(); // Subcuentas
}