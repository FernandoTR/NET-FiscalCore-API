using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.Interfaces.Cfdis;
using System.Xml.Linq;

namespace FiscalCore.Infrastructure.CfdiBuilder;

public sealed class CfdiXmlBuilder : ICfdiXmlBuilder
{
    public XDocument Build(CreateCfdiRequest cfdi)
    {
        var comprobante = new XElement(CfdiNamespaces.Cfdi + "Comprobante",
            new XAttribute("Version", "4.0"),
            new XAttribute("Serie", cfdi.Comprobante.Serie),
            new XAttribute("Folio", cfdi.Comprobante.Folio),
            new XAttribute("Fecha", cfdi.Comprobante.Fecha.ToString("yyyy-MM-ddTHH:mm:ss")),
            new XAttribute("FormaPago", cfdi.Comprobante.FormaPago),
            new XAttribute("MetodoPago", cfdi.Comprobante.MetodoPago),
            new XAttribute("Moneda", cfdi.Comprobante.Moneda),
            new XAttribute("SubTotal", cfdi.Comprobante.SubTotal.ToString("F2")),
            new XAttribute("Total", cfdi.Comprobante.Total.ToString("F2")),
            new XAttribute("TipoDeComprobante", cfdi.Comprobante.TipoDeComprobante),
            new XAttribute("Exportacion", cfdi.Comprobante.Exportacion),
            new XAttribute("LugarExpedicion", cfdi.Comprobante.LugarExpedicion),
            new XAttribute(XNamespace.Xmlns + "cfdi", CfdiNamespaces.Cfdi),
            new XAttribute(XNamespace.Xmlns + "xsi", CfdiNamespaces.Xsi),
            new XAttribute(CfdiNamespaces.Xsi + "schemaLocation", CfdiNamespaces.SchemaLocation)

        );

        comprobante.Add(BuildEmisor(cfdi.Emisor));
        comprobante.Add(BuildReceptor(cfdi.Receptor));
        comprobante.Add(BuildConceptos(cfdi.Conceptos));

        var impuestosElement = BuildImpuestos(cfdi.Impuestos);
        if (impuestosElement != null)
        {
            comprobante.Add(impuestosElement);
        }

        return new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            comprobante
        );
    }

    private XElement BuildEmisor(EmisorDto emisor)
    {
        return new XElement(CfdiNamespaces.Cfdi + "Emisor",
            new XAttribute("Rfc", emisor.Rfc),
            new XAttribute("Nombre", emisor.Nombre),
            new XAttribute("RegimenFiscal", emisor.RegimenFiscal)
        );
    }

    private XElement BuildReceptor(ReceptorDto receptor)
    {
        return new XElement(CfdiNamespaces.Cfdi + "Receptor",
            new XAttribute("Rfc", receptor.Rfc),
            new XAttribute("Nombre", receptor.Nombre),
            new XAttribute("DomicilioFiscalReceptor", receptor.DomicilioFiscalReceptor),
            new XAttribute("RegimenFiscalReceptor", receptor.RegimenFiscalReceptor),
            new XAttribute("UsoCFDI", receptor.UsoCFDI)
        );
    }

    private XElement BuildConceptos(List<ConceptoDto> conceptos)
    {
        return new XElement(CfdiNamespaces.Cfdi + "Conceptos",
            conceptos.Select(c =>
            {
                var concepto = new XElement(CfdiNamespaces.Cfdi + "Concepto",
                    new XAttribute("ClaveProdServ", c.ClaveProdServ),
                    new XAttribute("Cantidad", c.Cantidad.ToString("F2")),
                    new XAttribute("ClaveUnidad", c.ClaveUnidad),
                    new XAttribute("Descripcion", c.Descripcion),
                    new XAttribute("ValorUnitario", c.ValorUnitario.ToString("F2")),
                    new XAttribute("Importe", c.Importe.ToString("F2")),
                    new XAttribute("ObjetoImp", c.ObjetoImp)
                );

                // 👇 OBLIGATORIO si ObjetoImp = 02
                if (c.ObjetoImp == "02")
                {
                    var impuestosConcepto = new XElement(CfdiNamespaces.Cfdi + "Impuestos");

                    if (c.Impuestos?.Traslados?.Any() == true)
                    {
                        impuestosConcepto.Add(
                            new XElement(CfdiNamespaces.Cfdi + "Traslados",
                                c.Impuestos?.Traslados.Select(t =>
                                    new XElement(CfdiNamespaces.Cfdi + "Traslado",
                                        new XAttribute("Base", c.Importe.ToString("F2")),
                                        new XAttribute("Impuesto", t.Impuesto),
                                        new XAttribute("TipoFactor", t.TipoFactor),
                                        new XAttribute("TasaOCuota", t.TasaOCuota.ToString("F6")),
                                        new XAttribute("Importe", t.Importe.ToString("F2"))
                                    )
                                )
                            )
                        );
                    }

                    if (c.Impuestos?.Retenciones?.Any() == true)
                    {
                        impuestosConcepto.Add(
                            new XElement(CfdiNamespaces.Cfdi + "Retenciones",
                                c.Impuestos?.Retenciones.Select(r =>
                                    new XElement(CfdiNamespaces.Cfdi + "Retencion",
                                        new XAttribute("Base", c.Importe.ToString("F2")),
                                        new XAttribute("Impuesto", r.Impuesto),
                                        new XAttribute("TipoFactor", r.TipoFactor),
                                        new XAttribute("TasaOCuota", r.TasaOCuota.ToString("F6")),
                                        new XAttribute("Importe", r.Importe.ToString("F2"))
                                    )
                                )
                            )
                        );
                    }

                    concepto.Add(impuestosConcepto);
                }

                return concepto;
            })
        );
    }

    private XElement? BuildImpuestos(ImpuestosDto impuestos)
    {
        if (impuestos == null)
            return null;

        var hasTraslados = impuestos.Traslados?.Any() == true;
        var hasRetenciones = impuestos.Retenciones?.Any() == true;

        if (!hasTraslados && !hasRetenciones)
            return null;

        var impuestosElement = new XElement(CfdiNamespaces.Cfdi + "Impuestos");

        if (hasRetenciones)
        {
            impuestosElement.Add(
                new XAttribute("TotalImpuestosRetenidos",
                    impuestos.TotalImpuestosRetenidos!.ToString("F2"))
            );

            impuestosElement.Add(
                new XElement(CfdiNamespaces.Cfdi + "Retenciones",
                    impuestos.Retenciones!.Select(r =>
                        new XElement(CfdiNamespaces.Cfdi + "Retencion",
                            new XAttribute("Impuesto", r.Impuesto),
                            new XAttribute("Importe", r.Importe.ToString("F2"))
                        )
                    )
                )
            );
        }

        if (hasTraslados)
        {
            impuestosElement.Add(
                new XAttribute("TotalImpuestosTrasladados",
                    impuestos.TotalImpuestosTrasladados!.ToString("F2"))
            );

            impuestosElement.Add(
                new XElement(CfdiNamespaces.Cfdi + "Traslados",
                    impuestos.Traslados!.Select(t =>
                        new XElement(CfdiNamespaces.Cfdi + "Traslado",
                            new XAttribute("Base", t.Base.ToString("F2")),
                            new XAttribute("Impuesto", t.Impuesto),
                            new XAttribute("TipoFactor", t.TipoFactor),
                            new XAttribute("TasaOCuota", t.TasaOCuota.ToString("F6")),
                            new XAttribute("Importe", t.Importe.ToString("F2"))
                        )
                    )
                )
            );
        }

        return impuestosElement;
    }


}
