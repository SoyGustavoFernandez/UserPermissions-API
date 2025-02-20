using MediatR;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using UserPermissions.Application.Commands;
using UserPermissions.Application.Queries;

namespace UserPermissions.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PermissionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("request")]
        public async Task<IActionResult> RequestPermission([FromBody] RequestPermissionCommand command)
        {
            Log.Information($"Requesting permission for employee {command.EmployeeId} of type {command.PermissionTypeId}");
            var result = await _mediator.Send(command);
            return result ? Ok(result) : BadRequest();
        }

        [HttpPut("modify")]
        public async Task<IActionResult> ModifyPermission([FromBody] ModifyPermissionCommand command)
        {
            Log.Information($"Modifying permission {command.PermissionId} for employee {command.EmployeeId} of type {command.PermissionTypeId}");
            var result = await _mediator.Send(command);
            return result ? Ok(result) : BadRequest();
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetPermissions()
        {
            Log.Information("Getting all permissions");
            var query = new GetPermissionsQuery();
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}