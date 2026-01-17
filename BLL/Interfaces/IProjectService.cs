using DTOs.Entities;
using DTOs.Requests;
using DTOs.Responses;

namespace BLL.Interfaces
{
    public interface IProjectService
    {
        Task<Project> CreateProjectAsync(string managerId, CreateProjectRequest request);
        Task ImportDataItemsAsync(int projectId, List<string> storageUrls);
        Task<ProjectDetailResponse?> GetProjectDetailsAsync(int projectId);
        Task<List<ProjectSummaryResponse>> GetProjectsByManagerAsync(string managerId);
        Task UpdateProjectAsync(int projectId, UpdateProjectRequest request);
        Task DeleteProjectAsync(int projectId);
    }
}