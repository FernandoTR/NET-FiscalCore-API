
using System.Text;

namespace FiscalCore.Infrastructure.Pdf;

public static class NumeroALetraConverter
{
    public static string Convertir(decimal importe)
    {
        importe = Math.Round(importe, 2, MidpointRounding.AwayFromZero);

        long parteEntera = (long)Math.Floor(importe);
        int centavos = (int)((importe - parteEntera) * 100);

        string letras = ConvertirEntero(parteEntera);

        string moneda = parteEntera == 1 ? "PESO" : "PESOS";

        return $"{letras} {moneda} {centavos:00}/100 MXN";
    }

    private static string ConvertirEntero(long numero)
    {
        if (numero == 0)
            return "CERO";

        if (numero < 0)
            return "MENOS " + ConvertirEntero(Math.Abs(numero));

        var sb = new StringBuilder();

        if (numero >= 1_000_000)
        {
            sb.Append(ConvertirEntero(numero / 1_000_000));
            sb.Append(numero / 1_000_000 == 1 ? " MILLÓN " : " MILLONES ");
            numero %= 1_000_000;
        }

        if (numero >= 1_000)
        {
            sb.Append(numero / 1_000 == 1 ? "MIL " : ConvertirEntero(numero / 1_000) + " MIL ");
            numero %= 1_000;
        }

        if (numero >= 100)
        {
            sb.Append(Centenas(numero / 100));
            numero %= 100;
        }

        if (numero > 0)
        {
            sb.Append(Decenas(numero));
        }

        return sb.ToString().Trim();
    }

    private static string Centenas(long numero)
    {
        return numero switch
        {
            1 => "CIENTO ",
            2 => "DOSCIENTOS ",
            3 => "TRESCIENTOS ",
            4 => "CUATROCIENTOS ",
            5 => "QUINIENTOS ",
            6 => "SEISCIENTOS ",
            7 => "SETECIENTOS ",
            8 => "OCHOCIENTOS ",
            9 => "NOVECIENTOS ",
            _ => ""
        };
    }

    private static string Decenas(long numero)
    {
        if (numero < 10)
            return Unidades(numero);

        if (numero < 20)
            return numero switch
            {
                10 => "DIEZ",
                11 => "ONCE",
                12 => "DOCE",
                13 => "TRECE",
                14 => "CATORCE",
                15 => "QUINCE",
                16 => "DIECISÉIS",
                17 => "DIECISIETE",
                18 => "DIECIOCHO",
                19 => "DIECINUEVE",
                _ => ""
            };

        long decena = numero / 10;
        long unidad = numero % 10;

        string decenas = decena switch
        {
            2 => "VEINTE",
            3 => "TREINTA",
            4 => "CUARENTA",
            5 => "CINCUENTA",
            6 => "SESENTA",
            7 => "SETENTA",
            8 => "OCHENTA",
            9 => "NOVENTA",
            _ => ""
        };

        if (unidad == 0)
            return decenas;

        if (decena == 2)
            return "VEINTI" + Unidades(unidad).ToLower();

        return $"{decenas} Y {Unidades(unidad)}";
    }

    private static string Unidades(long numero)
    {
        return numero switch
        {
            1 => "UN",
            2 => "DOS",
            3 => "TRES",
            4 => "CUATRO",
            5 => "CINCO",
            6 => "SEIS",
            7 => "SIETE",
            8 => "OCHO",
            9 => "NUEVE",
            _ => ""
        };
    }
}
