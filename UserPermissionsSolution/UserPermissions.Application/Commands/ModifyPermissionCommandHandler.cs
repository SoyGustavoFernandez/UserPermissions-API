using MediatR;
using Newtonsoft.Json;
using Serilog;
using UserPermissions.Application.DTOs;
using UserPermissions.Domain.Interfaces;
using UserPermissions.Infrastructure.Elasticsearch;
using UserPermissions.Infrastructure.Kafka;

namespace UserPermissions.Application.Commands
{
    public class ModifyPermissionCommandHandler : IRequestHandler<ModifyPermissionCommand, bool>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IKafkaProducerService _kafkaProducer;

        public ModifyPermissionCommandHandler(IPermissionRepository permissionRepository, IUnitOfWork unitOfWork, IElasticsearchService elasticsearchService, IKafkaProducerService kafkaProducer)
        {
            _permissionRepository = permissionRepository;
            _unitOfWork = unitOfWork;
            _elasticsearchService = elasticsearchService;
            _kafkaProducer = kafkaProducer;
        }

        public async Task<bool> Handle(ModifyPermissionCommand request, CancellationToken cancellationToken)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var permission = await _permissionRepository.GetByIdAsync(request.PermissionId);
                    if (permission == null)
                    {
                        Log.Warning($"Permission with ID {request.PermissionId} not found");
                        await transaction.RollbackAsync();
                        return false;
                    }

                    permission.EmployeeID = request.EmployeeId;
                    permission.PermissionTypeID = request.PermissionTypeId;

                    _permissionRepository.UpdateAsync(permission);
                    await _unitOfWork.CompleteAsync();

                    await _elasticsearchService.IndexPermissionAsync(permission);

                    var kafkaMessage = new KafkaMessageDto
                    {
                        Id = Guid.NewGuid(),
                        NameOperation = "modify",
                        PermissionId = permission.PermissionID,
                        Timestamp = DateTime.UtcNow
                    };
                    await _kafkaProducer.SendMessageAsync("permissions-operations", JsonConvert.SerializeObject(kafkaMessage));

                    await transaction.CommitAsync();
                    return true;
                }
                catch(Exception ex)
                {
                    await transaction.RollbackAsync();
                    Log.Error(ex, "Error while modifying permission");
                    return false;
                }
            }
        }
    }
}