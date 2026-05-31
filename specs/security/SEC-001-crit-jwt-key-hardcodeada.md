# SEC-001: JWT Signing Key Hardcodeada en Código Fuente

| Campo | Valor |
|-------|-------|
| **ID** | SEC-001 |
| **Severidad** | 🔴 Crítico |
| **OWASP** | A02 — Cryptographic Failures |
| **CWE** | CWE-798 — Use of Hard-coded Credentials |
| **CVSS** | 9.8 (AV:N/AC:L/PR:N/UI:N/S:U/C:H/I:H/A:H) |
| **Archivos afectados** | `src/FiscalCore.Api/Program.cs:79` |
| **Estado** | Implementado |

---

## Descripción

La clave de firma JWT está **incrustada como literal de texto** en `Program.cs`, línea 79:

```csharp
IssuerSigningKey = new SymmetricSecurityKey(
    Encoding.UTF8.GetBytes("CAMBIA_ESTA_CLAVE_LARGA_Y_SEGURA")
)
```

`Program.cs` **NO** lee la clave desde configuración (`IConfiguration` ni `IOptions<JwtOptions>`), a pesar de que `appsettings.json` define `Jwt:SigningKey` y `JwtOptions` está registrado con `ValidateOnStart`.

> ⚠️ `appsettings.json` también contiene el mismo texto débil (`"SigningKey": "CAMBIA_ESTA_CLAVE_LARGA_Y_SEGURA"`), que en español significa literalmente "CAMBIAR ESTA CLAVE".

---

## Riesgo

Cualquiera con acceso al repositorio (público o filtrado) puede:

1. **Firmar tokens JWT arbitrarios** — suplantar cualquier usuario/rol del sistema.
2. **Descifrar tokens existentes** — el token se firma con HMAC-SHA256 (simétrico), por lo que la misma clave sirve para firmar y validar.
3. **Bypass total de autenticación** — acceso completo a todos los endpoints protegidos con `[Authorize]`.
4. El riesgo es **independiente del entorno** — afecta development, staging y producción por igual.

---

## Remediación

### Opción recomendada: Leer desde `IOptions<JwtOptions>`

```diff
// Program.cs — reemplazar bloque de JWT
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
+       var jwtOptions = builder.Configuration
+           .GetSection(JwtOptions.SectionName)
+           .Get<JwtOptions>()
+           ?? throw new InvalidOperationException("JWT options not configured");
+
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
-           ValidIssuer = "FiscalFlow.API",
-           ValidAudience = "FiscalFlow.Client",
+           ValidIssuer = jwtOptions.Issuer,
+           ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
-               Encoding.UTF8.GetBytes("CAMBIA_ESTA_CLAVE_LARGA_Y_SEGURA")
+               Encoding.UTF8.GetBytes(jwtOptions.SigningKey)
            )
        };
    });
```

### Generar clave segura

```powershell
# Generar clave aleatoria de 64+ caracteres
-join ((48..57) + (65..90) + (97..122) | Get-Random -Count 64 | % { [char]$_ })
```

### Almacenar fuera del repositorio

```bash
dotnet user-secrets set "Jwt:SigningKey" "<CLAVE_GENERADA>"
```

Eliminar el valor por defecto de `appsettings.json`:

```diff
  "Jwt": {
    "Issuer": "FiscalCore.API",
    "Audience": "FiscalCore.Client",
-   "SigningKey": "CAMBIA_ESTA_CLAVE_LARGA_Y_SEGURA",
+   "SigningKey": "",
    "ExpirationMinutes": 120
  }
```

---

## Verificación

1. ✅ `Program.cs` ya no contiene el string literal `"CAMBIA_ESTA_CLAVE_LARGA_Y_SEGURA"`
2. ✅ La API arranca sin errores de validación `ValidateOnStart`
3. ✅ `POST /api/v1/auth/login` devuelve token con la nueva clave
4. ✅ Un token firmado con la clave vieja es rechazado (401 Unauthorized)
5. ✅ `appsettings.json` no contiene la clave en texto plano

---

## Referencias

- [CWE-798: Hard-coded Credentials](https://cwe.mitre.org/data/definitions/798.html)
- [Microsoft: Configuration in .NET](https://learn.microsoft.com/en-us/dotnet/core/extensions/configuration)
- [OWASP A02:2021 Cryptographic Failures](https://owasp.org/Top10/A02_2021-Cryptographic_Failures/)
