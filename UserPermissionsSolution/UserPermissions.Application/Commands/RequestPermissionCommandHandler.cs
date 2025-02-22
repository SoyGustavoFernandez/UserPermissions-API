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
        private readonly IKafkaProducerService _kafkaProducer;
        private readonly IElasticsearchService _elasticsearchService;

        public RequestPermissionCommandHandler(IPermissionRepository permissionRepository, IKafkaProducerService kafkaProducer, IElasticsearchService elasticsearchService)
        {
            _permissionRepository = permissionRepository;
            _kafkaProducer = kafkaProducer;
            _elasticsearchService = elasticsearchService;
        }

        public async Task<bool> Handle(RequestPermissionCommand request, CancellationToken cancellationToken)
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

                await _kafkaProducer.SendMessageAsync("permissions", $"Permission requested: {permission.PermissionID}");

                await _elasticsearchService.IndexPermissionAsync(permission);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");

                return false;
            }
        }
    }
}