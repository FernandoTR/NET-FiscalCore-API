using FiscalCore.Application.DTOs.Certificate;
using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Interfaces.Logging;
using FiscalCore.Application.Interfaces.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace FiscalCore.Infrastructure.CfdiBuilder;

public sealed class CfdiSealService : ICfdiSealService
{
    private readonly IEncryptionService _encryptionService;
    private readonly ILogService _logService;

    public CfdiSealService(IEncryptionService encryptionService, ILogService logService)
    {
        _encryptionService = encryptionService;
        _logService = logService;
    }

    public ResponseDto Seal(string cadenaOriginal, CertificateResponse csd)
    {
        if (string.IsNullOrWhiteSpace(cadenaOriginal))
            return ResponseFactory.Error("La cadena original es obligatoria.");

        if (csd == null)
            return ResponseFactory.Error("El certificado no puede ser nulo.");

        if (csd.CerFile == null || csd.CerFile.Length == 0)
            return ResponseFactory.Error("El archivo .cer es inválido.");

        if (csd.KeyFile == null || csd.KeyFile.Length == 0)
            return ResponseFactory.Error("El archivo .key es inválido.");

        try
        {

            // 1. Cadena original → bytes UTF-8
            byte[] data = Encoding.UTF8.GetBytes(cadenaOriginal);

            // 2. Cargar certificado (liberación garantizada)
            using var cert = new X509Certificate2(csd.CerFile, (string)null, X509KeyStorageFlags.EphemeralKeySet);

            // 3. Número de certificado (serial X.509 convertido a decimal conforme Anexo 20)
            string noCertificado = GetCertificateSerialNumber(cert);

            // 4. Certificado Base64 (sin encabezados PEM)
            string certificadoBase64 = Convert.ToBase64String(cert.RawData);

            // 5. Desencriptar password de la llave
            string keyPassword = _encryptionService.Decrypt(csd.EncryptedKeyPassword);

            // 6. Cargar llave privada y firmar
            using var rsa = LoadPrivateKey(csd.KeyFile, keyPassword);

            byte[] signature = rsa.SignData(
                data,
                HashAlgorithmName.SHA256,
                RSASignaturePadding.Pkcs1
            );

            var cfdiSealResponse = new CfdiSealResponse
            {
                Sello = Convert.ToBase64String(signature),
                Certificado = certificadoBase64,
                NoCertificado = noCertificado
            };

            return ResponseFactory.Success<CfdiSealResponse>(cfdiSealResponse, "CDFI sellado correctamente.");
        }
        catch (CryptographicException ex)
        {
            return ResponseFactory.Exception(ex, "Error criptográfico al sellar el CFDI.");
        }
        catch (Exception ex)
        {
            return ResponseFactory.Exception(ex, "Error inesperado al sellar el CFDI.");
        }

    }

    public ResponseDto Certificate(CertificateResponse csd)
    {
        if (csd == null)
            return ResponseFactory.Error("El certificado no puede ser nulo.");

        if (csd.CerFile == null || csd.CerFile.Length == 0)
            return ResponseFactory.Error("El archivo .cer es inválido.");

        if (csd.KeyFile == null || csd.KeyFile.Length == 0)
            return ResponseFactory.Error("El archivo .key es inválido.");

        try
        {
            // 1. Cargar certificado (liberación garantizada)
            using var cert = new X509Certificate2(csd.CerFile, (string)null, X509KeyStorageFlags.EphemeralKeySet);

            // 2. Número de certificado (serial X.509 convertido a decimal conforme Anexo 20)
            string noCertificado = GetCertificateSerialNumber(cert);

            // 3. Certificado Base64 (sin encabezados PEM)
            string certificadoBase64 = Convert.ToBase64String(cert.RawData);

            var cfdiSealResponse = new CfdiSealResponse
            {
                Certificado = certificadoBase64,
                NoCertificado = noCertificado
            };

            return ResponseFactory.Success<CfdiSealResponse>(cfdiSealResponse, "Certificado recuperado correctamente.");          
        }
        catch (Exception ex)
        {
            return ResponseFactory.Exception(ex, "Error inesperado al recuperar el certificado.");
        }

    }

    private static RSA LoadPrivateKey(byte[] keyBytes, string password)
    {
        var rsa = RSA.Create();

        rsa.ImportEncryptedPkcs8PrivateKey(
            password,
            keyBytes,
            out _
        );

        return rsa;
    }

    private static string GetCertificateSerialNumber(X509Certificate2 cert)
    {
        // 1. Serial HEX (invertido)
        var hex = cert.SerialNumber;

        // 2. HEX → bytes
        var bytes = new byte[hex.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        }

        // 3. Invertir bytes
        Array.Reverse(bytes);

        // 4. Bytes → ASCII 
        return Encoding.ASCII.GetString(bytes);
    }

    
}
