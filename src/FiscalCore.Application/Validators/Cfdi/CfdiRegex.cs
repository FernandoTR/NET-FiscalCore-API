namespace FiscalCore.Application.Validators.Cfdi;

internal class CfdiRegex
{
    public const string RfcFisica = @"^[A-Z&Ñ]{4}\d{6}[A-Z0-9]{3}$";
    public const string RfcMoral = @"^[A-Z&Ñ]{3}\d{6}[A-Z0-9]{3}$";
    public const string CodigoPostal = @"^\d{5}$";
}
