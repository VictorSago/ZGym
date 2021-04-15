using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZGym.Core.Entities;
using ZGym.Core.Repositories;
using ZGym.Data.Data;

namespace ZGym.Data.Repositories
{
    public class UserGymClassRepository : IUserGymClassRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserGymClassRepository(ApplicationDbContext context)
        {
            this._dbContext = context;
        }

        public async Task<ApplicationUserGymClass> GetAttending(int? id, string userId)
        {
            return await _dbContext.UserGymClasses.FindAsync(userId, id);
        }

        public async Task<IEnumerable<GymClass>> GetBookingsAsync(string userId)
        {
            return await _dbContext.UserGymClasses
                                    .Include(a => a.GymClass)
                                    .ThenInclude(g => g.AttendingMembers)
                                    .IgnoreQueryFilters()
                                    .Where(a => a.ApplicationUserId == userId)
                                    .Select(a => a.GymClass)
                                    .ToListAsync();
        }

        public void Add(ApplicationUserGymClass attending)
        {
            _dbContext.Add(attending);
        }

        public void Remove(ApplicationUserGymClass attending)
        {
            _dbContext.Remove(attending);
        }
    }
}