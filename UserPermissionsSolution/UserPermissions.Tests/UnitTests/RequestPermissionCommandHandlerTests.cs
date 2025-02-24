using Microsoft.EntityFrameworkCore.Storage;
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
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockTransaction = new Mock<IDbContextTransaction>();

            mockKafkaProducer.Setup(kafka => kafka.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            mockUnitOfWork.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            mockUnitOfWork.Setup(uow => uow.CompleteAsync()).Returns(Task.CompletedTask);

            var handler = new RequestPermissionCommandHandler(mockRepository.Object, mockKafkaProducer.Object, mockElasticsearchService.Object, mockUnitOfWork.Object);

            var command = new RequestPermissionCommand { EmployeeId = 1, PermissionTypeId = 1 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            mockRepository.Verify(repo => repo.AddAsync(
                It.Is<Permission>(p =>
                p.EmployeeID == command.EmployeeId &&
                p.PermissionTypeID == command.PermissionTypeId &&
                p.RequestDate <= DateTime.UtcNow
                ),
                It.IsAny<CancellationToken>()), Times.Once);

            mockKafkaProducer.Verify(k => k.SendMessageAsync("permissions-operations", It.IsAny<string>()), Times.Once);
            mockElasticsearchService.Verify(e => e.IndexPermissionAsync(It.IsAny<Permission>()), Times.Once);

            Assert.True(result);
        }

        [Fact]
        public async Task Handle_ReturnsFalse_WhenRepositoryThrowsException()
        {
            // Arrange
            var mockRepository = new Mock<IPermissionRepository>();
            var mockKafkaProducer = new Mock<IKafkaProducerService>();
            var mockElasticsearchService = new Mock<IElasticsearchService>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            var mockTransaction = new Mock<IDbContextTransaction>();

            mockKafkaProducer.Setup(kafka => kafka.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            mockUnitOfWork.Setup(uow => uow.BeginTransactionAsync()).ReturnsAsync(mockTransaction.Object);
            mockUnitOfWork.Setup(uow => uow.RollbackAsync()).Returns(Task.CompletedTask);

            mockRepository.Setup(repo => repo.AddAsync(It.IsAny<Permission>(), It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Error adding permission"));

            var handler = new RequestPermissionCommandHandler(mockRepository.Object, mockKafkaProducer.Object, mockElasticsearchService.Object, mockUnitOfWork.Object);

            var command = new RequestPermissionCommand { EmployeeId = 1, PermissionTypeId = 1 };

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            mockRepository.Verify(repo => repo.AddAsync(
                It.IsAny<Permission>(),
                It.IsAny<CancellationToken>()
                ), Times.Once); 
            mockKafkaProducer.Verify(kafka => kafka.SendMessageAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            mockElasticsearchService.Verify(elastic => elastic.IndexPermissionAsync(It.IsAny<Permission>()), Times.Never);

            mockTransaction.Verify(t => t.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
            mockUnitOfWork.Verify(uow => uow.CompleteAsync(), Times.Never);

            Assert.False(result);
        }
    }
}