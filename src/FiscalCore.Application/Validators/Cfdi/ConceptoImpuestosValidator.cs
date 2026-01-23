using FiscalCore.Application.DTOs.Cfdis;
using FluentValidation;

namespace FiscalCore.Application.Validators.Cfdi;

public sealed class ConceptoImpuestosValidator : AbstractValidator<ConceptoImpuestosDto>
{
    public ConceptoImpuestosValidator()
    {
        RuleForEach(x => x.Traslados)
            .SetValidator(new TrasladoValidator());

        RuleForEach(x => x.Retenciones)
            .SetValidator(new RetencionValidator());
    }
}
