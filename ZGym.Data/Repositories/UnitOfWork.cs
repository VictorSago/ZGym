
using System.Threading.Tasks;
using ZGym.Core.Repositories;
using ZGym.Data.Data;

namespace ZGym.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public IGymClassRepository GymClassRepository { get; private set; }
        public IUserGymClassRepository UserGymClassRepository { get; private set; }

        public UnitOfWork(ApplicationDbContext context)
        {
            _dbContext = context;
            GymClassRepository = new GymClassRepository(context);
            UserGymClassRepository = new UserGymClassRepository(context);
        }

        public async Task CompeteAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}