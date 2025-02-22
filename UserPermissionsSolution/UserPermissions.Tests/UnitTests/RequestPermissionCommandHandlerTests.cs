using Moq;
using UserPermissions.Application.Commands;
using UserPermissions.Domain.Entities;
using UserPermissions.Domain.Interfaces;
using UserPermissions.Infrastructure.Elasticsearch;
using UserPermissions.Infrastructure.Kafka;

namespace UserPermissions.Tests.UnitTests
{
    public class RequestPermissionCommandHandlerTests
    {
        [Fact]
        public async Task Handle_AddsPermissionToRepository()
        {
            //Arrange
            var mockRepository = new Mock<IPermissionRepository>();
            var mockKafkaProducer = new Mock<IKafkaProducerService>();
            var mockElasticsearchService = new Mock<IElasticsearchService>();

            mockKafkaProducer.Setup(kafka => kafka.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var handler = new RequestPermissionCommandHandler(mockRepository.Object, mockKafkaProducer.Object, mockElasticsearchService.Object);

            var command = new RequestPermissionCommand { EmployeeId = 1, PermissionTypeId = 1 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            mockRepository.Verify(repo => repo.AddAsync(It.Is<Permission>(p =>
                p.EmployeeID == command.EmployeeId &&
                p.PermissionTypeID == command.PermissionTypeId &&
                p.RequestDate <= DateTime.UtcNow
            )), Times.Once);

            Assert.True(result);
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenRepositoryThrowsException()
        {
            // Arrange
            var mockRepository = new Mock<IPermissionRepository>();
            var mockKafkaProducer = new Mock<IKafkaProducerService>();
            var mockElasticsearchService = new Mock<IElasticsearchService>();

            mockKafkaProducer.Setup(kafka => kafka.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Permission>())).ThrowsAsync(new Exception("Error adding permission"));

            var handler = new RequestPermissionCommandHandler(
                mockRepository.Object,
                mockKafkaProducer.Object,
                mockElasticsearchService.Object
            );

            var command = new RequestPermissionCommand { EmployeeId = 1, PermissionTypeId = 1 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            mockKafkaProducer.Verify(kafka => kafka.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockElasticsearchService.Verify(elastic => elastic.IndexPermissionAsync(It.IsAny<Permission>()), Times.Never);

            Assert.False(result);
        }
    }
}