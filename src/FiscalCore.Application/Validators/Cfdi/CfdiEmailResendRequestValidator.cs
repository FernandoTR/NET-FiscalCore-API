
using FiscalCore.Application.DTOs.Cfdis;
using FluentValidation;

namespace FiscalCore.Application.Validators.Cfdi;

public class CfdiEmailResendRequestValidator : AbstractValidator<CfdiEmailResendRequest>
{
    public CfdiEmailResendRequestValidator()
    {
        RuleFor(x => x.Uuid)
            .NotEmpty()
            .WithMessage("El UUID del CFDI es obligatorio.");

        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage("El correo destino es obligatorio.")
            .EmailAddress()
            .WithMessage("El correo destino no tiene un formato válido.");
    }
}
