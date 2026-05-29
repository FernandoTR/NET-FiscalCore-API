using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Interfaces.Message;
using FiscalCore.Application.Interfaces.SatCatalog;

namespace FiscalCore.Application.Services.Cfdi;

public sealed class CfdiFiscalRulesValidatorService : ICfdiFiscalRulesValidatorService
{
    private readonly ISatCatalogStore _satCatalogStore;
    private readonly IMessagesProvider _messagesProvider;

    public CfdiFiscalRulesValidatorService(ISatCatalogStore satCatalogStore, IMessagesProvider messagesProvider)
    {
        _satCatalogStore = satCatalogStore;
        _messagesProvider = messagesProvider;
    }

    public async Task<ResponseDto> ExecuteAsync(CreateCfdiRequest request, CancellationToken ct)
    {
        var errors = new List<ResponseErrorDetailDto>();

        await ValidateComprobanteAsync(request, errors, ct);
        await ValidateEmisorAsync(request, errors, ct);
        await ValidateReceptorAsync(request, errors, ct);
        await ValidateConceptosAsync(request, errors, ct);
        await ValidateImpuestosAsync(request, errors, ct);

        return errors.Any()
          ? ResponseFactory.Error(_messagesProvider.GetError("FiscalValidationFailed"), errors)
          : ResponseFactory.Success("Ha superado satisfactoriamente las validaciones fiscales.");
    }

    private async Task ValidateComprobanteAsync(CreateCfdiRequest dto, List<ResponseErrorDetailDto> errors, CancellationToken ct)
    {

        if (!await _satCatalogStore.CatalogItemExistsAsync("c_FormaPago", dto.Comprobante.FormaPago, ct))
            errors.Add(new ResponseErrorDetailDto
            {
                Field = "Comprobante.FormaPago",
                Message = string.Format(_messagesProvider.GetError("CatalogKeyNotFound"), dto.Comprobante.FormaPago, "c_FormaPago")
            });


        if (!await _satCatalogStore.CatalogItemExistsAsync("c_MetodoPago", dto.Comprobante.MetodoPago, ct))
            errors.Add(new ResponseErrorDetailDto
            {
                Field = "Comprobante.MetodoPago",
                Message = string.Format(_messagesProvider.GetError("CatalogKeyNotFound"), dto.Comprobante.MetodoPago, "c_MetodoPago")
            });


        if (!await _satCatalogStore.IsRuleAllowedAsync(
                "c_FormaPago", dto.Comprobante.FormaPago,
                "c_MetodoPago", dto.Comprobante.MetodoPago, ct))
        {
            errors.Add(new ResponseErrorDetailDto
            {
                Field = "Comprobante.MetodoPago",
                Message = string.Format(_messagesProvider.GetError("InvalidCatalogCombination"), "FormaPago", dto.Comprobante.FormaPago, "MetodoPago", dto.Comprobante.MetodoPago)
            });
        }

        if (!await _satCatalogStore.CatalogItemExistsAsync("c_Moneda", dto.Comprobante.Moneda, ct))
            errors.Add(new ResponseErrorDetailDto
            {
                Field = "Comprobante.Moneda",
                Message = string.Format(_messagesProvider.GetError("CatalogKeyNotFound"), dto.Comprobante.Moneda, "c_Moneda")
            });

        if (!await _satCatalogStore.CatalogItemExistsAsync("c_TipoDeComprobante", dto.Comprobante.TipoDeComprobante, ct))
            errors.Add(new ResponseErrorDetailDto
            {
                Field = "Comprobante.TipoDeComprobante",
                Message = string.Format(_messagesProvider.GetError("CatalogKeyNotFound"), dto.Comprobante.TipoDeComprobante, "c_TipoDeComprobante")
            });


    }

    private async Task ValidateEmisorAsync(CreateCfdiRequest dto, List<ResponseErrorDetailDto> errors, CancellationToken ct)
    {
        if (!await _satCatalogStore.CatalogItemExistsAsync("c_RegimenFiscal", dto.Emisor.RegimenFiscal, ct))
            errors.Add(new ResponseErrorDetailDto
            {
                Field = "Emisor.RegimenFiscal",
                Message = string.Format(_messagesProvider.GetError("CatalogKeyNotFound"), dto.Emisor.RegimenFiscal, "c_RegimenFiscal")
            });
    }

