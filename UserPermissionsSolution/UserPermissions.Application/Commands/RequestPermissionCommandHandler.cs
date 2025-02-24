using MediatR;
using Newtonsoft.Json;
using Serilog;
using UserPermissions.Application.DTOs;
using UserPermissions.Domain.Entities;
using UserPermissions.Domain.Interfaces;
using UserPermissions.Infrastructure.Elasticsearch;
using UserPermissions.Infrastructure.Kafka;

namespace UserPermissions.Application.Commands
{
    public class RequestPermissionCommandHandler : IRequestHandler<RequestPermissionCommand, bool>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IKafkaProducerService _kafkaProducer;
        private readonly IElasticsearchService _elasticsearchService;
        private readonly IUnitOfWork _unitOfWork;

        public RequestPermissionCommandHandler(IPermissionRepository permissionRepository, IKafkaProducerService kafkaProducer, IElasticsearchService elasticsearchService, IUnitOfWork unitOfWork)
        {
            _permissionRepository = permissionRepository;
            _kafkaProducer = kafkaProducer;
            _elasticsearchService = elasticsearchService;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(RequestPermissionCommand request, CancellationToken cancellationToken)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    var permission = new Permission
                    {
                        EmployeeID = request.EmployeeId,
                        PermissionTypeID = request.PermissionTypeId,
                        RequestDate = DateTime.UtcNow
                    };

                    await _permissionRepository.AddAsync(permission);
                    await _unitOfWork.CompleteAsync();

                    await _elasticsearchService.IndexPermissionAsync(permission);

                    var kafkaMessage = new KafkaMessageDto
                    {
                        Id = Guid.NewGuid(),
                        NameOperation = "request",
                        PermissionId = permission.PermissionID,
                        Timestamp = DateTime.UtcNow
                    };

                    await _kafkaProducer.SendMessageAsync("permissions-operations", JsonConvert.SerializeObject(kafkaMessage));

                    await transaction.CommitAsync();

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    Log.Error(ex, $"Error en RequestPermissionCommandHandler para EmployeeID: {request.EmployeeId}");
                    return false;
                }
            }
        }
    }
}