using FiscalCore.Application.Abstractions;
using FiscalCore.Application.BackgroundJobs;
using FiscalCore.Application.DTOs.Cfdis;
using FiscalCore.Application.DTOs.Common;
using FiscalCore.Application.DTOs.Pac;
using FiscalCore.Application.Interfaces.Cfdis;
using FiscalCore.Application.Interfaces.Logging;
using FiscalCore.Domain.Entities;
using FiscalCore.Domain.Interfaces.Cfdis;
using FiscalCore.Domain.Interfaces.Stamping;
using FiscalCore.Domain.Interfaces.Users;

namespace FiscalCore.Application.Services.Cfdi;

public sealed class CfdiPersistenceService : ICfdiPersistenceService
{
    private readonly ILogService _logService;
    private readonly IUnitOfWork _uow;
    private readonly ICfdiRepository _cfdiRepository;
    private readonly ICfdiXmlStore _cfdiXmlStore;
    private readonly ICfdiStatusHistoryStore _cfdiStatusHistoryStore;
    private readonly IStampMovementRepository _stampMovementRepository;
    private readonly IStampBalanceRepository _balanceRepository;
    private readonly ICfdiBackgroundJobDispatcher _cfdiBackgroundJobDispatcher;
    private readonly IUserRepository _userRepository;

    public CfdiPersistenceService(
        ILogService logService,
        IUnitOfWork uow,
        ICfdiRepository cfdiRepository,
        ICfdiXmlStore cfdiXmlStore,
        ICfdiStatusHistoryStore cfdiStatusHistoryStore,
        IStampMovementRepository stampMovementRepository,
        IStampBalanceRepository balanceRepository,
        ICfdiBackgroundJobDispatcher cfdiBackgroundJobDispatcher,
        IUserRepository userRepository)
    {
        _logService = logService;
        _uow = uow;
        _cfdiRepository = cfdiRepository;
        _cfdiXmlStore = cfdiXmlStore;
        _cfdiStatusHistoryStore = cfdiStatusHistoryStore;
        _stampMovementRepository = stampMovementRepository;
        _balanceRepository = balanceRepository;
        _cfdiBackgroundJobDispatcher = cfdiBackgroundJobDispatcher;
        _userRepository = userRepository;
    }

    public async Task<ResponseDto> ExecuteAsync(PacStampingDto pacStampingResult, CreateCfdiRequest createCfdiRequest, Guid userId, Guid stampBalanceId)
    {
        await _uow.BeginTransactionAsync();

        try
        {
            // Obtener información del usuario
            var user = await _userRepository.GetByIdAsync(userId);

            // Persistir el CFDI
            var cfdi = new Domain.Entities.Cfdi
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Uuid = Guid.Parse(pacStampingResult.Uuid),
                RfcEmisor = createCfdiRequest.Emisor.Rfc,
                RfcReceptor = createCfdiRequest.Receptor.Rfc,
                Version = createCfdiRequest.Comprobante.Version,
                Total = createCfdiRequest.Comprobante.Total,
                Currency = createCfdiRequest.Comprobante.Moneda,
                IssueDate = createCfdiRequest.Comprobante.Fecha,
                CurrentStatus = "Vigente",
                CreatedAt = DateTime.UtcNow,
            };

            _cfdiRepository.Add(cfdi);


            // Persistir el XML del CFDI
            var cfdiXml = new CfdiXmlDto
            {
                CfdiId = cfdi.Id,
                XmlContent = pacStampingResult.Xml,
                CreatedAt = DateTime.UtcNow
            };

            _cfdiXmlStore.Add(cfdiXml);


            // Persistir el historial de estatus del CFDI
            var cfdiStatusHistory = new CfdiStatusHistoryDto
            {
                CfdiId = cfdi.Id,
                Status = "Vigente",
                ChangedAt = DateTime.UtcNow,
            };

            _cfdiStatusHistoryStore.Add(cfdiStatusHistory);


            // Registrar movimiento de timbre
            var stampMovement = new StampMovement
            {
                StampBalanceId = stampBalanceId,
                CfdiId = cfdi.Id,
                Amount = 1,
                CreatedAt = DateTime.UtcNow
            };

            _stampMovementRepository.Add(stampMovement);

            // Decrementar el balance de timbres
            _balanceRepository.Decrement(userId, 1);

            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            // 🔔 Disparar job en background
            _cfdiBackgroundJobDispatcher.EnqueueGeneratePdfAndSendEmail(
                cfdi.Id,
                pacStampingResult.Xml,
                user.Email
            );

            return ResponseFactory.Success("CFDI guardado exitosamente.");
        }
        catch (Exception ex)
        {
            await _uow.RollbackAsync();
            _logService.ErrorLog($"{nameof(CfdiPersistenceService)}.{nameof(ExecuteAsync)}", ex);

            return ResponseFactory.Exception(ex, "Se produjo un error al intentar guardar el CFDI.");
        }
    }
}
