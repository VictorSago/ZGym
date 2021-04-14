using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ZGym.Core.Entities;
using ZGym.Data.Data;

namespace ZGym.Data.Repositories
{
    public class GymClassRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public GymClassRepository(ApplicationDbContext context)
        {
            this._dbContext = context;
        }

        public async Task<GymClass> GetAsync(int? id)
        {
            return await _dbContext.GymClasses
                            .Include(g => g.AttendingMembers)
                            .ThenInclude(a => a.ApplicationUser)
                            .FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task<IEnumerable<GymClass>> GetAllAsync()
        {
            return await _dbContext.GymClasses.ToListAsync();
        }

        public async Task<IEnumerable<GymClass>> GetAllWithBookingsAsync()
        {
            return await _dbContext.GymClasses
                            .Include(g => g.AttendingMembers)
                            .ToListAsync();
        }

        public async Task<IEnumerable<GymClass>> GetHistoryAsync()
        {
            return await _dbContext.GymClasses
                            .IgnoreQueryFilters()
                            .Where(g => g.StartTime < DateTime.Now)
                            .ToListAsync();
        }

        public void Add(GymClass gymClass)
        {
            _dbContext.Add(gymClass);
        }
        public void Update(GymClass gymClass)
        {
            _dbContext.Update(gymClass);
        }

        public void Remove(GymClass gymClass)
        {
            _dbContext.Remove(gymClass);
        }

        public bool Any(int id)
        {
            return _dbContext.GymClasses.Any(g => g.Id == id);
        }
    }
}