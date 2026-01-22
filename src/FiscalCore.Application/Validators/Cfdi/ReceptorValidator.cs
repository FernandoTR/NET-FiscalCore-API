using FiscalCore.Application.DTOs.Cfdis;
using FluentValidation;

namespace FiscalCore.Application.Validators.Cfdi;

public sealed class ReceptorValidator : AbstractValidator<ReceptorDto>
{
    public ReceptorValidator()
    {
        RuleFor(x => x.Rfc)
            .NotEmpty()
            .WithMessage("El RFC del receptor es obligatorio conforme al Anexo 20 del CFDI 4.0.");

        RuleFor(x => x.DomicilioFiscalReceptor)
            .Matches(CfdiRegex.CodigoPostal)
            .WithMessage("El Domicilio Fiscal del receptor debe contener un código postal válido de 5 dígitos conforme al catálogo c_CodigoPostal del SAT.");

        RuleFor(x => x.RegimenFiscalReceptor)
            .Matches(@"^\d{3}$")
            .WithMessage("El Régimen Fiscal del receptor debe contener una clave válida de 3 dígitos conforme al catálogo c_RegimenFiscal del SAT.");

        RuleFor(x => x.UsoCFDI)
            .Length(3)
            .WithMessage("El Uso del CFDI debe contener una clave válida de 3 caracteres conforme al catálogo c_UsoCFDI del SAT.");
    }
}
