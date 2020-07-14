using System.Threading.Tasks;

namespace coretest.Domain.Repositories
{
    public interface IUnitOfWork
    {
        Task CompleteAsync();
    }
}
