using BLL.Interfaces;
using DAL.Interfaces;
using DTOs.Entities;
using DTOs.Requests;
using DTOs.Responses;

namespace BLL.Services
{
    public class TaskService : ITaskService
    {
        private readonly IAssignmentRepository _assignmentRepo;
        private readonly IRepository<DataItem> _dataItemRepo;
        private readonly IRepository<Annotation> _annotationRepo;

        public TaskService(
            IAssignmentRepository assignmentRepo,
            IRepository<DataItem> dataItemRepo,
            IRepository<Annotation> annotationRepo)
        {
            _assignmentRepo = assignmentRepo;
            _dataItemRepo = dataItemRepo;
            _annotationRepo = annotationRepo;
        }

        public async Task AssignTasksToAnnotatorAsync(AssignTaskRequest request)
        {
            var dataItems = await _assignmentRepo.GetUnassignedDataItemsAsync(request.ProjectId, request.Quantity);
            if (!dataItems.Any()) throw new Exception("Not enough available data items.");

            foreach (var item in dataItems)
            {
                var assignment = new Assignment
                {
                    ProjectId = request.ProjectId,
                    DataItemId = item.Id,
                    AnnotatorId = request.AnnotatorId,
                    Status = "Assigned",
                    AssignedDate = DateTime.UtcNow
                };

                item.Status = "Assigned";
                _dataItemRepo.Update(item);
                await _assignmentRepo.AddAsync(assignment);
            }
            await _assignmentRepo.SaveChangesAsync();
        }

        public async Task<List<TaskResponse>> GetMyTasksAsync(int projectId, string annotatorId)
        {
            var assignments = await _assignmentRepo.GetAssignmentsByAnnotatorAsync(projectId, annotatorId);

            return assignments.Select(a => new TaskResponse
            {
                AssignmentId = a.Id,
                DataItemId = a.DataItemId,
                StorageUrl = a.DataItem?.StorageUrl ?? "",
                Status = a.Status
            }).ToList();
        }

        public async Task<TaskResponse?> GetTaskDetailAsync(int assignmentId, string annotatorId)
        {
            var assignment = await _assignmentRepo.GetAssignmentWithDetailsAsync(assignmentId);
            if (assignment == null) return null;
            if (assignment.AnnotatorId != annotatorId) throw new Exception("Unauthorized");

            // Nếu trạng thái là Assigned, chuyển sang InProgress khi mở ra làm
            if (assignment.Status == "Assigned")
            {
                assignment.Status = "InProgress";
                _assignmentRepo.Update(assignment);
                await _assignmentRepo.SaveChangesAsync();
            }

            return new TaskResponse
            {
                AssignmentId = assignment.Id,
                DataItemId = assignment.DataItemId,
                StorageUrl = assignment.DataItem?.StorageUrl ?? "",
                ProjectName = assignment.Project?.Name ?? "",
                Status = assignment.Status,
                Labels = assignment.Project?.LabelClasses.Select(l => new LabelResponse
                {
                    Id = l.Id,
                    Name = l.Name,
                    Color = l.Color
                }).ToList() ?? new List<LabelResponse>(),

                ExistingAnnotations = assignment.Annotations.Select(an => new
                {
                    an.ClassId,
                    an.Value
                }).ToList<object>()
            };
        }

        public async Task SubmitTaskAsync(string annotatorId, SubmitAnnotationRequest request)
        {
            var assignment = await _assignmentRepo.GetAssignmentWithDetailsAsync(request.AssignmentId);
            if (assignment == null) throw new Exception("Task not found");

            if (assignment.AnnotatorId != annotatorId)
                throw new Exception("You are not authorized to submit this task.");

            if (assignment.Status == "Completed")
                throw new Exception("This task is already completed.");

            if (assignment.Annotations != null && assignment.Annotations.Any())
            {
                foreach (var oldAnno in assignment.Annotations)
                {
                    _annotationRepo.Delete(oldAnno);
                }
            }

            foreach (var item in request.Annotations)
            {
                await _annotationRepo.AddAsync(new Annotation
                {
                    AssignmentId = assignment.Id,
                    ClassId = item.LabelClassId,
                    Value = item.ValueJson
                });
            }

            assignment.Status = "Submitted";

            _assignmentRepo.Update(assignment);
            await _assignmentRepo.SaveChangesAsync();
        }
    }
}