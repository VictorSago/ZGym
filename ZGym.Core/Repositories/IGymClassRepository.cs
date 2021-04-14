using System.Collections.Generic;
using System.Threading.Tasks;
using ZGym.Core.Entities;

namespace ZGym.Core.Repositories
{
    public interface IGymClassRepository
    {
        Task<GymClass> GetAsync(int? id);
        Task<GymClass> GetWithUsersAsync(int? id);
        Task<GymClass> FindAsync(int? id);
        Task<IEnumerable<GymClass>> GetAllAsync();
        Task<IEnumerable<GymClass>> GetWithBookingsAsync();
        Task<IEnumerable<GymClass>> GetHistoryAsync();
        
        void Add(GymClass gymClass);
        void Remove(GymClass gymClass);
        void Update(GymClass gymClass);
        bool Any(int id);
    }
}