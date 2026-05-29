
using FiscalCore.Application.Abstractions;
using FiscalCore.Application.DTOs.Certificate;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.Interfaces.Certificate;
using FiscalCore.Application.Interfaces.Logging;
using FiscalCore.Application.Interfaces.Message;
using FiscalCore.Application.Interfaces.Security;
using FiscalCore.Domain.Interfaces.Certificates;
using FluentValidation;

namespace FiscalCore.Application.Services.Certificate;

public sealed class CertificateService : ICertificateService
{
    private readonly ILogService _logService;
    private readonly ICertificateRepository _repository;
    private readonly IEncryptionService _encryptionService;
    private readonly IUnitOfWork _uow;
    private readonly IMessagesProvider _messagesProvider;
    private readonly IValidator<CreateCertificateRequest> _validatorCreateCertificate;
    private readonly IValidator<UpdateCertificateRequest> _validatorUpdateCertificate;

    public CertificateService(
        ICertificateRepository repository,
        IEncryptionService encryptionService,
        ILogService logService,
        IUnitOfWork unitOfWork,
        IMessagesProvider messagesProvider,
        IValidator<CreateCertificateRequest> validatorCreateCetificate,
        IValidator<UpdateCertificateRequest> validatorUpdateCetificate
        )
    {
        _repository = repository;
        _encryptionService = encryptionService;
        _logService = logService;
        _uow = unitOfWork;
        _messagesProvider = messagesProvider;
        _validatorCreateCertificate = validatorCreateCetificate;
        _validatorUpdateCertificate = validatorUpdateCetificate;

    }

    public async Task<CertificateResponse?> GetByIdAsync(Guid certificateId)
    {
        var cert = await _repository.GetByIdAsync(certificateId);
        return cert is null ? null : Map(cert);
    }

    public async Task<IReadOnlyList<CertificateResponse>> GetByUserAsync(Guid userId)
    {
        var list = await _repository.GetByUserAsync(userId);
        return list.Select(Map).ToList();
    }

    public async Task<CertificateResponse?> GetActiveByRfcAsync(string rfc)
    {
        var cert = await _repository.GetActiveByRfcAsync(rfc);
        return cert is null ? null : Map(cert);
    }

    public async Task<ResponseDto> CreateAsync(CreateCertificateRequest certificate)
    {
        await _uow.BeginTransactionAsync();

        try
        {

            // 1 Validar request
            var validationResult = _validatorCreateCertificate.Validate(certificate);

            if (!validationResult.IsValid)
            {
                IEnumerable<ResponseErrorDetailDto> errors = validationResult.Errors.Select(e => new ResponseErrorDetailDto
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                });
                return ResponseFactory.Error(_messagesProvider.GetError("CertificateCreationFailed"), errors);
            }


            var entity = new Domain.Entities.Certificate
            {
                CertificateId = Guid.NewGuid(),
                UserId = certificate.UserId,
                Rfc = certificate.Rfc,
                SerialNumber = certificate.SerialNumber,
                CertificateType = certificate.CertificateType,
                ValidFrom = certificate.ValidFrom,
                ValidTo = certificate.ValidTo,
                CerFile = certificate.CerFile,
                KeyFile = certificate.KeyFile,
                EncryptedKeyPassword = _encryptionService.Encrypt(certificate.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _repository.Add(entity);

            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return ResponseFactory.Success<Guid>(entity.CertificateId, _messagesProvider.GetMessage("CertificateCreatedSuccessfully"));

        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();
            _logService.ErrorLog($"{nameof(CertificateService)}.{nameof(CreateAsync)}", ex);

            return ResponseFactory.Exception(ex, _messagesProvider.GetError("CertificateCreationFailed"));
        }
    }

    public async Task<ResponseDto> UpdateAsync(UpdateCertificateRequest certificate)
    {
        await _uow.BeginTransactionAsync();

        try
        {
            // 1 Validar request
            var validationResult = _validatorUpdateCertificate.Validate(certificate);

            if (!validationResult.IsValid)
            {
                IEnumerable<ResponseErrorDetailDto> errors = validationResult.Errors.Select(e => new ResponseErrorDetailDto
                {
                    Field = e.PropertyName,
                    Message = e.ErrorMessage
                });
                return ResponseFactory.Error(_messagesProvider.GetError("CertificateUpdateFailed"), errors);
            }

            var entity = new Domain.Entities.Certificate
            {
                CertificateId = Guid.NewGuid(),
                UserId = certificate.UserId,
                Rfc = certificate.Rfc,
                SerialNumber = certificate.SerialNumber,
                CertificateType = certificate.CertificateType,
                ValidFrom = certificate.ValidFrom,
                ValidTo = certificate.ValidTo,
                CerFile = certificate.CerFile,
                KeyFile = certificate.KeyFile,
                EncryptedKeyPassword = _encryptionService.Encrypt(certificate.Password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _repository.Update(entity);

            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return ResponseFactory.Success(_messagesProvider.GetMessage("CertificateUpdateSuccessfully"));

        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();
            _logService.ErrorLog($"{nameof(CertificateService)}.{nameof(UpdateAsync)}", ex);

            return ResponseFactory.Exception(ex, _messagesProvider.GetError("CertificateUpdateFailed"));
        }
    }

    public async Task<ResponseDto> DeleteAsync(Guid certificateId)
    {
        await _uow.BeginTransactionAsync();

        try
        {
            // 1️ Obtener usuario existente
            var certificate = await _repository.GetByIdAsync(certificateId);

            if (certificate is null)
            {
                IEnumerable<ResponseErrorDetailDto> errors = new[]
               {
                    new ResponseErrorDetailDto
                    {
                        Field = "certificateId",
                        Message = _messagesProvider.GetError("CertificateNotFound2")
                    }
                };

                return ResponseFactory.Error(_messagesProvider.GetError("CertificateDeleteFailed"), errors);
            }

            _repository.Delete(certificate);

            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return ResponseFactory.Success(_messagesProvider.GetMessage("CertificateDeleteSuccessfully"));

        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();
            _logService.ErrorLog($"{nameof(CertificateService)}.{nameof(DeleteAsync)}", ex);

            return ResponseFactory.Exception(ex, _messagesProvider.GetError("CertificateDeleteFailed"));
        }
    }

    private static CertificateResponse Map(Domain.Entities.Certificate c)
        => new()
        {
            CertificateId = c.CertificateId,
            UserId = c.UserId,
            Rfc = c.Rfc,
            SerialNumber = c.SerialNumber,
            CertificateType = c.CertificateType,
            ValidFrom = c.ValidFrom,
            ValidTo = c.ValidTo,
            IsActive = c.IsActive,
            CerFile = c.CerFile,
            KeyFile = c.KeyFile,
            EncryptedKeyPassword = c.EncryptedKeyPassword
        };
}

