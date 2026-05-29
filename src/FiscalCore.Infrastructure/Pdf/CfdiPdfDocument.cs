

using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.Interfaces.Cfdis;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Globalization;
using System.Xml.Linq;

namespace FiscalCore.Infrastructure.Pdf;

public sealed class CfdiPdfDocument : IDocument
{
    private readonly XDocument _cfdi;
    private readonly XNamespace _cfdiNs = "http://www.sat.gob.mx/cfd/4";
    private readonly XNamespace _tfdNs = "http://www.sat.gob.mx/TimbreFiscalDigital";
    CultureInfo mxCulture = new CultureInfo("es-MX");
    private readonly IOriginalStringGeneratorService _originalStringGeneratorService;
    private readonly IQrSatService _qrSatService;

    public CfdiPdfDocument(XDocument cfdi, IOriginalStringGeneratorService originalStringGeneratorService, IQrSatService qrSatService)
    {
        _cfdi = cfdi;
        _originalStringGeneratorService = originalStringGeneratorService;
        _qrSatService = qrSatService;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        container.Page(page =>
        {
            page.Size(PageSizes.Letter);
            page.Margin(20);
            page.DefaultTextStyle(x => x.FontSize(9));

            page.Content().Column(col =>
            {
                col.Spacing(8);

                col.Item().Element(Encabezado);
                col.Item().Element(Receptor);
                col.Item().Element(Conceptos);
                col.Item().Element(Totales);
                col.Item().Element(CadenaOriginal);
                col.Item().Element(Sellos);

            });

            page.Footer()
                 .PaddingVertical(5)
                 .AlignCenter()
                 .Text(text =>
                 {
                     text.DefaultTextStyle(x => x.FontSize(8));

                     text.Span("Este documento es una representación impresa de un CFDI");
                     text.Span("  ·  Página ");
                     text.CurrentPageNumber();
                     text.Span(" de ");
                     text.TotalPages();
                 });

        });
    }


