using DAL.Interfaces;
using DTOs.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class ProjectRepository : Repository<Project>, IProjectRepository
    {
        public ProjectRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Project?> GetProjectWithDetailsAsync(int id)
        {
            return await _context.Projects
                .Include(p => p.Manager)        
                .Include(p => p.LabelClasses)   
                .Include(p => p.DataItems)     
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<Project>> GetProjectsByManagerIdAsync(string managerId)
        {
            return await _context.Projects
                .Include(p => p.DataItems)
                .OrderByDescending(p => p.Id)
                .Where(p => p.ManagerId == managerId)
                .ToListAsync();
        }
    }
}