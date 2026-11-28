# sbcapi — SBC (DataFlowHub API)

API REST de contabilidad general construida sobre **.NET 10**, **Entity Framework Core** (SQL Server) y **ASP.NET Core Identity + JWT**.

### Documentación

- **[Manual de Instalación](docs/MANUAL_INSTALACION.md)** — Prerrequisitos, configuración de SQL Server y secretos JWT (`JwtSettings:SecretKey`), migraciones EF Core, datos semilla, pruebas, ejecución local y despliegue con Docker.
- **[Manual de Usuario](docs/MANUAL_USUARIO.md)** — Guía funcional y referencia completa de los 10 módulos (40 endpoints), reglas contables, carga masiva Excel, reportes financieros (JSON/Excel/PDF), tutorial mensual y solución de errores.

### Secrets

Para generar el secret de token JWT (`JwtSettings:SecretKey`) se puede utilizar **openssl** (Linux/macOS):

```shell
  openssl rand -base64 32
```

En **Windows (PowerShell 5.1 y 7+)** sin OpenSSL:

```powershell
$bytes = New-Object byte[] 32; $rng = [System.Security.Cryptography.RandomNumberGenerator]::Create(); $rng.GetBytes($bytes); $rng.Dispose(); [Convert]::ToBase64String($bytes)
```
