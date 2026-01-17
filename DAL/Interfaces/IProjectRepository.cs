using DTOs.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project?> GetProjectWithDetailsAsync(int id);
        Task<List<Project>> GetProjectsByManagerIdAsync(string managerId);
    }
}