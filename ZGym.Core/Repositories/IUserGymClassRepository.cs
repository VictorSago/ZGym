using System.Collections.Generic;
using System.Threading.Tasks;
using ZGym.Core.Entities;

namespace ZGym.Core.Repositories
{
    public interface IUserGymClassRepository
    {
        Task<ApplicationUserGymClass> GetAttending(int? id, string userId);
        Task<IEnumerable<ApplicationUserGymClass>> GetBookingsAsync(string userId);
        void Add(ApplicationUserGymClass attending);
        void Remove(ApplicationUserGymClass attending);
    }
}