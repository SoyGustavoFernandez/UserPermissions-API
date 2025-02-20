using MediatR;
using UserPermissions.Domain.Entities;
using UserPermissions.Domain.Interfaces;
using UserPermissions.Infrastructure.Elasticsearch;
using UserPermissions.Infrastructure.Kafka;

namespace UserPermissions.Application.Commands
{
    public class RequestPermissionCommandHandler : IRequestHandler<RequestPermissionCommand, bool>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly KafkaProducerService _kafkaProducer;
        private readonly ElasticsearchService _elasticsearchService;

        public RequestPermissionCommandHandler(IPermissionRepository permissionRepository, KafkaProducerService kafkaProducer, ElasticsearchService elasticsearchService)
        {
            _permissionRepository = permissionRepository;
            _kafkaProducer = kafkaProducer;
            _elasticsearchService = elasticsearchService;
        }

        public async Task<bool> Handle(RequestPermissionCommand request, CancellationToken cancellationToken)
        {
            var permission = new Permission
            {
                EmployeeID = request.EmployeeId,
                PermissionTypeID = request.PermissionTypeId,
                RequestDate = DateTime.UtcNow
            };

            await _permissionRepository.AddAsync(permission);

            await _kafkaProducer.SendMessageAsync("permissions", $"Permission requested: {permission.Id}");

            await _elasticsearchService.IndexPermissionAsync(permission);

            return true;
        }
    }
}