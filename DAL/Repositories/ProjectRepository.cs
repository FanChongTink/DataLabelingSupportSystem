using DAL.Interfaces;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;

namespace DAL.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Project?> GetProjectWithDetailsAsync(int id)
        {
            return await _dbSet
                .Include(p => p.LabelClasses)
                .Include(p => p.DataItems)
                .Include(p => p.Manager)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Project>> GetProjectsByManagerIdAsync(string managerId)
        {
            return await _dbSet
                .Include(p => p.DataItems)
                .Include(p => p.Manager)
                .Where(p => p.ManagerId == managerId)
                .OrderByDescending(p => p.Id)
                .ToListAsync();
        }
    }
}