    void Encabezado(IContainer container)
    {
        var logoBytes = ResourceImageLoader.Load("logo.png");
        var comprobante = _cfdi.Root!;
        var emisor = comprobante.Element(_cfdiNs + "Emisor");
        var timbre = _cfdi.Root!
            .Element(_cfdiNs + "Complemento")?
            .Element(_tfdNs + "TimbreFiscalDigital");


        container.Row(row =>
        {
            row.ConstantItem(70).Height(70).Image(logoBytes).FitWidth();
            row.ConstantItem(320).PaddingLeft(10).Column(col =>
            {               
                col.Item().Text(emisor?.Attribute("Nombre")?.Value).ExtraBold();
                col.Item().Text($"RFC: {emisor?.Attribute("Rfc")?.Value}");
                col.Item().Text($"Régimen Fiscal: {emisor?.Attribute("RegimenFiscal")?.Value}");
            });

            row.RelativeItem().AlignRight().Column(col =>
            {
                //col.Item().PaddingHorizontal(22).Text($"Factura #{comprobante.Attribute("Folio")?.Value}").Bold().FontSize(12);
                col.Item().Background(Colors.Grey.Lighten1).AlignCenter().Text("FOLIO").SemiBold();
                col.Item().AlignCenter().Text($"{comprobante.Attribute("Folio")?.Value}");
                col.Item().Background(Colors.Grey.Lighten1).AlignCenter().Text("FECHA DE EMISION").SemiBold();
                col.Item().AlignCenter().Text($"{comprobante.Attribute("Fecha")?.Value}");
                col.Item().Background(Colors.Grey.Lighten1).AlignCenter().Text("FECHA DE CERTIFICACIÓN").SemiBold();
                col.Item().AlignCenter().Text($"{timbre?.Attribute("FechaTimbrado")?.Value}");
                col.Item().Background(Colors.Grey.Lighten1).AlignCenter().Text("LUGAR DE EXPEDICIÓN").SemiBold();
                col.Item().AlignCenter().Text($"{comprobante.Attribute("LugarExpedicion")?.Value}");
                col.Item().Background(Colors.Grey.Lighten1).AlignCenter().Text("TIPO DE COMPROBANTE").SemiBold();
                col.Item().AlignCenter().Text($"{comprobante.Attribute("TipoDeComprobante")?.Value}");
            });
        });
    }
    void Receptor(IContainer container)
    {
        var receptor = _cfdi.Root!.Element(_cfdiNs + "Receptor");
        var timbre = _cfdi.Root!
            .Element(_cfdiNs + "Complemento")?
            .Element(_tfdNs + "TimbreFiscalDigital");
        var comprobante = _cfdi.Root!;

        container.Row(row =>
        {
            row.ConstantItem(350).Border(1).BorderColor(Colors.Grey.Lighten1).Column(col =>
            {
                var padding = 5;
                col.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten1).Padding(5).Text("Receptor").Bold();
                col.Item().PaddingLeft(padding).PaddingTop(padding).Text(receptor?.Attribute("Nombre")?.Value).SemiBold();
                col.Item().PaddingLeft(padding).Text($"{receptor?.Attribute("Rfc")?.Value}").SemiBold();
                col.Item().PaddingLeft(padding).Text($"{receptor?.Attribute("DomicilioFiscalReceptor")?.Value}").SemiBold();
                col.Item().PaddingLeft(padding).Text($"{receptor?.Attribute("RegimenFiscalReceptor")?.Value}").SemiBold();
                col.Item().PaddingLeft(padding).PaddingBottom(padding).Text($"{receptor?.Attribute("UsoCFDI")?.Value}").SemiBold();
            });

            row.RelativeItem().BorderColor(Colors.Grey.Lighten1).PaddingLeft(0).AlignRight().Column(col =>
            {
                col.Item().Background(Colors.Grey.Lighten1).AlignCenter().PaddingHorizontal(80).Text("Folio Fiscal").SemiBold();
                col.Item().PaddingBottom(5).PaddingTop(5).AlignCenter().Text($"{timbre.Attribute("UUID")?.Value}");
                col.Item().Background(Colors.Grey.Lighten1).AlignCenter().Text("No. Serie Certificado SAT").SemiBold();
                col.Item().PaddingBottom(5).PaddingTop(5).AlignCenter().Text($"{timbre.Attribute("NoCertificadoSAT")?.Value}");
                col.Item().Background(Colors.Grey.Lighten1).AlignCenter().Text("No. de Serie del CSD").SemiBold();
                col.Item().PaddingBottom(5).PaddingTop(5).AlignCenter().Text($"{comprobante.Attribute("NoCertificado")?.Value}");
            });
        });
    }
    void Conceptos(IContainer container)
    {        
        var conceptos = _cfdi.Root!.Element(_cfdiNs + "Conceptos")!.Elements(_cfdiNs + "Concepto");

        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(50);
                columns.ConstantColumn(50);
                columns.ConstantColumn(50);
                columns.RelativeColumn();
                columns.ConstantColumn(60);
                columns.ConstantColumn(60);
                columns.ConstantColumn(60);
            });

            table.Header(header =>
            {                
                header.Cell().Element(EstiloCelda).AlignCenter().AlignMiddle().Text("Cantidad").Bold();
                header.Cell().Element(EstiloCelda).AlignCenter().AlignMiddle().Text("Clave Unidad").Bold();
                header.Cell().Element(EstiloCelda).AlignCenter().AlignMiddle().Text("ClaveProd Serv").Bold();
                header.Cell().Element(EstiloCelda).AlignCenter().AlignMiddle().Text("Descripción").Bold();
                header.Cell().Element(EstiloCelda).AlignRight().AlignMiddle().Text("Precio").Bold();
                header.Cell().Element(EstiloCelda).AlignRight().AlignMiddle().Text("Descuento").Bold();
                header.Cell().Element(EstiloCelda).AlignRight().AlignMiddle().PaddingRight(5).Text("Importe").Bold();

                static IContainer EstiloCelda(IContainer contenedor)
                {
                    return contenedor.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten1);
                }
            });

            foreach (var c in conceptos)
            {
                var impuestos = c.Element(_cfdiNs + "Impuestos");
                int border = impuestos == null ? 1 : 0;

                decimal valorUnitario = ParseDecimal(c, "ValorUnitario");
                decimal descuento = ParseDecimal(c, "Descuento");
                decimal importe = ParseDecimal(c, "Importe");

                // ===== Línea del concepto =====
                table.Cell().BorderBottom(border).BorderColor(Colors.Grey.Lighten1).AlignCenter()
                    .Text(c.Attribute("Cantidad")?.Value);

                table.Cell().BorderBottom(border).BorderColor(Colors.Grey.Lighten1).AlignCenter()
                    .Text(c.Attribute("ClaveUnidad")?.Value);

                table.Cell().BorderBottom(border).BorderColor(Colors.Grey.Lighten1).AlignCenter()
                    .Text(c.Attribute("ClaveProdServ")?.Value);

                table.Cell().BorderBottom(border).BorderColor(Colors.Grey.Lighten1)
                    .Text(c.Attribute("Descripcion")?.Value);

                table.Cell().BorderBottom(border).BorderColor(Colors.Grey.Lighten1).AlignRight()
                    .Text(valorUnitario.ToString("C2", mxCulture));

                table.Cell().BorderBottom(border).BorderColor(Colors.Grey.Lighten1).AlignRight()
                    .Text(descuento.ToString("C2", mxCulture));

                table.Cell().BorderBottom(border).BorderColor(Colors.Grey.Lighten1).AlignRight()
                    .Text(importe.ToString("C2", mxCulture));

                // ===== Impuestos por línea =====
                if (impuestos == null)
                    continue;

                var traslados = impuestos
                    .Element(_cfdiNs + "Traslados")?
                    .Elements(_cfdiNs + "Traslado") ?? Enumerable.Empty<XElement>();

                var retenciones = impuestos
                    .Element(_cfdiNs + "Retenciones")?
                    .Elements(_cfdiNs + "Retencion") ?? Enumerable.Empty<XElement>();

               
                foreach (var t in traslados)
                {
                    EmptyCells(table, 3, 0);

                    table.Cell()
                        .Text($"Traslado - {t.Attribute("Impuesto")?.Value}  Base: {ParseFormatMoney(t, "Base")}  {t.Attribute("TipoFactor")?.Value}: {Math.Round(ParseDecimal(t, "TasaOCuota"), 2)}%  Importe: {ParseFormatMoney(t, "Importe")}");

                    EmptyCells(table, 3, 0);
                }

                foreach (var r in retenciones)
                {
                    EmptyCells(table, 3, 0);

                    table.Cell()
                       .Text($"Retención - {r.Attribute("Impuesto")?.Value}  Base: {ParseFormatMoney(r, "Base")}  {r.Attribute("TipoFactor")?.Value}: {Math.Round(ParseDecimal(r, "TasaOCuota"), 2)}%  Importe: {ParseFormatMoney(r, "Importe")}");
                   
                    EmptyCells(table, 3, 0);
                }

                EmptyCells(table, 7, 1);
            }

        });

        decimal ParseDecimal(XElement element, string attributeName)
        {
            return decimal.Parse(
                element.Attribute(attributeName)?.Value ?? "0",
                CultureInfo.InvariantCulture
            );
        }

        string ParseFormatMoney(XElement element, string attributeName)
        {
            return decimal.Parse(
                element.Attribute(attributeName)?.Value ?? "0",
                CultureInfo.InvariantCulture
            ).ToString("C2", mxCulture);
        }

        void EmptyCells(TableDescriptor table, int count, int border)
        {
            for (int i = 0; i < count; i++)
            {
                table.Cell()
                    .BorderBottom(border)
                    .BorderColor(Colors.Grey.Lighten1)
                    .Text("");
            }
        }
    }
    void Totales(IContainer container)
    {
        var comprobante = _cfdi.Root!;
        var impuestosElement = _cfdi.Root!.Element(_cfdiNs + "Impuestos");


        decimal subtotal = decimal.Parse(comprobante.Attribute("SubTotal")?.Value ?? "0", CultureInfo.InvariantCulture);
        decimal descuento = decimal.Parse(comprobante.Attribute("Descuento")?.Value ?? "0", CultureInfo.InvariantCulture);
        decimal retencion = decimal.Parse(impuestosElement.Attribute("TotalImpuestosRetenidos")?.Value ?? "0", CultureInfo.InvariantCulture);
        decimal impuesto = decimal.Parse(impuestosElement.Attribute("TotalImpuestosTrasladados")?.Value ?? "0", CultureInfo.InvariantCulture);

        decimal total = decimal.Parse(comprobante.Attribute("Total")?.Value ?? "0", CultureInfo.InvariantCulture);
        string totalEnLetra = NumeroALetraConverter.Convertir(total);

        container.Row(row =>
        {
            row.RelativeItem().Border(1).BorderColor(Colors.Grey.Lighten1).Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(100);     // titulo
                    columns.RelativeColumn();   // descripcion
                });

                // ===== Forma de pago =====
                table.Cell().BorderLeft(1).BorderColor(Colors.Grey.Lighten1).Padding(5).Text("Forma de pago: ").Bold();
                table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignLeft().Text($"{comprobante.Attribute("FormaPago")?.Value}");

                // ===== Método de pago =====
                table.Cell().BorderLeft(1).BorderColor(Colors.Grey.Lighten1).Padding(5).Text("Método de pago: ").Bold();
                table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignLeft().Text($"{comprobante.Attribute("MetodoPago")?.Value}");

                // ===== Condiciones de pago =====
                //table.Cell().BorderLeft(1).BorderColor(Colors.Grey.Lighten1).Padding(5).Text("Condiciones de pago: ").Bold();
                //table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignLeft().Text($"{comprobante.Attribute("FormaPago")?.Value}");

                // ===== Importe con letra =====
                table.Cell().BorderLeft(1).BorderColor(Colors.Grey.Lighten1).Padding(5).Text("Importe con letra: ").Bold();
                table.Cell().BorderRight(1).BorderColor(Colors.Grey.Lighten1).Padding(5).AlignLeft().Text($"{totalEnLetra}");                
            });
            


            row.ConstantItem(150).PaddingLeft(5).AlignRight().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();     // Concepto
                    columns.ConstantColumn(60);   // Importe
                });

                // ===== Header =====
                table.Cell().ColumnSpan(2)
                    .BorderLeft(1).BorderRight(1).BorderBottom(1)
                    .BorderColor(Colors.Grey.Lighten1)
                    .Background(Colors.Grey.Lighten1)
                    .PaddingVertical(5)
                    .AlignCenter()
                    .Text("TOTALES")
                    .Bold();

                // ===== Subtotal =====
                AddTotalRow(table, "Subtotal", subtotal, mxCulture);

                // ===== Descuento =====
                AddTotalRow(table, "Descuento", descuento, mxCulture);                  

                // ===== Impuestos =====
                AddTotalRow(table, "Imp. Trasladados", impuesto, mxCulture);

                // ===== Retenciones =====
                AddTotalRow(table, "Imp. Retenidos", retencion, mxCulture);

                // ===== Total =====
                table.Cell()
                       .BorderLeft(1).BorderBottom(1)
                       .BorderColor(Colors.Grey.Lighten1)
                       .Padding(5)
                       .Text("Total")
                       .Bold();

                   table.Cell()
                       .BorderRight(1).BorderBottom(1)
                       .BorderColor(Colors.Grey.Lighten1)
                       .Padding(5)
                       .AlignRight()
                       .Text(total.ToString("C2", mxCulture))
                       .Bold();
            });

            void AddTotalRow(TableDescriptor table, string label, decimal amount, CultureInfo culture)
            {
                table.Cell()
                    .BorderLeft(1)
                    .BorderColor(Colors.Grey.Lighten1)
                    .Padding(5)
                    .Text(label);

                table.Cell()
                    .BorderRight(1)
                    .BorderColor(Colors.Grey.Lighten1)
                    .Padding(5)
                    .AlignRight()
                    .Text(amount.ToString("C2", culture));
            }


        });
    }
    void CadenaOriginal(IContainer container)
    {
        var comprobante = _cfdi.Root!;

        #region Regenerar cadena original
        var originalStringGeneratorResult = _originalStringGeneratorService.Generate(_cfdi);
        var cadenaOriginal = ((ResponseSuccessDto<string>)originalStringGeneratorResult).Data;
        #endregion


        container.Border(1).BorderColor(Colors.Grey.Lighten1).Column(col =>
        {
            col.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten1).Padding(5).Text("Cadena Original del Complemento de Certificación Digital del SAT").Bold();
            col.Item().Padding(5).Text($"{cadenaOriginal}").FontSize(7);
        });       

    }
    void Sellos(IContainer container)
    {
        var timbre = _cfdi.Root!
            .Element(_cfdiNs + "Complemento")?
            .Element(_tfdNs + "TimbreFiscalDigital");

        var qrUrl = _qrSatService.BuildQrUrl(_cfdi);
        var qrBytes = _qrSatService.GenerateQrImage(qrUrl);

        container.Row(row =>
        {
            row.ConstantItem(130).Height(130).Image(qrBytes).FitWidth();
            row.RelativeItem().PaddingLeft(10).Column(col =>
            {
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten1).Padding(5).Text("Sello digital del CFDI").Bold();
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(5).Text($"{timbre?.Attribute("SelloCFD")?.Value}").FontSize(7);
                col.Item().Text("");
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Background(Colors.Grey.Lighten1).Padding(5).Text("Sello Digital del SAT").Bold();
                col.Item().Border(1).BorderColor(Colors.Grey.Lighten1).Padding(5).Text($"{timbre?.Attribute("SelloSAT")?.Value}").FontSize(7);
            });

            
        });

    }


}
