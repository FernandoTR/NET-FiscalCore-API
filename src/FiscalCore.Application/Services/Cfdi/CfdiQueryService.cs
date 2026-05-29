using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Interfaces.Logging;
using FiscalCore.Application.Interfaces.Message;
using FiscalCore.Domain.Interfaces.Cfdis;
using System.Xml.Linq;

namespace FiscalCore.Application.Services.Cfdi;

public class CfdiQueryService : ICfdiQueryService
{
    private readonly XNamespace _cfdiNs = "http://www.sat.gob.mx/cfd/4";
    private readonly XNamespace _tfdNs = "http://www.sat.gob.mx/TimbreFiscalDigital";
    private readonly ILogService _logService;
    private readonly ICfdiRepository _cfdiRepository;
    private readonly IMessagesProvider _messagesProvider;
    private readonly ICfdiXmlStore _cfdiXmlStore;

    public CfdiQueryService(
        ICfdiRepository cfdiRepository, 
        ILogService logService, 
        IMessagesProvider messagesProvider,
        ICfdiXmlStore cfdiXmlStore)
    {
        _cfdiRepository = cfdiRepository;
        _logService = logService;
        _messagesProvider = messagesProvider;
        _cfdiXmlStore = cfdiXmlStore;
    }

    public async Task<ResponseDto?> GetByUuidAsync(Guid uuid)
    {
        
        try
        {
            var cfdi = await _cfdiRepository.GetByUuidAsync(uuid);

            if (cfdi == null) 
                return ResponseFactory.Error(_messagesProvider.GetMessage("CfdiNotFound"));
            

            var cfdiXml = await _cfdiXmlStore.GetByCfdiIdAsync(cfdi.Id);
            if (cfdiXml == null)
                return ResponseFactory.Error(_messagesProvider.GetMessage("CfdiNotFound"));


            var xmlDocument = XDocument.Parse(cfdiXml.XmlContent);

            var comprobante = xmlDocument.Root!;
            var emisor = comprobante.Element(_cfdiNs + "Emisor");
            var receptor = comprobante.Element(_cfdiNs + "Receptor");
            var timbre = comprobante.Element(_cfdiNs + "Complemento")?.Element(_tfdNs + "TimbreFiscalDigital");
            var impuestosElement = comprobante.Element(_cfdiNs + "Impuestos");

            var entity = new CfdiFullResponse
            {
                UserId = cfdi.UserId,
                Uuid = cfdi.Uuid,
                Folio = comprobante.Attribute("Folio")?.Value,
                Version = comprobante.Attribute("Version")?.Value,
                Serie = comprobante.Attribute("Serie")?.Value,
                Status = cfdi.CurrentStatus,
                Fecha = comprobante.Attribute("Fecha") != null ? DateTime.Parse(comprobante.Attribute("Fecha")!.Value) : DateTime.MinValue,
                FechaTimbrado = timbre.Attribute("FechaTimbrado") != null ? DateTime.Parse(timbre?.Attribute("FechaTimbrado")!.Value) : DateTime.MinValue,
                LugarExpedicion = comprobante.Attribute("LugarExpedicion")?.Value,
                TipoComprobante = comprobante.Attribute("TipoDeComprobante")?.Value,
                NumeroCertificado = timbre.Attribute("NoCertificadoSAT")?.Value,
                NumeroSerie = comprobante.Attribute("NoCertificado")?.Value,
                FormaPago = comprobante.Attribute("FormaPago")?.Value,
                MetodoPago = comprobante.Attribute("MetodoPago")?.Value,
                UsoCFDI = receptor?.Attribute("UsoCFDI")?.Value,
                moneda = comprobante.Attribute("Moneda")?.Value,
                tipoCambio = comprobante.Attribute("TipoCambio")?.Value,
                SubTotal = comprobante.Attribute("SubTotal") != null ? decimal.Parse(comprobante.Attribute("SubTotal")!.Value) : 0,
                Descuento = comprobante.Attribute("Descuento") != null ? decimal.Parse(comprobante.Attribute("Descuento")!.Value) : 0,
                Total = comprobante.Attribute("Total") != null ? decimal.Parse(comprobante.Attribute("Total")!.Value) : 0,
                TotalImpuestosTraslados = impuestosElement.Attribute("TotalImpuestosTrasladados")?.Value != null ? decimal.Parse(impuestosElement.Attribute("TotalImpuestosTrasladados")!.Value) : 0,
                TotalImpuestosRetencion = impuestosElement.Attribute("TotalImpuestosRetenidos")?.Value != null ? decimal.Parse(impuestosElement.Attribute("TotalImpuestosRetenidos")!.Value) : 0,

                EmisorRFC = emisor?.Attribute("Rfc")?.Value,
                EmisorNombre = emisor?.Attribute("Nombre")?.Value,
                EmisorRegimenFiscal = emisor?.Attribute("RegimenFiscal")?.Value,
                ReceptorRFC = receptor?.Attribute("Rfc")?.Value,
                ReceptorNombre = receptor?.Attribute("Nombre")?.Value,
                ReceptoDomicilioFiscal = receptor?.Attribute("DomicilioFiscalReceptor")?.Value,
                SelloCFD = timbre?.Attribute("SelloCFD")?.Value,
                Xml = cfdiXml.XmlContent
            };

            return ResponseFactory.Success<CfdiFullResponse>(entity, _messagesProvider.GetMessage("CfdiRetrievedSuccessfully"));
        }
        catch (Exception ex)
        {
            _logService.ErrorLog($"{nameof(CfdiQueryService)}.{nameof(GetByUuidAsync)}", ex);

            return ResponseFactory.Exception(ex, $"Error al recuperar la información del cfdi: {uuid}");
        }
    }


}
