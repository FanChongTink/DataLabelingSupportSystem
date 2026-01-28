using BLL.Services;
using DAL.Interfaces;
using DTOs.Entities;
using Moq;
using Xunit;

namespace UnitTests
{
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _mockProjectRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IRepository<UserProjectStat>> _mockStatsRepo;
        private readonly Mock<IRepository<Invoice>> _mockInvoiceRepo;
        private readonly ProjectService _service;

        public ProjectServiceTests()
        {
            _mockProjectRepo = new Mock<IProjectRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockStatsRepo = new Mock<IRepository<UserProjectStat>>();
            _mockInvoiceRepo = new Mock<IRepository<Invoice>>();

            _service = new ProjectService(
                _mockProjectRepo.Object,
                _mockUserRepo.Object,
                _mockStatsRepo.Object,
                _mockInvoiceRepo.Object);
        }

        [Fact]
        public async Task GetProjectStatisticsAsync_ReturnsCorrectStats()
        {
            // Arrange
            var projectId = 1;
            var project = new Project
            {
                Id = projectId,
                Name = "Test Project",
                LabelClasses = new List<LabelClass> { new LabelClass { Id = 1, Name = "Cat" } }
            };

            var dataItems = new List<DataItem>
            {
                new DataItem { Id = 101, Status = "Completed" },
                new DataItem { Id = 102, Status = "New" }
            };

            // Add assignments to data items
            var assignment1 = new Assignment { Id = 1, Status = "Completed", AnnotatorId = "user1", DataItem = dataItems[0], Project = project, Annotator = new User { FullName = "User One" } };
            assignment1.Annotations.Add(new Annotation { ClassId = 1 });
            dataItems[0].Assignments.Add(assignment1);

            project.DataItems = dataItems;

            _mockProjectRepo.Setup(r => r.GetProjectWithStatsDataAsync(projectId))
                .ReturnsAsync(project);

            _mockStatsRepo.Setup(r => r.GetAllAsync())
                .ReturnsAsync(new List<UserProjectStat>());

            // Act
            var result = await _service.GetProjectStatisticsAsync(projectId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalItems);
            Assert.Equal(1, result.CompletedItems);
            Assert.Equal(1, result.TotalAssignments);
            Assert.Equal(1, result.ApprovedAssignments);
            Assert.Equal(1, result.LabelDistributions[0].Count);
        }
    }
}
