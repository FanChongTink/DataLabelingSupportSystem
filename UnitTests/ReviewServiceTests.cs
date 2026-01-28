using BLL.Services;
using DAL.Interfaces;
using DTOs.Entities;
using DTOs.Requests;
using Moq;
using Xunit;

namespace UnitTests
{
    public class ReviewServiceTests
    {
        private readonly Mock<IAssignmentRepository> _mockAssignmentRepo;
        private readonly Mock<IRepository<ReviewLog>> _mockReviewLogRepo;
        private readonly Mock<IRepository<DataItem>> _mockDataItemRepo;
        private readonly Mock<IRepository<UserProjectStat>> _mockStatsRepo;
        private readonly Mock<IRepository<Project>> _mockProjectRepo;
        private readonly ReviewService _service;

        public ReviewServiceTests()
        {
            _mockAssignmentRepo = new Mock<IAssignmentRepository>();
            _mockReviewLogRepo = new Mock<IRepository<ReviewLog>>();
            _mockDataItemRepo = new Mock<IRepository<DataItem>>();
            _mockStatsRepo = new Mock<IRepository<UserProjectStat>>();
            _mockProjectRepo = new Mock<IRepository<Project>>();

            _service = new ReviewService(
                _mockAssignmentRepo.Object,
                _mockReviewLogRepo.Object,
                _mockDataItemRepo.Object,
                _mockStatsRepo.Object,
                _mockProjectRepo.Object);
        }

        [Fact]
        public async Task ReviewAssignmentAsync_Approved_UpdatesStatusAndStats()
        {
            // Arrange
            var assignmentId = 1;
            var reviewerId = "reviewer1";
            var annotatorId = "annotator1";
            var projectId = 10;
            var dataItemId = 100;

            var assignment = new Assignment
            {
                Id = assignmentId,
                AnnotatorId = annotatorId,
                ProjectId = projectId,
                DataItemId = dataItemId,
                Status = "Submitted"
            };

            var project = new Project
            {
                Id = projectId,
                PricePerLabel = 0.5m
            };

            var stats = new UserProjectStat
            {
                UserId = annotatorId,
                ProjectId = projectId,
                TotalAssigned = 1,
                TotalApproved = 0
            };

            var dataItem = new DataItem { Id = dataItemId };

            _mockAssignmentRepo.Setup(r => r.GetByIdAsync(assignmentId)).ReturnsAsync(assignment);
            _mockProjectRepo.Setup(r => r.GetByIdAsync(projectId)).ReturnsAsync(project);
            _mockStatsRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<UserProjectStat> { stats });
            _mockDataItemRepo.Setup(r => r.GetByIdAsync(dataItemId)).ReturnsAsync(dataItem);

            var request = new ReviewRequest
            {
                AssignmentId = assignmentId,
                IsApproved = true,
                Comment = "Good job"
            };

            // Act
            await _service.ReviewAssignmentAsync(reviewerId, request);

            // Assert
            Assert.Equal("Completed", assignment.Status);
            Assert.Equal(1, stats.TotalApproved);
            Assert.Equal(0.5m, stats.EstimatedEarnings);
            Assert.Equal("Done", dataItem.Status);
            _mockReviewLogRepo.Verify(r => r.AddAsync(It.IsAny<ReviewLog>()), Times.Once);
            _mockAssignmentRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
