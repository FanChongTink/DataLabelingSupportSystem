using BLL.Services;
using DAL.Interfaces;
using DTOs.Entities;
using DTOs.Requests;
using Moq;
using Xunit;

namespace UnitTests
{
    public class TaskServiceTests
    {
        private readonly Mock<IAssignmentRepository> _mockAssignmentRepo;
        private readonly Mock<IRepository<DataItem>> _mockDataItemRepo;
        private readonly Mock<IRepository<Annotation>> _mockAnnotationRepo;
        private readonly Mock<IRepository<UserProjectStat>> _mockStatsRepo;
        private readonly TaskService _service;

        public TaskServiceTests()
        {
            _mockAssignmentRepo = new Mock<IAssignmentRepository>();
            _mockDataItemRepo = new Mock<IRepository<DataItem>>();
            _mockAnnotationRepo = new Mock<IRepository<Annotation>>();
            _mockStatsRepo = new Mock<IRepository<UserProjectStat>>();

            _service = new TaskService(
                _mockAssignmentRepo.Object,
                _mockDataItemRepo.Object,
                _mockAnnotationRepo.Object,
                _mockStatsRepo.Object);
        }

        [Fact]
        public async Task AssignTasksToAnnotatorAsync_AssignsTasksAndUpdatesStats()
        {
            // Arrange
            var projectId = 1;
            var annotatorId = "annotator1";
            var quantity = 2;

            var dataItems = new List<DataItem>
            {
                new DataItem { Id = 101, Status = "New" },
                new DataItem { Id = 102, Status = "New" }
            };

            _mockAssignmentRepo.Setup(r => r.GetUnassignedDataItemsAsync(projectId, quantity))
                .ReturnsAsync(dataItems);

            _mockStatsRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<UserProjectStat>());

            var request = new AssignTaskRequest
            {
                ProjectId = projectId,
                AnnotatorId = annotatorId,
                Quantity = quantity
            };

            // Act
            await _service.AssignTasksToAnnotatorAsync(request);

            // Assert
            Assert.Equal("Assigned", dataItems[0].Status);
            Assert.Equal("Assigned", dataItems[1].Status);

            _mockAssignmentRepo.Verify(r => r.AddAsync(It.IsAny<Assignment>()), Times.Exactly(2));
            _mockStatsRepo.Verify(r => r.AddAsync(It.IsAny<UserProjectStat>()), Times.Once);
            _mockAssignmentRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
