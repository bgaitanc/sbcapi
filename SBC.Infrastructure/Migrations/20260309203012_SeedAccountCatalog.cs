using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace SBC.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedAccountCatalog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "accounting",
                table: "Accounts",
                columns: new[] { "Id", "Code", "CreatedAt", "CreatedBy", "Name", "ParentAccountId", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("1c097916-885a-af0f-6fb5-e6087eb1b2dc"), "6", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "GASTOS", null, "Expense", null, null },
                    { new Guid("3842cac4-b9a0-8223-0dcc-509a6f75849b"), "1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ACTIVO", null, "Asset", null, null },
                    { new Guid("79f67fa8-f3a2-1de7-9181-a67b7542122c"), "4", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INGRESOS", null, "Revenue", null, null },
                    { new Guid("7ec8cbec-5c4b-fee2-2830-8fd9f2a7baf3"), "3", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PATRIMONIO", null, "Equity", null, null },
                    { new Guid("7f3bdae4-cebb-4523-d777-2b0674a318d5"), "5", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "COSTOS", null, "Cost", null, null },
                    { new Guid("8d721ec8-4c9d-632f-6f06-7f89cc14862c"), "2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PASIVO", null, "Liability", null, null },
                    { new Guid("4a075a89-215b-2263-121b-5f76bc831927"), "4.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INGRESOS OPERACIONALES", new Guid("79f67fa8-f3a2-1de7-9181-a67b7542122c"), "Revenue", null, null },
                    { new Guid("4f19ff43-0f41-933e-a868-0bef5ba51e50"), "5.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "COSTOS OPERACIONALES", new Guid("7f3bdae4-cebb-4523-d777-2b0674a318d5"), "Cost", null, null },
                    { new Guid("72547656-0468-4901-9c79-732468ba4340"), "1.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ACTIVOS FIJOS", new Guid("3842cac4-b9a0-8223-0dcc-509a6f75849b"), "Asset", null, null },
                    { new Guid("757d2008-8a2d-6356-68d1-fd97b6142fa3"), "2.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PASIVOS A CORTO PLAZO", new Guid("8d721ec8-4c9d-632f-6f06-7f89cc14862c"), "Liability", null, null },
                    { new Guid("79d51817-1186-6a0b-f2dd-96b59d3db416"), "2.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PASIVOS A LARGO PLAZO", new Guid("8d721ec8-4c9d-632f-6f06-7f89cc14862c"), "Liability", null, null },
                    { new Guid("958a065b-2c44-557d-05b4-166a77357ea5"), "3.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PATRIMONIO NEGOCIO X", new Guid("7ec8cbec-5c4b-fee2-2830-8fd9f2a7baf3"), "Equity", null, null },
                    { new Guid("bb457d77-dfbc-d450-9c42-c70ad7acf5fe"), "1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ACTIVOS CIRCULANTES", new Guid("3842cac4-b9a0-8223-0dcc-509a6f75849b"), "Asset", null, null },
                    { new Guid("c382ebd6-ca24-f3b2-6e34-5863fad95090"), "6.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "GASTOS OPERACIONALES", new Guid("1c097916-885a-af0f-6fb5-e6087eb1b2dc"), "Expense", null, null },
                    { new Guid("f1c90e0b-28cc-c1b3-9dc6-c36dcd5af7cc"), "1.3", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "OTROS ACTIVOS", new Guid("3842cac4-b9a0-8223-0dcc-509a6f75849b"), "Asset", null, null },
                    { new Guid("0d876295-827e-d045-3c2a-c6055dff735f"), "2.1.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "NOMINA POR PAGAR", new Guid("757d2008-8a2d-6356-68d1-fd97b6142fa3"), "Liability", null, null },
                    { new Guid("18982fad-c120-6be2-9c95-b61832af190a"), "1.1.3", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ACTIVOS REALIZABLES", new Guid("bb457d77-dfbc-d450-9c42-c70ad7acf5fe"), "Asset", null, null },
                    { new Guid("34baba1e-f365-0691-375b-e623a0ebab45"), "6.1.3", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "GASTOS FINANCIEROS", new Guid("c382ebd6-ca24-f3b2-6e34-5863fad95090"), "Expense", null, null },
                    { new Guid("38e6df38-cf8a-3ca8-3486-d41f22b08165"), "1.3.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "OTROS ACTIVOS VARIOS", new Guid("f1c90e0b-28cc-c1b3-9dc6-c36dcd5af7cc"), "Asset", null, null },
                    { new Guid("4ab5517f-c03f-255c-effb-04f483552de7"), "6.1.4", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "GASTOS GENERALES", new Guid("c382ebd6-ca24-f3b2-6e34-5863fad95090"), "Expense", null, null },
                    { new Guid("4bc20f2c-303d-3138-7dc5-1c05339856d0"), "5.1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "COSTO DE VENTA", new Guid("4f19ff43-0f41-933e-a868-0bef5ba51e50"), "Cost", null, null },
                    { new Guid("50004e1f-f96f-5ffb-e179-f2ba1c60ff61"), "3.1.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RESERVAS", new Guid("958a065b-2c44-557d-05b4-166a77357ea5"), "Equity", null, null },
                    { new Guid("569817b1-37b0-b82c-7779-75cb658548ac"), "3.1.3", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RESULTADOS", new Guid("958a065b-2c44-557d-05b4-166a77357ea5"), "Equity", null, null },
                    { new Guid("755be1d9-1199-386e-f870-d0dc600c08a4"), "3.1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "CAPITAL SOCIAL", new Guid("958a065b-2c44-557d-05b4-166a77357ea5"), "Equity", null, null },
                    { new Guid("7ab5ac54-70fb-0f11-b7f8-e1de35e176b4"), "1.3.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ACTIVOS DIFERIDOS", new Guid("f1c90e0b-28cc-c1b3-9dc6-c36dcd5af7cc"), "Asset", null, null },
                    { new Guid("9031b59d-ad2d-bdda-db83-b22e9629f0ad"), "2.1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "CUENTAS POR PAGAR", new Guid("757d2008-8a2d-6356-68d1-fd97b6142fa3"), "Liability", null, null },
                    { new Guid("95a97e73-213b-ffc4-abc3-49e9b20f3b4b"), "4.1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "VENTAS NETAS", new Guid("4a075a89-215b-2263-121b-5f76bc831927"), "Revenue", null, null },
                    { new Guid("98752b81-e8bb-cd81-8e5e-faa29fc2d684"), "1.1.4", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "OTROS ACTIVOS CIRCULANTES", new Guid("bb457d77-dfbc-d450-9c42-c70ad7acf5fe"), "Asset", null, null },
                    { new Guid("9ccd2c87-ce6d-ce18-6ea4-d5106540f089"), "1.1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ACTIVOS DISPONIBLES", new Guid("bb457d77-dfbc-d450-9c42-c70ad7acf5fe"), "Asset", null, null },
                    { new Guid("ae1d70bf-ff9a-b25b-2b8f-000dc9bf6199"), "1.2.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ACTIVOS INTANGIBLES", new Guid("72547656-0468-4901-9c79-732468ba4340"), "Asset", null, null },
                    { new Guid("b27a832c-cac7-dcdb-35b5-bd7115e9eff1"), "2.2.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PRESTAMOS A LARGO PLAZO", new Guid("79d51817-1186-6a0b-f2dd-96b59d3db416"), "Liability", null, null },
                    { new Guid("c666b7ac-7f06-6104-05f8-b1263b9e5172"), "1.2.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ACTIVOS TANGIBLES", new Guid("72547656-0468-4901-9c79-732468ba4340"), "Asset", null, null },
                    { new Guid("cda3be13-90e5-7c99-1094-f9bba14d719a"), "6.1.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "GASTOS DE VENTAS", new Guid("c382ebd6-ca24-f3b2-6e34-5863fad95090"), "Expense", null, null },
                    { new Guid("d6971f6c-ec7f-253e-5c0c-b2279cc943de"), "2.1.3", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "OBLIGACIONES FISCALES POR PAGAR", new Guid("757d2008-8a2d-6356-68d1-fd97b6142fa3"), "Liability", null, null },
                    { new Guid("db2b5881-4a25-e494-4644-24087c6479a8"), "6.1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "GASTOS DE ADMINISTRACION", new Guid("c382ebd6-ca24-f3b2-6e34-5863fad95090"), "Expense", null, null },
                    { new Guid("ef332c52-a0dd-dcb8-6ce9-0c991beb9666"), "1.1.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ACTIVOS EXIGIBLES", new Guid("bb457d77-dfbc-d450-9c42-c70ad7acf5fe"), "Asset", null, null },
                    { new Guid("05086698-eecd-2a36-7483-27ad6032805b"), "1.1.1.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "BANCOS", new Guid("9ccd2c87-ce6d-ce18-6ea4-d5106540f089"), "Asset", null, null },
                    { new Guid("13aa86e0-a17f-679f-d27b-39d0eca18610"), "1.1.1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "CAJA", new Guid("9ccd2c87-ce6d-ce18-6ea4-d5106540f089"), "Asset", null, null },
                    { new Guid("15ef207e-9661-34db-373f-49975ba2dee4"), "6.1.4.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "MANTENIMIENTO DE MUEBLES Y ENSERES GEN", new Guid("4ab5517f-c03f-255c-effb-04f483552de7"), "Expense", null, null },
                    { new Guid("17d217b3-e15f-c6b0-9aac-10ce7974ae03"), "3.1.3.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RESULTADOS ANTERIORES", new Guid("569817b1-37b0-b82c-7779-75cb658548ac"), "Equity", null, null },
                    { new Guid("196f42c8-9986-d647-79b6-6787d7b60150"), "1.1.4.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "IMPUESTOS ANTICIPADOS Y RETENIDOS", new Guid("98752b81-e8bb-cd81-8e5e-faa29fc2d684"), "Asset", null, null },
                    { new Guid("1da01327-c44b-cea8-26b3-fc7a88d44272"), "2.1.2.3", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PROVICIONES POR PAGAR", new Guid("0d876295-827e-d045-3c2a-c6055dff735f"), "Liability", null, null },
                    { new Guid("211015fc-50c4-e649-5733-166abc4b98aa"), "6.1.3.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "GASTOS BANCARIOS", new Guid("34baba1e-f365-0691-375b-e623a0ebab45"), "Expense", null, null },
                    { new Guid("23ace421-f903-1c88-b4c1-acd2adc2ad6d"), "2.2.1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PRESTMOS SOCIOS LARGO PLAZO", new Guid("b27a832c-cac7-dcdb-35b5-bd7115e9eff1"), "Liability", null, null },
                    { new Guid("266e4093-ff3e-dce8-5c15-018242515e34"), "2.1.3.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RETENCIONES FUENTE POR PAGAR", new Guid("d6971f6c-ec7f-253e-5c0c-b2279cc943de"), "Liability", null, null },
                    { new Guid("3626acf5-7033-ca29-b4b4-12086549cedb"), "3.1.1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "CAPITAL PAGADO", new Guid("755be1d9-1199-386e-f870-d0dc600c08a4"), "Equity", null, null },
                    { new Guid("3673955b-2b17-e07b-0ee4-a120d4395d53"), "3.1.2.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RESERVAS PATRIMONIALES", new Guid("50004e1f-f96f-5ffb-e179-f2ba1c60ff61"), "Equity", null, null },
                    { new Guid("3c1ca1bb-d9c1-61f8-3645-76bc485c05d1"), "1.3.2.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "DEPOSITOD ENTREGADO EN GARANTIA", new Guid("38e6df38-cf8a-3ca8-3486-d41f22b08165"), "Asset", null, null },
                    { new Guid("4b84144f-0594-c99f-a81d-2cb3ecb034e8"), "1.1.2.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "CUENTAS POR COBRAR", new Guid("ef332c52-a0dd-dcb8-6ce9-0c991beb9666"), "Asset", null, null },
                    { new Guid("525df4d7-7aee-31f1-910f-7aa261128d7a"), "1.3.1.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "GASTOS DE CONSTRUCCION.", new Guid("7ab5ac54-70fb-0f11-b7f8-e1de35e176b4"), "Asset", null, null },
                    { new Guid("656b93a7-5040-e593-bc2a-017ff28354f9"), "6.1.4.3", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "MOVILIZACIONE LOCALE GEN", new Guid("4ab5517f-c03f-255c-effb-04f483552de7"), "Expense", null, null },
                    { new Guid("6a00dc2b-cbeb-d874-5c4b-e559ddb582e3"), "2.1.3.5", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RETENCIONES SOBRE SALARIO", new Guid("d6971f6c-ec7f-253e-5c0c-b2279cc943de"), "Liability", null, null },
                    { new Guid("73ce7494-6d10-2853-c84e-dc9a7f9b2637"), "2.1.1.4", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "OTRAS CUENTAS POR PAGAR", new Guid("9031b59d-ad2d-bdda-db83-b22e9629f0ad"), "Liability", null, null },
                    { new Guid("767f1fc0-1ba4-7a2d-576a-352f788142ab"), "3.1.3.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RESULTADOS DEL EJERCICIO", new Guid("569817b1-37b0-b82c-7779-75cb658548ac"), "Equity", null, null },
                    { new Guid("774856af-d174-b7e3-ce13-083119d763eb"), "1.2.2.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "SOFTWARE", new Guid("ae1d70bf-ff9a-b25b-2b8f-000dc9bf6199"), "Asset", null, null },
                    { new Guid("79c149f7-c0f0-bee9-516b-3aa825de2661"), "2.1.2.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "REMUNERACIONES POR PAGAR", new Guid("0d876295-827e-d045-3c2a-c6055dff735f"), "Liability", null, null },
                    { new Guid("7eaedbd3-ce30-a908-e7c8-7de13cbbb3a7"), "4.1.1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "VENTAS DE GRANOS BASICOS", new Guid("95a97e73-213b-ffc4-abc3-49e9b20f3b4b"), "Revenue", null, null },
                    { new Guid("7f9435aa-977b-1298-11a6-9de1b591ab17"), "5.1.1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "COSTOS DE VENTAS GRANOS BASICO", new Guid("4bc20f2c-303d-3138-7dc5-1c05339856d0"), "Cost", null, null },
                    { new Guid("8df70780-75bf-6674-f1f3-3a05292c3aa1"), "1.2.2.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "MARCAS", new Guid("ae1d70bf-ff9a-b25b-2b8f-000dc9bf6199"), "Asset", null, null },
                    { new Guid("9b056158-b76d-ba52-9c81-4c3accf7e261"), "2.1.3.3", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RETENCIONES IVA POR PAGAR", new Guid("d6971f6c-ec7f-253e-5c0c-b2279cc943de"), "Liability", null, null },
                    { new Guid("a1cadea3-6b99-6d1c-d169-7d5f2556f7be"), "1.2.1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ACTIVOS DEPRECIABLES", new Guid("c666b7ac-7f06-6104-05f8-b1263b9e5172"), "Asset", null, null },
                    { new Guid("a6daff80-8852-f23c-41b7-1e8accaf220e"), "2.1.3.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "IMPUESTOS POR PAGAR", new Guid("d6971f6c-ec7f-253e-5c0c-b2279cc943de"), "Liability", null, null },
                    { new Guid("aaec7a79-7dd5-229e-d7ad-51ffb466c25d"), "6.1.4.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "CONS. DE ENEGIA ELECTRICA GEN", new Guid("4ab5517f-c03f-255c-effb-04f483552de7"), "Expense", null, null },
                    { new Guid("ade886b2-349b-bbd0-0241-ed78624440f7"), "2.1.3.4", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "OTROS IMPUESTOS POR PAGAR", new Guid("d6971f6c-ec7f-253e-5c0c-b2279cc943de"), "Liability", null, null },
                    { new Guid("afec596b-3e4f-123e-194a-ae1cba1b3bb5"), "1.1.3.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INVENTARIO DE MERCADERIAS", new Guid("18982fad-c120-6be2-9c95-b61832af190a"), "Asset", null, null },
                    { new Guid("b36e7827-b3be-be30-3470-f4630df519d4"), "2.1.1.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PROVEEDORES LOCALES", new Guid("9031b59d-ad2d-bdda-db83-b22e9629f0ad"), "Liability", null, null },
                    { new Guid("bfc8a6f5-e6d6-8bd1-4978-a5134c3ddc1e"), "2.1.1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PROVEEDORES NACIONALES", new Guid("9031b59d-ad2d-bdda-db83-b22e9629f0ad"), "Liability", null, null },
                    { new Guid("c5ce6e69-6ea8-e419-d4df-56006eccda0b"), "1.2.1.3", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "DEPRECIACION ACUMULDA", new Guid("c666b7ac-7f06-6104-05f8-b1263b9e5172"), "Asset", null, null },
                    { new Guid("d9955cc3-1288-55a9-f190-cd4067bb58b8"), "1.2.1.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ACTIVOS NO DEPRECIABLES", new Guid("c666b7ac-7f06-6104-05f8-b1263b9e5172"), "Asset", null, null },
                    { new Guid("e70c4cd0-6059-c7c7-6a59-ea09e4101e63"), "2.1.1.3", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "SERVICIOS POR PAGAR", new Guid("9031b59d-ad2d-bdda-db83-b22e9629f0ad"), "Liability", null, null },
                    { new Guid("e87d3e46-ee55-c893-54a3-80b4969fd8b1"), "1.3.1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "GASTOS DE CONSTITUCION.", new Guid("7ab5ac54-70fb-0f11-b7f8-e1de35e176b4"), "Asset", null, null },
                    { new Guid("f0b4d215-484b-098d-7836-94890eb57cc4"), "2.1.2.2", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INSS E INATEC POR PAGAR", new Guid("0d876295-827e-d045-3c2a-c6055dff735f"), "Liability", null, null },
                    { new Guid("f5a4aba4-d301-f98e-d4d3-01f1d08d4969"), "6.1.1.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "SUELDOS Y SIMILARES ADMON", new Guid("db2b5881-4a25-e494-4644-24087c6479a8"), "Expense", null, null },
                    { new Guid("faa6c09e-00ec-2d42-302e-294aba2005bb"), "1.1.3.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INVENTARIO DE MATERIALES", new Guid("18982fad-c120-6be2-9c95-b61832af190a"), "Asset", null, null },
                    { new Guid("fe1cc2bd-fc34-b430-032c-77dce53c3db7"), "6.1.2.1", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "SUELDOS Y SIMILARES", new Guid("cda3be13-90e5-7c99-1094-f9bba14d719a"), "Expense", null, null },
                    { new Guid("0363d213-c9ce-d7ad-775a-8519b0f98e0c"), "2.1.3.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "IMPUESTO A LA RENTA POR PAGAR", new Guid("a6daff80-8852-f23c-41b7-1e8accaf220e"), "Liability", null, null },
                    { new Guid("07c24aa9-42ea-079c-6e46-1b4cbc06baf0"), "1.1.3.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INV. DE ACCESORIOS", new Guid("faa6c09e-00ec-2d42-302e-294aba2005bb"), "Asset", null, null },
                    { new Guid("07da8880-1255-0ae2-f774-1a081e52b977"), "2.2.1.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "R.G. PMO LARGO PLAZO", new Guid("23ace421-f903-1c88-b4c1-acd2adc2ad6d"), "Liability", null, null },
                    { new Guid("12be69f0-a634-3621-9fb7-55104857dbcc"), "1.1.3.2.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "GRANOS BASICOS", new Guid("afec596b-3e4f-123e-194a-ae1cba1b3bb5"), "Asset", null, null },
                    { new Guid("1a018f25-53e3-fa4d-8a66-eaca84eaf11b"), "3.1.2.1.003", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RESERVA LEGAL", new Guid("3673955b-2b17-e07b-0ee4-a120d4395d53"), "Equity", null, null },
                    { new Guid("21e7b256-5e0f-2a49-9282-1735e7c895b6"), "6.1.4.2.01", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "CONS.ENEGIA ENEL GEN", new Guid("aaec7a79-7dd5-229e-d7ad-51ffb466c25d"), "Expense", null, null },
                    { new Guid("257f3d2e-f9e1-61e1-a4ee-ca228abdfee5"), "2.1.2.3.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "DECIMO TERCER MES POR PAGAR", new Guid("1da01327-c44b-cea8-26b3-fc7a88d44272"), "Liability", null, null },
                    { new Guid("27be1f94-a9a8-fa73-c98e-33b275b550f6"), "4.1.1.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "VENTAS GENERALES", new Guid("7eaedbd3-ce30-a908-e7c8-7de13cbbb3a7"), "Revenue", null, null },
                    { new Guid("27d353af-8e1f-7e03-3ab5-5a3442255f3c"), "1.2.2.2.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "SOFTWARE CONTABLE", new Guid("774856af-d174-b7e3-ce13-083119d763eb"), "Asset", null, null },
                    { new Guid("2ec895fd-fe31-8e1e-77b5-63482ba3fbf2"), "1.2.2.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "SERVICIO DE EMPRESA", new Guid("8df70780-75bf-6674-f1f3-3a05292c3aa1"), "Asset", null, null },
                    { new Guid("313d34ad-48e4-59a5-7aec-8fabcf67c423"), "6.1.2.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "SUELDOS Y SALARIOS DE VENTAS", new Guid("fe1cc2bd-fc34-b430-032c-77dce53c3db7"), "Expense", null, null },
                    { new Guid("38efa72e-98a8-40bb-2b48-d7ad00e130d6"), "2.1.2.2.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INATEC POR PAGAR", new Guid("f0b4d215-484b-098d-7836-94890eb57cc4"), "Liability", null, null },
                    { new Guid("40f69734-81c9-7422-f498-60850cb237e6"), "6.1.1.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "SUELDOS Y SALARIOS ADMON", new Guid("f5a4aba4-d301-f98e-d4d3-01f1d08d4969"), "Expense", null, null },
                    { new Guid("43b2c282-b427-8cbc-a086-6048dd1fd5ed"), "6.1.4.3.003", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "VIATICOS GEN", new Guid("656b93a7-5040-e593-bc2a-017ff28354f9"), "Expense", null, null },
                    { new Guid("44f7696f-3e31-0780-52b9-c0dc12ea8560"), "2.1.3.2.004", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "10% RET.FTE.HONORARIOS PROFESIONALES", new Guid("266e4093-ff3e-dce8-5c15-018242515e34"), "Liability", null, null },
                    { new Guid("46cc841a-eb91-91d8-af99-065de15e703d"), "2.1.3.5.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RETENCIONES SOBRE SALARIO DGI XP", new Guid("6a00dc2b-cbeb-d874-5c4b-e559ddb582e3"), "Liability", null, null },
                    { new Guid("473eb7e9-df3d-e2af-7736-c83873def473"), "1.2.1.1.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INSTALACIONES", new Guid("a1cadea3-6b99-6d1c-d169-7d5f2556f7be"), "Asset", null, null },
                    { new Guid("4858281d-8ec6-9401-b54e-35eaf2ea5a35"), "1.2.1.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INMUEBLES", new Guid("a1cadea3-6b99-6d1c-d169-7d5f2556f7be"), "Asset", null, null },
                    { new Guid("4c431c65-3643-9525-c750-c51871bcf52d"), "6.1.2.1.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "COMPROBANTE SALARIAL DE VTA", new Guid("fe1cc2bd-fc34-b430-032c-77dce53c3db7"), "Expense", null, null },
                    { new Guid("5643c345-4bf4-7d01-36e9-ca732b95d6ab"), "6.1.3.1.003", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "GAST. X DEVALUACION DE LA MONEDA FIN.", new Guid("211015fc-50c4-e649-5733-166abc4b98aa"), "Expense", null, null },
                    { new Guid("5a1249d2-0207-cca7-6340-3d8af5ed4dbf"), "2.1.1.4.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "CUENTAS VARIAS POR PAGAR", new Guid("73ce7494-6d10-2853-c84e-dc9a7f9b2637"), "Liability", null, null },
                    { new Guid("5fbdec57-dbf0-f6f0-8017-c174e31346d9"), "2.1.2.3.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "VACACIONES POR PAGAR", new Guid("1da01327-c44b-cea8-26b3-fc7a88d44272"), "Liability", null, null },
                    { new Guid("62fac3b9-f703-2e9d-d792-e8e218706f9d"), "1.3.1.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "GASTOS DE CONSTITUCION", new Guid("e87d3e46-ee55-c893-54a3-80b4969fd8b1"), "Asset", null, null },
                    { new Guid("6493f394-6f81-5d94-13bf-9c15b10686d6"), "1.1.4.1.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ANTICIPO IMPUESTO A LA RENTA", new Guid("196f42c8-9986-d647-79b6-6787d7b60150"), "Asset", null, null },
                    { new Guid("6ebcf058-ade8-58e6-5253-ea8439434788"), "6.1.2.1.003", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "HORAS EXTRAS VTAS", new Guid("fe1cc2bd-fc34-b430-032c-77dce53c3db7"), "Expense", null, null },
                    { new Guid("7d4a6143-5a4c-d1f9-ea57-9bbd17fbd00a"), "2.1.2.1.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "DESCUENTOS POR ATRASOS", new Guid("79c149f7-c0f0-bee9-516b-3aa825de2661"), "Liability", null, null },
                    { new Guid("8409a2b7-6118-8543-2b31-2423775ac901"), "2.1.3.3.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "15% RET. IVA VENTAS", new Guid("9b056158-b76d-ba52-9c81-4c3accf7e261"), "Liability", null, null },
                    { new Guid("85326ad4-3b12-143b-a3f1-0487fd374020"), "2.1.2.1.003", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "LIQUIDACIONES POR PAGAR", new Guid("79c149f7-c0f0-bee9-516b-3aa825de2661"), "Liability", null, null },
                    { new Guid("879ac69a-8156-7b3a-8564-b3bb360b10b3"), "2.1.3.2.003", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "2% RET.FTE. SERVICIOS", new Guid("266e4093-ff3e-dce8-5c15-018242515e34"), "Liability", null, null },
                    { new Guid("8b2fd487-3dd7-0693-94a3-c8bc701278f3"), "2.1.2.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "SUELDOS POR PAGAR", new Guid("79c149f7-c0f0-bee9-516b-3aa825de2661"), "Liability", null, null },
                    { new Guid("91799361-9815-c5f2-18ca-7f3310389b5d"), "1.3.2.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INMUEBLES ENTREGADOS EN GARANTIA", new Guid("3c1ca1bb-d9c1-61f8-3645-76bc485c05d1"), "Asset", null, null },
                    { new Guid("954d67be-fbd3-a55b-00b2-34d0a39b26c4"), "3.1.3.1.003", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "AMORTIZACION PERDIDAS ANTERIORES", new Guid("17d217b3-e15f-c6b0-9aac-10ce7974ae03"), "Equity", null, null },
                    { new Guid("97434d56-efd7-d685-1bae-80c351fca817"), "1.1.2.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "CUENTAS POR COBRAR CLIENTES", new Guid("4b84144f-0594-c99f-a81d-2cb3ecb034e8"), "Asset", null, null },
                    { new Guid("a0954ed0-2da7-bac6-3890-9d9da858a7f2"), "1.2.1.3.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INSTALACIONES DEP", new Guid("c5ce6e69-6ea8-e419-d4df-56006eccda0b"), "Asset", null, null },
                    { new Guid("a31ab429-8b95-89a0-6be3-7863ea265ace"), "3.1.2.1.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RESERVA DE CAPITAL", new Guid("3673955b-2b17-e07b-0ee4-a120d4395d53"), "Equity", null, null },
                    { new Guid("a37e6afd-53e8-8256-2bcc-0f1a1c20dd1d"), "2.1.3.1.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "IVA POR PAGAR (15%)", new Guid("a6daff80-8852-f23c-41b7-1e8accaf220e"), "Liability", null, null },
                    { new Guid("a52e29a6-cdb3-9e51-d93d-29cc07a883f0"), "1.1.1.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "CAJA GENERAL", new Guid("13aa86e0-a17f-679f-d27b-39d0eca18610"), "Asset", null, null },
                    { new Guid("a5f5bfc1-dea1-2df2-c202-60b26aac0a37"), "6.1.4.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "MANT. DE MUEBLES GEN", new Guid("15ef207e-9661-34db-373f-49975ba2dee4"), "Expense", null, null },
                    { new Guid("a7083d88-2f13-eda9-9346-bdb40410d938"), "2.1.3.2.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "2% RET. FTE DE COMPRAS", new Guid("266e4093-ff3e-dce8-5c15-018242515e34"), "Liability", null, null },
                    { new Guid("a7716697-fbbf-6e26-bcbc-41a6f4eb3b84"), "3.1.3.2.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "UTILIDADES DE EJERCICIO", new Guid("767f1fc0-1ba4-7a2d-576a-352f788142ab"), "Equity", null, null },
                    { new Guid("a8c0cb02-b212-7879-117e-da3922c99fcb"), "6.1.1.1.003", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "HORA EXTRA ADM", new Guid("f5a4aba4-d301-f98e-d4d3-01f1d08d4969"), "Expense", null, null },
                    { new Guid("aa28873d-a2ab-585f-1fee-beeae1a9147d"), "1.1.3.1.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INV. DE PAPERIA Y UTILES DE OFICINA", new Guid("faa6c09e-00ec-2d42-302e-294aba2005bb"), "Asset", null, null },
                    { new Guid("b0ab5b5b-990c-3464-6812-cd761197a647"), "6.1.3.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "GTOS BANC.X CHEQUERA", new Guid("211015fc-50c4-e649-5733-166abc4b98aa"), "Expense", null, null },
                    { new Guid("b0dd41f4-b175-c200-9b15-e5e9431f366b"), "2.1.2.2.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "APORTES PERSONAL Y PATRONAL CXP", new Guid("f0b4d215-484b-098d-7836-94890eb57cc4"), "Liability", null, null },
                    { new Guid("b250719e-2cdc-36fd-ff9d-84827cd584e5"), "1.2.2.1.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "MARCA DE PRODUCTO", new Guid("8df70780-75bf-6674-f1f3-3a05292c3aa1"), "Asset", null, null },
                    { new Guid("b4f1ac33-f8b3-6db6-0031-1bab741ffda8"), "2.1.1.3.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "TELEFONO E INTERNET CXP", new Guid("e70c4cd0-6059-c7c7-6a59-ea09e4101e63"), "Liability", null, null },
                    { new Guid("b517f440-0d2b-4b7f-177b-881fb204523f"), "1.2.1.2.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "TERRENO # 1", new Guid("d9955cc3-1288-55a9-f190-cd4067bb58b8"), "Asset", null, null },
                    { new Guid("b8cc9014-90f3-97b3-612c-049111fccf0f"), "3.1.3.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "UTILIDADES ANTERIORES", new Guid("17d217b3-e15f-c6b0-9aac-10ce7974ae03"), "Equity", null, null },
                    { new Guid("ba20cb9d-8cfe-6b83-1cc0-a72fb3e48c23"), "5.1.1.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "COSTO GRANOS BASICO", new Guid("7f9435aa-977b-1298-11a6-9de1b591ab17"), "Cost", null, null },
                    { new Guid("bc3743f5-750f-9072-dce3-02ead55da913"), "1.1.4.1.003", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ANTICIPO RETENCION EN LA FUENTE", new Guid("196f42c8-9986-d647-79b6-6787d7b60150"), "Asset", null, null },
                    { new Guid("c3eb59ee-e0b2-b98f-a474-e2075e164b8b"), "2.1.1.2.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PROVEEDOR X CXP", new Guid("b36e7827-b3be-be30-3470-f4630df519d4"), "Liability", null, null },
                    { new Guid("c695234b-de5d-e1c5-d9e1-d15e752bfaa2"), "1.1.1.2.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "EMPRESA X CTA.CTE.#10012705215805", new Guid("05086698-eecd-2a36-7483-27ad6032805b"), "Asset", null, null },
                    { new Guid("c92eaec2-b569-ea16-edb5-bb72dd6e5ac2"), "3.1.2.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "RESERVA FACULTATIVA", new Guid("3673955b-2b17-e07b-0ee4-a120d4395d53"), "Equity", null, null },
                    { new Guid("c9f80cf2-4d00-b5dd-38fa-53ba0391b89d"), "1.1.2.1.003", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "CUENTAS POR COBRAR OTROS", new Guid("4b84144f-0594-c99f-a81d-2cb3ecb034e8"), "Asset", null, null },
                    { new Guid("ce8cda09-a336-fba3-08bf-bdaf87d7e881"), "6.1.4.3.004", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PASAJES AEREOS GEN.", new Guid("656b93a7-5040-e593-bc2a-017ff28354f9"), "Expense", null, null },
                    { new Guid("d0dd85ee-228b-9388-2484-985e2589052a"), "3.1.1.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "CAPITAL NEGOCIO X", new Guid("3626acf5-7033-ca29-b4b4-12086549cedb"), "Equity", null, null },
                    { new Guid("d2d33fc7-e48b-40d0-9b38-0b55f70f3b4a"), "6.1.3.1.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "GAST.X SERV.BANC.", new Guid("211015fc-50c4-e649-5733-166abc4b98aa"), "Expense", null, null },
                    { new Guid("d4767614-346d-fdd3-419c-e90872e2d2d2"), "2.1.1.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INDUSTRIALES CXP", new Guid("bfc8a6f5-e6d6-8bd1-4978-a5134c3ddc1e"), "Liability", null, null },
                    { new Guid("d5e1e9e8-58e0-1b00-e6f0-b41429bc4458"), "1.2.1.3.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INMUEBLES DEP", new Guid("c5ce6e69-6ea8-e419-d4df-56006eccda0b"), "Asset", null, null },
                    { new Guid("d5e7ff31-8436-74e3-b31b-a1d6ba97a786"), "1.1.3.1.003", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INV. DE MATERIALES Y LIMPIEZA", new Guid("faa6c09e-00ec-2d42-302e-294aba2005bb"), "Asset", null, null },
                    { new Guid("dbb04a13-bfcb-8e76-7837-d9bc42513c15"), "1.1.1.1.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "CAJA CHICA", new Guid("13aa86e0-a17f-679f-d27b-39d0eca18610"), "Asset", null, null },
                    { new Guid("de9277ca-b7e4-072f-6622-717675de5ce9"), "2.1.3.2.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "5% RET.FTE. SERVICIO", new Guid("266e4093-ff3e-dce8-5c15-018242515e34"), "Liability", null, null },
                    { new Guid("e149409e-ff82-ec4e-3341-5197c827d6bd"), "3.1.3.1.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "PERDIDAS ANTERIORES", new Guid("17d217b3-e15f-c6b0-9aac-10ce7974ae03"), "Equity", null, null },
                    { new Guid("e98ee395-a07e-8a8a-bf51-1e36a5ac6aab"), "6.1.1.1.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "COMPONENTE SALARIAL ADM", new Guid("f5a4aba4-d301-f98e-d4d3-01f1d08d4969"), "Expense", null, null },
                    { new Guid("eb47bb3a-0fb4-8e75-50fd-8d1b0761b2bb"), "1.1.2.1.004", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "ANTICIPO A JUSTIFICAR", new Guid("4b84144f-0594-c99f-a81d-2cb3ecb034e8"), "Asset", null, null },
                    { new Guid("ee6c0412-accd-8edf-21ca-73ab74c14dc7"), "6.1.4.1.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "MANT. DE ENSERES GEN", new Guid("15ef207e-9661-34db-373f-49975ba2dee4"), "Expense", null, null },
                    { new Guid("f05971ae-54e9-d7d9-037d-2d866d281f13"), "1.1.2.1.002", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "CUENTAS POR COBRAR EMPLEADOS", new Guid("4b84144f-0594-c99f-a81d-2cb3ecb034e8"), "Asset", null, null },
                    { new Guid("f1bcd7c9-f9b5-ffc9-d12e-99f24c76cd16"), "2.1.1.3.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "SERV. ENERGIA CXP", new Guid("e70c4cd0-6059-c7c7-6a59-ea09e4101e63"), "Liability", null, null },
                    { new Guid("f780c1a7-84a9-f2a7-df1e-4a9c9b495a2d"), "2.1.2.3.003", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "INDEMNIZACIONES POR PAGAR", new Guid("1da01327-c44b-cea8-26b3-fc7a88d44272"), "Liability", null, null },
                    { new Guid("fe67dc69-32cd-aba3-c699-d6f33205b14f"), "1.1.4.1.001", new DateTime(2026, 3, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "IVA CREDITO TRIBUNAL", new Guid("196f42c8-9986-d647-79b6-6787d7b60150"), "Asset", null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("0363d213-c9ce-d7ad-775a-8519b0f98e0c"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("07c24aa9-42ea-079c-6e46-1b4cbc06baf0"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("07da8880-1255-0ae2-f774-1a081e52b977"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("12be69f0-a634-3621-9fb7-55104857dbcc"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("1a018f25-53e3-fa4d-8a66-eaca84eaf11b"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("21e7b256-5e0f-2a49-9282-1735e7c895b6"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("257f3d2e-f9e1-61e1-a4ee-ca228abdfee5"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("27be1f94-a9a8-fa73-c98e-33b275b550f6"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("27d353af-8e1f-7e03-3ab5-5a3442255f3c"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("2ec895fd-fe31-8e1e-77b5-63482ba3fbf2"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("313d34ad-48e4-59a5-7aec-8fabcf67c423"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("38efa72e-98a8-40bb-2b48-d7ad00e130d6"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("40f69734-81c9-7422-f498-60850cb237e6"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("43b2c282-b427-8cbc-a086-6048dd1fd5ed"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("44f7696f-3e31-0780-52b9-c0dc12ea8560"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("46cc841a-eb91-91d8-af99-065de15e703d"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("473eb7e9-df3d-e2af-7736-c83873def473"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("4858281d-8ec6-9401-b54e-35eaf2ea5a35"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("4c431c65-3643-9525-c750-c51871bcf52d"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("525df4d7-7aee-31f1-910f-7aa261128d7a"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("5643c345-4bf4-7d01-36e9-ca732b95d6ab"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("5a1249d2-0207-cca7-6340-3d8af5ed4dbf"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("5fbdec57-dbf0-f6f0-8017-c174e31346d9"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("62fac3b9-f703-2e9d-d792-e8e218706f9d"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("6493f394-6f81-5d94-13bf-9c15b10686d6"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("6ebcf058-ade8-58e6-5253-ea8439434788"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("7d4a6143-5a4c-d1f9-ea57-9bbd17fbd00a"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("8409a2b7-6118-8543-2b31-2423775ac901"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("85326ad4-3b12-143b-a3f1-0487fd374020"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("879ac69a-8156-7b3a-8564-b3bb360b10b3"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("8b2fd487-3dd7-0693-94a3-c8bc701278f3"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("91799361-9815-c5f2-18ca-7f3310389b5d"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("954d67be-fbd3-a55b-00b2-34d0a39b26c4"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("97434d56-efd7-d685-1bae-80c351fca817"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("a0954ed0-2da7-bac6-3890-9d9da858a7f2"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("a31ab429-8b95-89a0-6be3-7863ea265ace"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("a37e6afd-53e8-8256-2bcc-0f1a1c20dd1d"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("a52e29a6-cdb3-9e51-d93d-29cc07a883f0"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("a5f5bfc1-dea1-2df2-c202-60b26aac0a37"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("a7083d88-2f13-eda9-9346-bdb40410d938"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("a7716697-fbbf-6e26-bcbc-41a6f4eb3b84"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("a8c0cb02-b212-7879-117e-da3922c99fcb"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("aa28873d-a2ab-585f-1fee-beeae1a9147d"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("ade886b2-349b-bbd0-0241-ed78624440f7"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("b0ab5b5b-990c-3464-6812-cd761197a647"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("b0dd41f4-b175-c200-9b15-e5e9431f366b"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("b250719e-2cdc-36fd-ff9d-84827cd584e5"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("b4f1ac33-f8b3-6db6-0031-1bab741ffda8"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("b517f440-0d2b-4b7f-177b-881fb204523f"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("b8cc9014-90f3-97b3-612c-049111fccf0f"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("ba20cb9d-8cfe-6b83-1cc0-a72fb3e48c23"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("bc3743f5-750f-9072-dce3-02ead55da913"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("c3eb59ee-e0b2-b98f-a474-e2075e164b8b"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("c695234b-de5d-e1c5-d9e1-d15e752bfaa2"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("c92eaec2-b569-ea16-edb5-bb72dd6e5ac2"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("c9f80cf2-4d00-b5dd-38fa-53ba0391b89d"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("ce8cda09-a336-fba3-08bf-bdaf87d7e881"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("d0dd85ee-228b-9388-2484-985e2589052a"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("d2d33fc7-e48b-40d0-9b38-0b55f70f3b4a"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("d4767614-346d-fdd3-419c-e90872e2d2d2"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("d5e1e9e8-58e0-1b00-e6f0-b41429bc4458"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("d5e7ff31-8436-74e3-b31b-a1d6ba97a786"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("dbb04a13-bfcb-8e76-7837-d9bc42513c15"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("de9277ca-b7e4-072f-6622-717675de5ce9"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("e149409e-ff82-ec4e-3341-5197c827d6bd"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("e98ee395-a07e-8a8a-bf51-1e36a5ac6aab"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("eb47bb3a-0fb4-8e75-50fd-8d1b0761b2bb"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("ee6c0412-accd-8edf-21ca-73ab74c14dc7"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("f05971ae-54e9-d7d9-037d-2d866d281f13"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("f1bcd7c9-f9b5-ffc9-d12e-99f24c76cd16"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("f780c1a7-84a9-f2a7-df1e-4a9c9b495a2d"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("fe67dc69-32cd-aba3-c699-d6f33205b14f"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("05086698-eecd-2a36-7483-27ad6032805b"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("13aa86e0-a17f-679f-d27b-39d0eca18610"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("15ef207e-9661-34db-373f-49975ba2dee4"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("17d217b3-e15f-c6b0-9aac-10ce7974ae03"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("196f42c8-9986-d647-79b6-6787d7b60150"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("1da01327-c44b-cea8-26b3-fc7a88d44272"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("211015fc-50c4-e649-5733-166abc4b98aa"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("23ace421-f903-1c88-b4c1-acd2adc2ad6d"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("266e4093-ff3e-dce8-5c15-018242515e34"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("3626acf5-7033-ca29-b4b4-12086549cedb"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("3673955b-2b17-e07b-0ee4-a120d4395d53"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("3c1ca1bb-d9c1-61f8-3645-76bc485c05d1"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("4b84144f-0594-c99f-a81d-2cb3ecb034e8"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("656b93a7-5040-e593-bc2a-017ff28354f9"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("6a00dc2b-cbeb-d874-5c4b-e559ddb582e3"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("73ce7494-6d10-2853-c84e-dc9a7f9b2637"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("767f1fc0-1ba4-7a2d-576a-352f788142ab"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("774856af-d174-b7e3-ce13-083119d763eb"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("79c149f7-c0f0-bee9-516b-3aa825de2661"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("7eaedbd3-ce30-a908-e7c8-7de13cbbb3a7"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("7f9435aa-977b-1298-11a6-9de1b591ab17"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("8df70780-75bf-6674-f1f3-3a05292c3aa1"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("9b056158-b76d-ba52-9c81-4c3accf7e261"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("a1cadea3-6b99-6d1c-d169-7d5f2556f7be"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("a6daff80-8852-f23c-41b7-1e8accaf220e"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("aaec7a79-7dd5-229e-d7ad-51ffb466c25d"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("afec596b-3e4f-123e-194a-ae1cba1b3bb5"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("b36e7827-b3be-be30-3470-f4630df519d4"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("bfc8a6f5-e6d6-8bd1-4978-a5134c3ddc1e"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("c5ce6e69-6ea8-e419-d4df-56006eccda0b"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("d9955cc3-1288-55a9-f190-cd4067bb58b8"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("e70c4cd0-6059-c7c7-6a59-ea09e4101e63"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("e87d3e46-ee55-c893-54a3-80b4969fd8b1"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("f0b4d215-484b-098d-7836-94890eb57cc4"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("f5a4aba4-d301-f98e-d4d3-01f1d08d4969"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("faa6c09e-00ec-2d42-302e-294aba2005bb"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("fe1cc2bd-fc34-b430-032c-77dce53c3db7"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("0d876295-827e-d045-3c2a-c6055dff735f"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("18982fad-c120-6be2-9c95-b61832af190a"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("34baba1e-f365-0691-375b-e623a0ebab45"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("38e6df38-cf8a-3ca8-3486-d41f22b08165"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("4ab5517f-c03f-255c-effb-04f483552de7"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("4bc20f2c-303d-3138-7dc5-1c05339856d0"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("50004e1f-f96f-5ffb-e179-f2ba1c60ff61"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("569817b1-37b0-b82c-7779-75cb658548ac"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("755be1d9-1199-386e-f870-d0dc600c08a4"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("7ab5ac54-70fb-0f11-b7f8-e1de35e176b4"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("9031b59d-ad2d-bdda-db83-b22e9629f0ad"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("95a97e73-213b-ffc4-abc3-49e9b20f3b4b"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("98752b81-e8bb-cd81-8e5e-faa29fc2d684"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("9ccd2c87-ce6d-ce18-6ea4-d5106540f089"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("ae1d70bf-ff9a-b25b-2b8f-000dc9bf6199"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("b27a832c-cac7-dcdb-35b5-bd7115e9eff1"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("c666b7ac-7f06-6104-05f8-b1263b9e5172"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("cda3be13-90e5-7c99-1094-f9bba14d719a"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("d6971f6c-ec7f-253e-5c0c-b2279cc943de"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("db2b5881-4a25-e494-4644-24087c6479a8"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("ef332c52-a0dd-dcb8-6ce9-0c991beb9666"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("4a075a89-215b-2263-121b-5f76bc831927"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("4f19ff43-0f41-933e-a868-0bef5ba51e50"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("72547656-0468-4901-9c79-732468ba4340"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("757d2008-8a2d-6356-68d1-fd97b6142fa3"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("79d51817-1186-6a0b-f2dd-96b59d3db416"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("958a065b-2c44-557d-05b4-166a77357ea5"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("bb457d77-dfbc-d450-9c42-c70ad7acf5fe"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("c382ebd6-ca24-f3b2-6e34-5863fad95090"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("f1c90e0b-28cc-c1b3-9dc6-c36dcd5af7cc"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("1c097916-885a-af0f-6fb5-e6087eb1b2dc"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("3842cac4-b9a0-8223-0dcc-509a6f75849b"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("79f67fa8-f3a2-1de7-9181-a67b7542122c"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("7ec8cbec-5c4b-fee2-2830-8fd9f2a7baf3"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("7f3bdae4-cebb-4523-d777-2b0674a318d5"));

            migrationBuilder.DeleteData(
                schema: "accounting",
                table: "Accounts",
                keyColumn: "Id",
                keyValue: new Guid("8d721ec8-4c9d-632f-6f06-7f89cc14862c"));
        }
    }
}
