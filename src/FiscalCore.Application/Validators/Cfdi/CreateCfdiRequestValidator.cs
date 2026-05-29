using FiscalCore.Application.DTOs.Cfdis;
using FluentValidation;

namespace FiscalCore.Application.Validators.Cfdi;

public sealed class CreateCfdiRequestValidator : AbstractValidator<CreateCfdiRequest>
{
    public CreateCfdiRequestValidator()
    {
        RuleFor(x => x.Comprobante)
            .NotNull()
            .SetValidator(new ComprobanteValidator());

        RuleFor(x => x.Emisor)
            .NotNull()
            .SetValidator(new EmisorValidator());

        RuleFor(x => x.Receptor)
            .NotNull()
            .SetValidator(new ReceptorValidator());

        RuleFor(x => x.Conceptos)
            .NotEmpty();

        RuleForEach(x => x.Conceptos)
            .SetValidator(new ConceptoValidator());

        RuleFor(x => x)
            .Must(TotalesCuadran)
            .WithMessage("El total no coincide con subtotal, descuentos e impuestos.");
    }

    private bool TotalesCuadran(CreateCfdiRequest req)
    {
        var subtotalConceptos = req.Conceptos.Sum(c => c.Importe);
        if (subtotalConceptos != req.Comprobante.SubTotal)
            return false;

        var impuestosTrasladados = req.Impuestos?.Traslados?.Sum(t => t.Importe) ?? 0;
        var impuestosRetenidos = req.Impuestos?.Retenciones?.Sum(r => r.Importe) ?? 0;

        var totalCalculado =
            req.Comprobante.SubTotal
            - req.Comprobante.Descuento
            + impuestosTrasladados
            - impuestosRetenidos;

        return totalCalculado == req.Comprobante.Total;
    }
}

