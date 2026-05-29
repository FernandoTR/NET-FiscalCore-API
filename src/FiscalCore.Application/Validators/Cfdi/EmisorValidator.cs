using FiscalCore.Application.DTOs.Cfdis;
using FluentValidation;

namespace FiscalCore.Application.Validators.Cfdi;

public sealed class EmisorValidator : AbstractValidator<EmisorDto>
{
    public EmisorValidator()
    {
        RuleFor(x => x.Rfc)
            .Matches(CfdiRegex.RfcFisica)
            .When(x => x.Rfc.Length == 13)
            .WithMessage("El RFC del emisor no cumple con el formato válido para persona física.")
            .Matches(CfdiRegex.RfcMoral)
            .When(x => x.Rfc.Length == 12)
            .WithMessage("El RFC del emisor no cumple con el formato válido para persona moral.");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre del emisor es obligatorio conforme al CFDI 4.0.")
            .MaximumLength(254)
            .WithMessage("El nombre del emisor no debe exceder de 254 caracteres.");

        RuleFor(x => x.RegimenFiscal)
            .Matches(@"^\d{3}$")
            .WithMessage("El régimen fiscal del emisor debe contener exactamente 3 dígitos conforme al catálogo c_RegimenFiscal.");
    }
}
