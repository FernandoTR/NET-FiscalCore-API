using FiscalCore.Application.DTOs.Cfdis;
using FluentValidation;

namespace FiscalCore.Application.Validators.Cfdi;

public sealed class ComprobanteValidator : AbstractValidator<ComprobanteDto>
{
    public ComprobanteValidator()
    {
        RuleFor(x => x.Version)
            .Must(x => x is "4.0")
            .WithMessage("La versión del comprobante debe ser 4.0 conforme al estándar CFDI vigente.");


        RuleFor(x => x.Serie)
            .MaximumLength(25)
            .WithMessage("La serie del comprobante no debe exceder 25 caracteres.");


        RuleFor(x => x.Folio)
            .MaximumLength(40)
            .WithMessage("El folio del comprobante no debe exceder 40 caracteres.");

        RuleFor(x => x.Fecha)
            .LessThanOrEqualTo(DateTime.UtcNow.AddMinutes(5))
            .WithMessage("La fecha de expedición no puede ser mayor al momento actual.");

        RuleFor(x => x.FormaPago)
            .Matches(@"^\d{2}$")
            .WithMessage("La forma de pago debe contener exactamente dos dígitos conforme al catálogo del SAT.");

        RuleFor(x => x.MetodoPago)
            .Must(x => x is "PUE" or "PPD")
            .WithMessage("El método de pago debe ser PUE (Pago en una sola exhibición) o PPD (Pago en parcialidades o diferido).");

        RuleFor(x => x.Moneda)
            .Length(3)
            .WithMessage("La moneda debe expresarse con una clave de tres caracteres conforme al catálogo del SAT.");

        RuleFor(x => x.SubTotal)
            .GreaterThan(0)
            .WithMessage("El subtotal del comprobante debe ser mayor a cero.");

        RuleFor(x => x.Descuento)
            .GreaterThanOrEqualTo(0)
            .WithMessage("El descuento, en caso de existir, debe ser mayor o igual a cero.");

        RuleFor(x => x.Total)
            .GreaterThan(0)
            .WithMessage("El total del comprobante debe ser mayor a cero.");

        RuleFor(x => x.TipoDeComprobante)
            .Must(x => x is "I" or "E" or "T" or "N" or "P")
            .WithMessage("El tipo de comprobante debe ser I (Ingreso), E (Egreso), T (Traslado), N (Nómina) o P (Pago).");

        RuleFor(x => x.Exportacion)
            .Must(x => x is "01" or "02" or "03");

        RuleFor(x => x.LugarExpedicion)
            .Matches(CfdiRegex.CodigoPostal)
            .WithMessage("El lugar de expedición debe contener un código postal válido conforme al catálogo del SAT.");
    }
}
