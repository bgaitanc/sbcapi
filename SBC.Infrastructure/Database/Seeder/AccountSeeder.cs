using Microsoft.EntityFrameworkCore;
using SBC.Domain.Entities.Accounting;
using SBC.Domain.Entities.Enums;

namespace SBC.Infrastructure.Database.Seeder;

public static class AccountSeeder
{
    public static void SeedAccounts(ModelBuilder modelBuilder)
    {
        var accounts = new List<Account>
        {
            // ACTIVO (1)
            CreateAccount("1", "ACTIVO", AccountType.Asset, null),
            CreateAccount("1.1", "ACTIVOS CIRCULANTES", AccountType.Asset, "1"),
            CreateAccount("1.1.1", "ACTIVOS DISPONIBLES", AccountType.Asset, "1.1"),
            CreateAccount("1.1.1.1", "CAJA", AccountType.Asset, "1.1.1"),
            CreateAccount("1.1.1.1.001", "CAJA GENERAL", AccountType.Asset, "1.1.1.1"),
            CreateAccount("1.1.1.1.002", "CAJA CHICA", AccountType.Asset, "1.1.1.1"),
            CreateAccount("1.1.1.2", "BANCOS", AccountType.Asset, "1.1.1"),
            CreateAccount("1.1.1.2.001", "EMPRESA X CTA.CTE.#10012705215805", AccountType.Asset, "1.1.1.2"),
            CreateAccount("1.1.2", "ACTIVOS EXIGIBLES", AccountType.Asset, "1.1"),
            CreateAccount("1.1.2.1", "CUENTAS POR COBRAR", AccountType.Asset, "1.1.2"),
            CreateAccount("1.1.2.1.001", "CUENTAS POR COBRAR CLIENTES", AccountType.Asset, "1.1.2.1"),
            CreateAccount("1.1.2.1.002", "CUENTAS POR COBRAR EMPLEADOS", AccountType.Asset, "1.1.2.1"),
            CreateAccount("1.1.2.1.003", "CUENTAS POR COBRAR OTROS", AccountType.Asset, "1.1.2.1"),
            CreateAccount("1.1.2.1.004", "ANTICIPO A JUSTIFICAR", AccountType.Asset, "1.1.2.1"),
            CreateAccount("1.1.3", "ACTIVOS REALIZABLES", AccountType.Asset, "1.1"),
            CreateAccount("1.1.3.1", "INVENTARIO DE MATERIALES", AccountType.Asset, "1.1.3"),
            CreateAccount("1.1.3.1.001", "INV. DE ACCESORIOS", AccountType.Asset, "1.1.3.1"),
            CreateAccount("1.1.3.1.002", "INV. DE PAPERIA Y UTILES DE OFICINA", AccountType.Asset, "1.1.3.1"),
            CreateAccount("1.1.3.1.003", "INV. DE MATERIALES Y LIMPIEZA", AccountType.Asset, "1.1.3.1"),
            CreateAccount("1.1.3.2", "INVENTARIO DE MERCADERIAS", AccountType.Asset, "1.1.3"),
            CreateAccount("1.1.3.2.001", "GRANOS BASICOS", AccountType.Asset, "1.1.3.2"),
            CreateAccount("1.1.4", "OTROS ACTIVOS CIRCULANTES", AccountType.Asset, "1.1"),
            CreateAccount("1.1.4.1", "IMPUESTOS ANTICIPADOS Y RETENIDOS", AccountType.Asset, "1.1.4"),
            CreateAccount("1.1.4.1.001", "IVA CREDITO TRIBUNAL", AccountType.Asset, "1.1.4.1"),
            CreateAccount("1.1.4.1.002", "ANTICIPO IMPUESTO A LA RENTA", AccountType.Asset, "1.1.4.1"),
            CreateAccount("1.1.4.1.003", "ANTICIPO RETENCION EN LA FUENTE", AccountType.Asset, "1.1.4.1"),
            CreateAccount("1.2", "ACTIVOS FIJOS", AccountType.Asset, "1"),
            CreateAccount("1.2.1", "ACTIVOS TANGIBLES", AccountType.Asset, "1.2"),
            CreateAccount("1.2.1.1", "ACTIVOS DEPRECIABLES", AccountType.Asset, "1.2.1"),
            CreateAccount("1.2.1.1.001", "INMUEBLES", AccountType.Asset, "1.2.1.1"),
            CreateAccount("1.2.1.1.002", "INSTALACIONES", AccountType.Asset, "1.2.1.1"),
            CreateAccount("1.2.1.2", "ACTIVOS NO DEPRECIABLES", AccountType.Asset, "1.2.1"),
            CreateAccount("1.2.1.2.001", "TERRENO # 1", AccountType.Asset, "1.2.1.2"),
            CreateAccount("1.2.1.3", "DEPRECIACION ACUMULDA", AccountType.Asset, "1.2.1"),
            CreateAccount("1.2.1.3.001", "INMUEBLES DEP", AccountType.Asset, "1.2.1.3"),
            CreateAccount("1.2.1.3.002", "INSTALACIONES DEP", AccountType.Asset, "1.2.1.3"),
            CreateAccount("1.2.2", "ACTIVOS INTANGIBLES", AccountType.Asset, "1.2"),
            CreateAccount("1.2.2.1", "MARCAS", AccountType.Asset, "1.2.2"),
            CreateAccount("1.2.2.1.001", "SERVICIO DE EMPRESA", AccountType.Asset, "1.2.2.1"),
            CreateAccount("1.2.2.1.002", "MARCA DE PRODUCTO", AccountType.Asset, "1.2.2.1"),
            CreateAccount("1.2.2.2", "SOFTWARE", AccountType.Asset, "1.2.2"),
            CreateAccount("1.2.2.2.001", "SOFTWARE CONTABLE", AccountType.Asset, "1.2.2.2"),
            CreateAccount("1.3", "OTROS ACTIVOS", AccountType.Asset, "1"),
            CreateAccount("1.3.1", "ACTIVOS DIFERIDOS", AccountType.Asset, "1.3"),
            CreateAccount("1.3.1.1", "GASTOS DE CONSTITUCION.", AccountType.Asset, "1.3.1"),
            CreateAccount("1.3.1.1.001", "GASTOS DE CONSTITUCION", AccountType.Asset, "1.3.1.1"),
            CreateAccount("1.3.1.2", "GASTOS DE CONSTRUCCION.", AccountType.Asset, "1.3.1"),
            CreateAccount("1.3.2", "OTROS ACTIVOS VARIOS", AccountType.Asset, "1.3"),
            CreateAccount("1.3.2.1", "DEPOSITOD ENTREGADO EN GARANTIA", AccountType.Asset, "1.3.2"),
            CreateAccount("1.3.2.1.001", "INMUEBLES ENTREGADOS EN GARANTIA", AccountType.Asset, "1.3.2.1"),

            // PASIVO (2)
            CreateAccount("2", "PASIVO", AccountType.Liability, null),
            CreateAccount("2.1", "PASIVOS A CORTO PLAZO", AccountType.Liability, "2"),
            CreateAccount("2.1.1", "CUENTAS POR PAGAR", AccountType.Liability, "2.1"),
            CreateAccount("2.1.1.1", "PROVEEDORES NACIONALES", AccountType.Liability, "2.1.1"),
            CreateAccount("2.1.1.1.001", "INDUSTRIALES CXP", AccountType.Liability, "2.1.1.1"),
            CreateAccount("2.1.1.2", "PROVEEDORES LOCALES", AccountType.Liability, "2.1.1"),
            CreateAccount("2.1.1.2.001", "PROVEEDOR X CXP", AccountType.Liability, "2.1.1.2"),
            CreateAccount("2.1.1.3", "SERVICIOS POR PAGAR", AccountType.Liability, "2.1.1"),
            CreateAccount("2.1.1.3.001", "SERV. ENERGIA CXP", AccountType.Liability, "2.1.1.3"),
            CreateAccount("2.1.1.3.002", "TELEFONO E INTERNET CXP", AccountType.Liability, "2.1.1.3"),
            CreateAccount("2.1.1.4", "OTRAS CUENTAS POR PAGAR", AccountType.Liability, "2.1.1"),
            CreateAccount("2.1.1.4.001", "CUENTAS VARIAS POR PAGAR", AccountType.Liability, "2.1.1.4"),
            CreateAccount("2.1.2", "NOMINA POR PAGAR", AccountType.Liability, "2.1"),
            CreateAccount("2.1.2.1", "REMUNERACIONES POR PAGAR", AccountType.Liability, "2.1.2"),
            CreateAccount("2.1.2.1.001", "SUELDOS POR PAGAR", AccountType.Liability, "2.1.2.1"),
            CreateAccount("2.1.2.1.002", "DESCUENTOS POR ATRASOS", AccountType.Liability, "2.1.2.1"),
            CreateAccount("2.1.2.1.003", "LIQUIDACIONES POR PAGAR", AccountType.Liability, "2.1.2.1"),
            CreateAccount("2.1.2.2", "INSS E INATEC POR PAGAR", AccountType.Liability, "2.1.2"),
            CreateAccount("2.1.2.2.001", "APORTES PERSONAL Y PATRONAL CXP", AccountType.Liability, "2.1.2.2"),
            CreateAccount("2.1.2.2.002", "INATEC POR PAGAR", AccountType.Liability, "2.1.2.2"),
            CreateAccount("2.1.2.3", "PROVICIONES POR PAGAR", AccountType.Liability, "2.1.2"),
            CreateAccount("2.1.2.3.001", "DECIMO TERCER MES POR PAGAR", AccountType.Liability, "2.1.2.3"),
            CreateAccount("2.1.2.3.002", "VACACIONES POR PAGAR", AccountType.Liability, "2.1.2.3"),
            CreateAccount("2.1.2.3.003", "INDEMNIZACIONES POR PAGAR", AccountType.Liability, "2.1.2.3"),
            CreateAccount("2.1.3", "OBLIGACIONES FISCALES POR PAGAR", AccountType.Liability, "2.1"),
            CreateAccount("2.1.3.1", "IMPUESTOS POR PAGAR", AccountType.Liability, "2.1.3"),
            CreateAccount("2.1.3.1.001", "IMPUESTO A LA RENTA POR PAGAR", AccountType.Liability, "2.1.3.1"),
            CreateAccount("2.1.3.1.002", "IVA POR PAGAR (15%)", AccountType.Liability, "2.1.3.1"),
            CreateAccount("2.1.3.2", "RETENCIONES FUENTE POR PAGAR", AccountType.Liability, "2.1.3"),
            CreateAccount("2.1.3.2.001", "2% RET. FTE DE COMPRAS", AccountType.Liability, "2.1.3.2"),
            CreateAccount("2.1.3.2.002", "5% RET.FTE. SERVICIO", AccountType.Liability, "2.1.3.2"),
            CreateAccount("2.1.3.2.003", "2% RET.FTE. SERVICIOS", AccountType.Liability, "2.1.3.2"),
            CreateAccount("2.1.3.2.004", "10% RET.FTE.HONORARIOS PROFESIONALES", AccountType.Liability, "2.1.3.2"),
            CreateAccount("2.1.3.3", "RETENCIONES IVA POR PAGAR", AccountType.Liability, "2.1.3"),
            CreateAccount("2.1.3.3.001", "15% RET. IVA VENTAS", AccountType.Liability, "2.1.3.3"),
            CreateAccount("2.1.3.4", "OTROS IMPUESTOS POR PAGAR", AccountType.Liability, "2.1.3"),
            CreateAccount("2.1.3.5", "RETENCIONES SOBRE SALARIO", AccountType.Liability, "2.1.3"),
            CreateAccount("2.1.3.5.001", "RETENCIONES SOBRE SALARIO DGI XP", AccountType.Liability, "2.1.3.5"),
            CreateAccount("2.2", "PASIVOS A LARGO PLAZO", AccountType.Liability, "2"),
            CreateAccount("2.2.1", "PRESTAMOS A LARGO PLAZO", AccountType.Liability, "2.2"),
            CreateAccount("2.2.1.1", "PRESTMOS SOCIOS LARGO PLAZO", AccountType.Liability, "2.2.1"),
            CreateAccount("2.2.1.1.001", "R.G. PMO LARGO PLAZO", AccountType.Liability, "2.2.1.1"),

            // PATRIMONIO (3)
            CreateAccount("3", "PATRIMONIO", AccountType.Equity, null),
            CreateAccount("3.1", "PATRIMONIO NEGOCIO X", AccountType.Equity, "3"),
            CreateAccount("3.1.1", "CAPITAL SOCIAL", AccountType.Equity, "3.1"),
            CreateAccount("3.1.1.1", "CAPITAL PAGADO", AccountType.Equity, "3.1.1"),
            CreateAccount("3.1.1.1.001", "CAPITAL NEGOCIO X", AccountType.Equity, "3.1.1.1"),
            CreateAccount("3.1.2", "RESERVAS", AccountType.Equity, "3.1"),
            CreateAccount("3.1.2.1", "RESERVAS PATRIMONIALES", AccountType.Equity, "3.1.2"),
            CreateAccount("3.1.2.1.001", "RESERVA FACULTATIVA", AccountType.Equity, "3.1.2.1"),
            CreateAccount("3.1.2.1.002", "RESERVA DE CAPITAL", AccountType.Equity, "3.1.2.1"),
            CreateAccount("3.1.2.1.003", "RESERVA LEGAL", AccountType.Equity, "3.1.2.1"),
            CreateAccount("3.1.3", "RESULTADOS", AccountType.Equity, "3.1"),
            CreateAccount("3.1.3.1", "RESULTADOS ANTERIORES", AccountType.Equity, "3.1.3"),
            CreateAccount("3.1.3.1.001", "UTILIDADES ANTERIORES", AccountType.Equity, "3.1.3.1"),
            CreateAccount("3.1.3.1.002", "PERDIDAS ANTERIORES", AccountType.Equity, "3.1.3.1"),
            CreateAccount("3.1.3.1.003", "AMORTIZACION PERDIDAS ANTERIORES", AccountType.Equity, "3.1.3.1"),
            CreateAccount("3.1.3.2", "RESULTADOS DEL EJERCICIO", AccountType.Equity, "3.1.3"),
            CreateAccount("3.1.3.2.001", "UTILIDADES DE EJERCICIO", AccountType.Equity, "3.1.3.2"),

            // INGRESOS (4)
            CreateAccount("4", "INGRESOS", AccountType.Revenue, null),
            CreateAccount("4.1", "INGRESOS OPERACIONALES", AccountType.Revenue, "4"),
            CreateAccount("4.1.1", "VENTAS NETAS", AccountType.Revenue, "4.1"),
            CreateAccount("4.1.1.1", "VENTAS DE GRANOS BASICOS", AccountType.Revenue, "4.1.1"),
            CreateAccount("4.1.1.1.001", "VENTAS GENERALES", AccountType.Revenue, "4.1.1.1"),

            // COSTOS (5)
            CreateAccount("5", "COSTOS", AccountType.Cost, null),
            CreateAccount("5.1", "COSTOS OPERACIONALES", AccountType.Cost, "5"),
            CreateAccount("5.1.1", "COSTO DE VENTA", AccountType.Cost, "5.1"),
            CreateAccount("5.1.1.1", "COSTOS DE VENTAS GRANOS BASICO", AccountType.Cost, "5.1.1"),
            CreateAccount("5.1.1.1.001", "COSTO GRANOS BASICO", AccountType.Cost, "5.1.1.1"),

            // GASTOS (6)
            CreateAccount("6", "GASTOS", AccountType.Expense, null),
            CreateAccount("6.1", "GASTOS OPERACIONALES", AccountType.Expense, "6"),
            CreateAccount("6.1.1", "GASTOS DE ADMINISTRACION", AccountType.Expense, "6.1"),
            CreateAccount("6.1.1.1", "SUELDOS Y SIMILARES ADMON", AccountType.Expense, "6.1.1"),
            CreateAccount("6.1.1.1.001", "SUELDOS Y SALARIOS ADMON", AccountType.Expense, "6.1.1.1"),
            CreateAccount("6.1.1.1.002", "COMPONENTE SALARIAL ADM", AccountType.Expense, "6.1.1.1"),
            CreateAccount("6.1.1.1.003", "HORA EXTRA ADM", AccountType.Expense, "6.1.1.1"),
            CreateAccount("6.1.2", "GASTOS DE VENTAS", AccountType.Expense, "6.1"),
            CreateAccount("6.1.2.1", "SUELDOS Y SIMILARES", AccountType.Expense, "6.1.2"),
            CreateAccount("6.1.2.1.001", "SUELDOS Y SALARIOS DE VENTAS", AccountType.Expense, "6.1.2.1"),
            CreateAccount("6.1.2.1.002", "COMPROBANTE SALARIAL DE VTA", AccountType.Expense, "6.1.2.1"),
            CreateAccount("6.1.2.1.003", "HORAS EXTRAS VTAS", AccountType.Expense, "6.1.2.1"),
            CreateAccount("6.1.3", "GASTOS FINANCIEROS", AccountType.Expense, "6.1"),
            CreateAccount("6.1.3.1", "GASTOS BANCARIOS", AccountType.Expense, "6.1.3"),
            CreateAccount("6.1.3.1.001", "GTOS BANC.X CHEQUERA", AccountType.Expense, "6.1.3.1"),
            CreateAccount("6.1.3.1.002", "GAST.X SERV.BANC.", AccountType.Expense, "6.1.3.1"),
            CreateAccount("6.1.3.1.003", "GAST. X DEVALUACION DE LA MONEDA FIN.", AccountType.Expense, "6.1.3.1"),
            CreateAccount("6.1.4", "GASTOS GENERALES", AccountType.Expense, "6.1"),
            CreateAccount("6.1.4.1", "MANTENIMIENTO DE MUEBLES Y ENSERES GEN", AccountType.Expense, "6.1.4"),
            CreateAccount("6.1.4.1.001", "MANT. DE MUEBLES GEN", AccountType.Expense, "6.1.4.1"),
            CreateAccount("6.1.4.1.002", "MANT. DE ENSERES GEN", AccountType.Expense, "6.1.4.1"),
            CreateAccount("6.1.4.2", "CONS. DE ENEGIA ELECTRICA GEN", AccountType.Expense, "6.1.4"),
            CreateAccount("6.1.4.2.01", "CONS.ENEGIA ENEL GEN", AccountType.Expense, "6.1.4.2"),
            CreateAccount("6.1.4.3", "MOVILIZACIONE LOCALE GEN", AccountType.Expense, "6.1.4"),
            CreateAccount("6.1.4.3.003", "VIATICOS GEN", AccountType.Expense, "6.1.4.3"),
            CreateAccount("6.1.4.3.004", "PASAJES AEREOS GEN.", AccountType.Expense, "6.1.4.3")
        };

        modelBuilder.Entity<Account>().HasData(accounts);
    }

    private static Account CreateAccount(string code, string name, AccountType type, string? parentCode)
    {
        return new Account
        {
            Id = GenerateGuid(code),
            Code = code,
            Name = name,
            Type = type,
            ParentAccountId = parentCode != null ? GenerateGuid(parentCode) : null,
            CreatedAt = new DateTime(2026, 3, 9)
        };
    }

    // Helper to generate a deterministic Guid based on the account code
    private static Guid GenerateGuid(string key)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        byte[] hash = md5.ComputeHash(System.Text.Encoding.Default.GetBytes(key));
        return new Guid(hash);
    }
}
