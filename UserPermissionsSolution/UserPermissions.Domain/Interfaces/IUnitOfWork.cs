using Microsoft.EntityFrameworkCore.Storage;

namespace UserPermissions.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        Task CompleteAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}