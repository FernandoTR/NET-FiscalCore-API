using FiscalCore.Application.DTOs.Cfdis;
using FluentValidation;

namespace FiscalCore.Application.Validators.Cfdi;

public sealed class ConceptoValidator : AbstractValidator<ConceptoDto>
{
    public ConceptoValidator()
    {
        RuleFor(x => x.ClaveProdServ)
            .Length(8)
            .WithMessage("La ClaveProdServ debe contener exactamente 8 caracteres conforme al catálogo c_ClaveProdServ del SAT.");

        RuleFor(x => x.Cantidad)
            .GreaterThan(0)
            .WithMessage("La Cantidad debe ser mayor que cero.");

        RuleFor(x => x.ValorUnitario)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El ValorUnitario debe ser mayor o igual a cero.");

        RuleFor(x => x.Importe)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El Importe debe ser mayor o igual a cero.");

        RuleFor(x => x)
            .Must(c => c.Cantidad * c.ValorUnitario == c.Importe)
            .WithMessage("El Importe debe ser igual al resultado de multiplicar Cantidad por ValorUnitario.");

        RuleFor(x => x.ObjetoImp)
            .Must(x => x is "01" or "02" or "03")
            .WithMessage("El valor de ObjetoImp debe ser 01, 02 o 03 conforme al catálogo c_ObjetoImp del SAT.");

        When(x => x.Impuestos != null, () =>
        {
            RuleFor(x => x.Impuestos)
                .SetValidator(new ConceptoImpuestosValidator())
                .WithMessage("La información de Impuestos del concepto no cumple con la estructura o reglas fiscales aplicables.");
        });
    }
}
