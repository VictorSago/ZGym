using System.Threading.Tasks;

namespace ZGym.Core.Repositories
{
    public interface IUnitOfWork
    {
        IGymClassRepository GymClassRepository { get; }
        IUserGymClassRepository UserGymClassRepository { get; }

        Task CompeteAsync();
    }
}