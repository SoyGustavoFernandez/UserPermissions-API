using AutoMapper;
using Moq;
using UserPermissions.Application.DTOs;
using UserPermissions.Application.Queries;
using UserPermissions.Domain.Entities;
using UserPermissions.Domain.Interfaces;

namespace UserPermissions.Tests.UnitTests
{
    public class GetPermissionsQueryHandlerTests
    {
        [Fact]
        public async Task Handle_ReturnListOfPermissions()
        {
            //Arrange
            var mockRepository = new Mock<IPermissionRepository>();
            var mockMapper = new Mock<IMapper>();

            var permissions = new List<Permission>
            {
                new() { PermissionID = 1, EmployeeID = 1, PermissionTypeID = 1, RequestDate = DateTime.UtcNow },
                new() { PermissionID = 2, EmployeeID = 2, PermissionTypeID = 2, RequestDate = DateTime.UtcNow }
            };

            var permissionDTOs = new List<PermissionDTO>
            {
                new() { PermissionID = 1, EmployeeID = 1, PermissionTypeID = 1, RequestDate = DateTime.UtcNow },
                new() { PermissionID = 2, EmployeeID = 2, PermissionTypeID = 2, RequestDate = DateTime.UtcNow }
            };

            mockRepository.Setup(repo => repo.GetAllAsync()).ReturnsAsync(permissions);
            mockMapper.Setup(mapper => mapper.Map<IEnumerable<PermissionDTO>>(permissions)).Returns(permissionDTOs);

            var handler = new GetPermissionsQueryHandler(mockRepository.Object, mockMapper.Object);

            //Act
            var result = await handler.Handle(new GetPermissionsQuery(), CancellationToken.None);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());

            mockMapper.Verify(mapper => mapper.Map<IEnumerable<PermissionDTO>>(permissions), Times.Once);

            var firstPermission = result.First();
            Assert.Equal(1, firstPermission.PermissionID);
            Assert.Equal(1, firstPermission.EmployeeID);
            Assert.Equal(1, firstPermission.PermissionTypeID);
        }
    }
}