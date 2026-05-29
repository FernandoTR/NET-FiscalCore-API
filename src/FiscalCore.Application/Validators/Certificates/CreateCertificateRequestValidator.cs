using FiscalCore.Application.DTOs.Certificate;
using FluentValidation;

namespace FiscalCore.Application.Validators.Certificates;

public class CreateCertificateRequestValidator : AbstractValidator<CreateCertificateRequest>
{
    public CreateCertificateRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("El ID de usuario es obligatorio.");

        RuleFor(x => x.Rfc)
            .NotEmpty().WithMessage("El RFC es obligatorio.")
            .MinimumLength(12).WithMessage("La longitud del RFC debe tener al menos 12 caracteres.")
            .MaximumLength(13).WithMessage("La longitud del RFC no es válida.");

        RuleFor(x => x.SerialNumber)
            .NotEmpty().WithMessage("El número de serie es obligatorio.")
            .MaximumLength(100).WithMessage("La longitud del número de serie no es válida.");

        RuleFor(x => x.CertificateType)
            .NotEmpty().WithMessage("El tipo de certificado es obligatorio.")
            .MaximumLength(50).WithMessage("La longitud del tipo de certificado no es válida");

        RuleFor(x => x.ValidFrom)
            .NotEmpty().WithMessage("Se requiere fecha de validez.");

        RuleFor(x => x.ValidTo)
            .NotEmpty().WithMessage("Se requiere fecha de validez.")
            .GreaterThan(x => x.ValidFrom).WithMessage("ValidTo debe ser mayor que ValidFrom.");

        RuleFor(x => x.CerFile)
            .NotNull().WithMessage("Se requiere el archivo CER.")
            .Must(x => x.Length > 0).WithMessage("El archivo CER no puede estar vacío.");

        RuleFor(x => x.KeyFile)
            .NotNull().WithMessage("Se requiere el archivo KEY.")
            .Must(x => x.Length > 0).WithMessage("El archivo KEY no puede estar vacío.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("La contraseña es requerida")
            .MinimumLength(8).WithMessage("La contraseña debe tener al menos 8 caracteres.");
    }
}