    private async Task ValidateReceptorAsync(CreateCfdiRequest dto, List<ResponseErrorDetailDto> errors, CancellationToken ct)
    {
        if (!await _satCatalogStore.CatalogItemExistsAsync("c_UsoCFDI", dto.Receptor.UsoCFDI, ct))
            errors.Add(new ResponseErrorDetailDto
            {
                Field = "Receptor.UsoCFDI",
                Message = string.Format(_messagesProvider.GetError("CatalogKeyNotFound"), dto.Receptor.UsoCFDI, "c_UsoCFDI")
            });

        if (!await _satCatalogStore.CatalogItemExistsAsync("c_RegimenFiscal", dto.Receptor.RegimenFiscalReceptor, ct))
            errors.Add(new ResponseErrorDetailDto
            {
                Field = "Receptor.RegimenFiscalReceptor",
                Message = string.Format(_messagesProvider.GetError("CatalogKeyNotFound"), dto.Receptor.RegimenFiscalReceptor, "c_RegimenFiscal")
            });


        //if (!await _satCatalogStore.IsRuleAllowedAsync(
        //        "c_RegimenFiscal",
        //        dto.Receptor.RegimenFiscalReceptor,
        //        "c_UsoCFDI",
        //        dto.Receptor.UsoCFDI,
        //        ct))
        //{
        //    errors.Add(new CfdiErrorDetailDto
        //    {
        //        Field = "Receptor.UsoCFDI",
        //        Message = string.Format(_messagesProvider.GetError("InvalidCatalogCombination"), "UsoCFDI", dto.Receptor.UsoCFDI, "RegimenFiscalReceptor", dto.Receptor.RegimenFiscalReceptor)
        //    });
        //}
    }

    private async Task ValidateConceptosAsync(CreateCfdiRequest dto, List<ResponseErrorDetailDto> errors, CancellationToken ct)
    {
        foreach (var (concepto, index) in dto.Conceptos.Select((c, i) => (c, i)))
        {
            if (!await _satCatalogStore.CatalogItemExistsAsync("c_ClaveProdServ", concepto.ClaveProdServ, ct))
                errors.Add(new ResponseErrorDetailDto
                {
                    Field = $"Conceptos[{index}].ClaveProdServ",
                    Message = string.Format(_messagesProvider.GetError("CatalogKeyNotFound"), concepto.ClaveProdServ, "c_ClaveProdServ")
                });

            if (!await _satCatalogStore.CatalogItemExistsAsync("c_ClaveUnidad", concepto.ClaveUnidad, ct))
                errors.Add(new ResponseErrorDetailDto
                {
                    Field = $"Conceptos[{index}].ClaveUnidad",
                    Message = string.Format(_messagesProvider.GetError("CatalogKeyNotFound"), concepto.ClaveUnidad, "c_ClaveUnidad")
                });

            if (!await _satCatalogStore.CatalogItemExistsAsync("c_ObjetoImp", concepto.ObjetoImp, ct))
                errors.Add(new ResponseErrorDetailDto
                {
                    Field = $"Conceptos[{index}].ObjetoImp",
                    Message = string.Format(_messagesProvider.GetError("CatalogKeyNotFound"), concepto.ObjetoImp, "c_ObjetoImp")
                });

        }
    }

    private async Task ValidateImpuestosAsync(CreateCfdiRequest dto, List<ResponseErrorDetailDto> errors, CancellationToken ct)
    {
        foreach (var traslado in dto.Impuestos?.Traslados ?? Enumerable.Empty<TrasladoGlobalDto>())
        {
            if (!await _satCatalogStore.CatalogItemExistsAsync("c_Impuesto", traslado.Impuesto, ct))
                errors.Add(new ResponseErrorDetailDto
                {
                    Field = $"Impuestos.Traslados.Impuesto",
                    Message = string.Format(_messagesProvider.GetError("CatalogKeyNotFound"), traslado.Impuesto, "c_Impuesto")
                });

            if (!await _satCatalogStore.CatalogItemExistsAsync("c_TipoFactor", traslado.TipoFactor, ct))
                errors.Add(new ResponseErrorDetailDto
                {
                    Field = $"Impuestos.Traslados.TipoFactor",
                    Message = string.Format(_messagesProvider.GetError("CatalogKeyNotFound"), traslado.TipoFactor, "c_TipoFactor")
                });
        }
    }


}
